﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    /*
     * Singleton stuff!
     */
    private static bool shuttingDown = false;
    private static object lockObj = new object();
    private static SoundManager instance;

    public static SoundManager Instance
    {
        get
        {
            if (shuttingDown)
            {
                Debug.LogWarning("Shutting down SoundManger returning null.");
                return null;
            }
            lock (lockObj)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SoundManager>();

                    if (instance == null)
                    {
                        var sm = new GameObject("Sound Manager");
                        instance = sm.AddComponent<SoundManager>();

                    }

                    DontDestroyOnLoad(instance);
                }

                return instance;
            }
        }
    }

    private void OnApplicationQuit()
    {
        shuttingDown = true;
    }

    private void OnDestroy()
    {
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.clearHandle();
        }

        shuttingDown = true;
    }

    /*
     * Actual SoundManager
     */

    private void Awake()
    {
        var listener = FindObjectOfType<FMODUnity.StudioListener>();

        if (listener == null)
        {
            var camera = Camera.main.gameObject;

            if (camera != null)
            {
                camera.AddComponent<FMODUnity.StudioListener>();
            }
            else
            {
                Debug.LogError("No camera found to attach sound listener");
            }
        }
    }

    private void Start()
    {
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(SoundManager.BeatEventCallback);
    }

    // SFX
    public string PlayerThruster;
    public string EnemyHitEvent, PlanetHitEvent;

    public void StartThruster(GameObject target)
    {
        var eventEmitter = GetEmitter(target);

        if (eventEmitter.Event != PlayerThruster)
            eventEmitter.Event = PlayerThruster;

        eventEmitter.SetParameter("Power", 1.0f);

        if (eventEmitter.IsPlaying())
            return;

        eventEmitter.Play();
    }

    public void StopThruster(GameObject target)
    {
        var eventEmitter = GetEmitter(target);

        if (eventEmitter == null || eventEmitter.Event != PlayerThruster)
            return;
        
        eventEmitter.SetParameter("Power", 0.0f);
    }

    public void PlayEnemyHit(GameObject target)
    {
        var eventEmitter = GetEmitter(target);

        if (eventEmitter.IsPlaying())
            eventEmitter.Stop();

        eventEmitter.Event = EnemyHitEvent;

        eventEmitter.Play();
    }

    public void PlayPlanetHit(GameObject target)
    {
        var eventEmitter = GetEmitter(target);

        if (eventEmitter.IsPlaying())
            eventEmitter.Stop();

        eventEmitter.Event = PlanetHitEvent;

        eventEmitter.Play();
    }

    private FMODUnity.StudioEventEmitter GetEmitter(GameObject obj)
    {
        var eventEmitter = obj.GetComponent<FMODUnity.StudioEventEmitter>();
        if (eventEmitter == null)
        {
            eventEmitter = obj.AddComponent<FMODUnity.StudioEventEmitter>();
        }

        return eventEmitter;
    }

    // Music

    FMOD.Studio.EventInstance musicInstance;
    public string LevelMusicEvent;
    public string MenuMusicEvent;

    public void StartLevelMusic()
    {
        SetMusicInstance(LevelMusicEvent);
        musicInstance.start();
    }

    public void StopLevelMusic()
    {
        if (!CheckCurrentMusicEvent(LevelMusicEvent))
            return;

        musicInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

        if (state == FMOD.Studio.PLAYBACK_STATE.PLAYING)
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void StartMenuMusic()
    {
        SetMusicInstance(MenuMusicEvent);
        musicInstance.start();
    }

    public void StopMenuMusic()
    {
        if (!CheckCurrentMusicEvent(MenuMusicEvent))
            return;

        musicInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

        if (state == FMOD.Studio.PLAYBACK_STATE.PLAYING)
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void SetMusicParam(string name, float value)
    {
        if (!musicInstance.isValid())
            return;

        musicInstance.setParameterValue(name, value);
    }

    public float GetMusicParam(string name)
    {
        if (!musicInstance.isValid())
        {
            Debug.LogWarning("Music instance has not been started");
            return -1;
        }

        musicInstance.getParameterValue(name, out float value, out float finalValue);

        return value;
    }

    private void SetMusicInstance(string musicEvent) {
        if (CheckCurrentMusicEvent(musicEvent))
            return;

        if (musicInstance.isValid())
            musicInstance.clearHandle();

        musicInstance = FMODUnity.RuntimeManager.CreateInstance(musicEvent);
        musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
    }

    private bool CheckCurrentMusicEvent(string musicEvent)
    {
        if (!musicInstance.isValid())
            return false;

        musicInstance.getDescription(out FMOD.Studio.EventDescription description);
        description.getPath(out string path);

        return path == musicEvent;
    }

    // Beat Tracking

    FMOD.Studio.EVENT_CALLBACK beatCallback;

    public delegate void BeatCallback(int bar, int beat);
    private List<BeatCallback> beatCallbacks = new List<BeatCallback>();

    public void AddBeatCallback(BeatCallback cb)
    {
        beatCallbacks.Add(cb);
    }

    public void RemoveBeatCallback(BeatCallback cb)
    {
        beatCallbacks.Remove(cb);
    }

    private void TriggerBeatCallbacks(int bar, int beat)
    {
        beatCallbacks.ForEach(cb => cb(bar, beat));
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, FMOD.Studio.EventInstance instance, IntPtr parameterPtr)
    {
        if (SoundManager.shuttingDown)
            return FMOD.RESULT.OK;

        // Retrieve the user data
        switch (type)
        {
            case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                {
                    var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                    SoundManager.Instance.TriggerBeatCallbacks(parameter.bar, parameter.beat);
                }
                break;
            case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                {
                    //var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                }
                break;
        }
        return FMOD.RESULT.OK;
    }
}
