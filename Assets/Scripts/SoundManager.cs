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

                        DontDestroyOnLoad(sm);
                    }
                }

                return instance;
            }
        }
    }

    private void OnApplicationQuit()
    {
        shuttingDown = true;
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void OnDestroy()
    {
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

    // SFX
    public string PlayerThruster;
    public string[] EnemyHitEvents, PlanetHitEvents, EnemyExplodeEvents;
    private int currentEnemyHitEvent, currentPlanetHitEvent, currentEnemyExplodeEvent = 0;

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

        if (eventEmitter.Event != PlayerThruster)
            return;
        
        eventEmitter.SetParameter("Power", 0.0f);
    }

    public void PlayEnemyHit(GameObject target)
    {
        currentEnemyHitEvent++;

        if (currentEnemyHitEvent >= EnemyHitEvents.Length)
            currentEnemyHitEvent = 0;

        var eventEmitter = GetEmitter(target);

        if (eventEmitter.IsPlaying())
            eventEmitter.Stop();

        eventEmitter.Event = EnemyHitEvents[currentEnemyHitEvent];

        eventEmitter.Play();
    }

    public void PlayPlanetHit(GameObject target)
    {
        currentPlanetHitEvent++;

        var eventEmitter = GetEmitter(target);

        if (currentPlanetHitEvent >= PlanetHitEvents.Length)
            currentPlanetHitEvent = 0;

        if (eventEmitter.IsPlaying())
            eventEmitter.Stop();

        eventEmitter.Event = PlanetHitEvents[currentPlanetHitEvent];

        eventEmitter.Play();
    }

    public void PlayEnemyExplode(GameObject target)
    {
        currentEnemyExplodeEvent++;

        var eventEmitter = GetEmitter(target);

        if (currentEnemyExplodeEvent >= EnemyExplodeEvents.Length)
            currentEnemyExplodeEvent = 0;

        if (eventEmitter.IsPlaying())
            eventEmitter.Stop();

        eventEmitter.Event = EnemyExplodeEvents[currentEnemyExplodeEvent];

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

    public string MusicEvent;

    public void StartMusic()
    {
        if (musicInstance.isValid())
        {
            FMOD.Studio.PLAYBACK_STATE state;
            musicInstance.getPlaybackState(out state);

            if (state != FMOD.Studio.PLAYBACK_STATE.STOPPED)
                return;
        }

        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(SoundManager.BeatEventCallback);

        musicInstance = FMODUnity.RuntimeManager.CreateInstance(MusicEvent);

        musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        musicInstance.start();
    }

    public void StopMusic()
    {
        if (!musicInstance.isValid())
            return;

        FMOD.Studio.PLAYBACK_STATE state;
        musicInstance.getPlaybackState(out state);

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

    // Beat Tracking

    FMOD.Studio.EventInstance musicInstance;
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
