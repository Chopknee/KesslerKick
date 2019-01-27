using System;
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

    private void Start()
    {
        DontDestroyOnLoad(this);

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

        beatCallback = new FMOD.Studio.EVENT_CALLBACK(SoundManager.BeatEventCallback);
        beatCallbacks = new List<BeatCallback>();
    }

    private void OnLevelWasLoaded(int level)
    {
        Start();        
    }

    // SFX
    public string PlayerThruster;
    public string EnemyHitEvent, PlanetHitEvent, BossMelody, BossDeath, GameOver;
    public string UiButton, UiStart, UiQuit;

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

    public void PlayBossMelody(GameObject target)
    {
        var eventEmitter = GetEmitter(target);

        if (eventEmitter.IsPlaying())
            eventEmitter.Stop();

        eventEmitter.Event = BossMelody;

        eventEmitter.Play();
    }

    public void PlayBossDeath(GameObject target)
    {
        var eventEmitter = GetEmitter(target);

        if (eventEmitter.IsPlaying())
            eventEmitter.Stop();

        eventEmitter.Event = BossDeath;

        eventEmitter.Play();
    }

    public void PlayUiButton()
    {
        var eventEmitter = GetEmitter(Camera.main.gameObject);

        if (eventEmitter.IsPlaying())
            eventEmitter.Stop();

        eventEmitter.Event = UiButton;

        eventEmitter.Play();
    }

    public void PlayUiStart()
    {
        var eventEmitter = GetEmitter(Camera.main.gameObject);

        if (eventEmitter.IsPlaying())
            eventEmitter.Stop();

        eventEmitter.Event = UiStart;

        eventEmitter.Play();
    }

    public void PlayUiQuit()
    {
        var eventEmitter = GetEmitter(Camera.main.gameObject);

        if (eventEmitter.IsPlaying())
            eventEmitter.Stop();

        eventEmitter.Event = UiQuit;

        eventEmitter.Play();
    }

    public void StopSound(GameObject target)
    {
        var eventEmitter = GetEmitter(target);

        if (eventEmitter.IsPlaying())
            eventEmitter.Stop();
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
    public string AmbientMusicEvent;

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

        FMOD.Studio.PLAYBACK_STATE state;
        musicInstance.getPlaybackState(out state);

        if (state == FMOD.Studio.PLAYBACK_STATE.PLAYING)
            return;

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

    public void StartAmbientMusic()
    {
        SetMusicInstance(AmbientMusicEvent);
        musicInstance.start();
    }

    public void StopAmbientMusic()
    {
        if (!CheckCurrentMusicEvent(AmbientMusicEvent))
            return;

        musicInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

        if (state == FMOD.Studio.PLAYBACK_STATE.PLAYING)
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void StartGameOverMusic()
    {
        SetMusicInstance(GameOver);
        musicInstance.start();
    }

    public void StopGameOverMusic()
    {
        if (!CheckCurrentMusicEvent(GameOver))
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
    public delegate void TimelineCallback(string tag);
    private List<TimelineCallback> timelineCallbacks = new List<TimelineCallback>();

    public void AddBeatCallback(BeatCallback cb)
    {
        beatCallbacks.Add(cb);
    }

    public void RemoveBeatCallback(BeatCallback cb)
    {
        if (beatCallbacks.Contains(cb))
            beatCallbacks.Remove(cb);
    }

    private void TriggerBeatCallbacks(int bar, int beat)
    {
        beatCallbacks.ForEach(cb => cb?.Invoke(bar, beat));
    }

    public void AddTimelineCallback(TimelineCallback cb)
    {
        timelineCallbacks.Add(cb);
    }

    public void RemoveTimelineCallback(TimelineCallback cb)
    {
        if (timelineCallbacks.Contains(cb))
            timelineCallbacks.Remove(cb);
    }

    private void TriggerTimelineCallback(string tag)
    {
        timelineCallbacks.ForEach(cb => cb?.Invoke(tag));
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
                    var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                    SoundManager.Instance.TriggerTimelineCallback(parameter.name);
                }
                break;
        }
        return FMOD.RESULT.OK;
    }
}
