/// <author>Thoams Krahl</author>

using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

namespace ProjectGTA2_Unity
{
    [System.Serializable]
    internal class AudioEvent
    {
        public string eventName = "";
        public EventReference eventPath;
    }

    [CreateAssetMenu(fileName = "NewAudioEventList", menuName = "Data/AudioEventList")]
    public class AudioEventList : ScriptableObject
    {
        [SerializeField] private List<AudioEvent> audioEvents = new List<AudioEvent>();
        [SerializeField] private bool showDebugMessages = true;
        private EventInstance instance;
        private Dictionary<string, EventInstance> eventInstances = new Dictionary<string, EventInstance>();

        private void OnDestroy()
        {
            if (eventInstances.Count < 1) return;

            foreach (var eventInstance in eventInstances)
            {
                eventInstance.Value.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                eventInstance.Value.release();
                if(showDebugMessages) Debug.Log($"<color=#FFB12B>Release {eventInstance.Key}</color>");
            }
        }

        public void PlayAudioEventOneShot(string eventName)
        {
            foreach (var audioEvent in audioEvents)
            {
                if (audioEvent.eventName == eventName)
                {
                    if (showDebugMessages) Debug.Log($"<color=#FFB12B>Play Audio {audioEvent.eventName}</color>");
                    RuntimeManager.PlayOneShot(audioEvent.eventPath);
                    return;
                }
            }
            Debug.LogError($"AudioEvent : {eventName} not found");
        }

        public void PlayAudioEventOneShot(int eventListIndex)
        {
            if (eventListIndex < 0 || eventListIndex > audioEvents.Count)
            {
                if (showDebugMessages) Debug.LogError($"AudioEvent with at Index: {eventListIndex} not found");
                return;
            }

            if (showDebugMessages) Debug.Log($"<color=#FFB12B>Play Audio {audioEvents[eventListIndex].eventName}</color>");
            RuntimeManager.PlayOneShot(audioEvents[eventListIndex].eventPath);
        }

        public void PlayAudioEvent(string eventName)
        {
            foreach (var audioEvent in audioEvents)
            {
                if (audioEvent.eventName == eventName)
                {
                    Debug.Log($"<color=#FFB12B>Play Audio {audioEvent.eventName}</color>");
                    instance = RuntimeManager.CreateInstance(audioEvent.eventPath);
                    instance.start();
                    instance.release();
                    return;
                }
            }
            if (showDebugMessages) Debug.LogError($"AudioEvent : {eventName} not found");
        }

        public void PlayAudioEvent(int eventListIndex)
        {
            if (eventListIndex < 0 || eventListIndex > audioEvents.Count)
            {
                Debug.LogError($"AudioEvent with at Index: {eventListIndex} not found");
                return;
            }

            if (showDebugMessages) Debug.Log($"<color=#FFB12B>Play Audio {audioEvents[eventListIndex].eventName}</color>");
            instance = RuntimeManager.CreateInstance(audioEvents[eventListIndex].eventPath);
            instance.start();
            instance.release();
        }

        public void CreateEvent(string eventName)
        {
            if (eventInstances.ContainsKey(eventName))
            {
                RestartEvent(eventName);
                return;
            }

            EventInstance i = new EventInstance();

            foreach (var audioEvent in audioEvents)
            {
                if (audioEvent.eventName == eventName)
                {
                    if (showDebugMessages) Debug.Log($"<color=#FFB12B>Start Audio {audioEvent.eventName}</color>");
                    i = RuntimeManager.CreateInstance(audioEvent.eventPath);
                    i.start();
                    break;
                }
            }
            eventInstances.Add(eventName,i);

        }

        public void StopEvent(string eventName)
        {
            if (eventInstances.ContainsKey(eventName))
            {
                if (showDebugMessages) Debug.Log($"<color=#FFB12B>Stop Audio {eventName}</color>");
                eventInstances[eventName].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            }
        }

        public void RestartEvent(string eventName)
        {
            if (eventInstances.ContainsKey(eventName))
            {
                PLAYBACK_STATE state;
                eventInstances[eventName].getPlaybackState(out state);
                if (state == PLAYBACK_STATE.PLAYING) return;
                if (showDebugMessages) Debug.Log($"<color=#FFB12B>Restart Audio {eventName}</color>");
                eventInstances[eventName].start();
            }
        }


        public bool EventIsPlaying(string eventName)
        {
            if (eventInstances.ContainsKey(eventName))
            {
                PLAYBACK_STATE state;
                eventInstances[eventName].getPlaybackState(out state);

                if (state == PLAYBACK_STATE.PLAYING)
                {
                    return true;
                }
                return false;
            }
            return false;
        }


        public void RemoveEvent(string eventName)
        {
            if (eventInstances.ContainsKey(eventName))
            {
                eventInstances[eventName].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                eventInstances[eventName].release();
                eventInstances.Remove(eventName);
                if (showDebugMessages) Debug.Log($"<color=#FFB12B>AudioEvent {eventName} removed</color>");
            }
        }

        public EventInstance GetInstance(string eventName)
        {
            return eventInstances[eventName];
        }
    }
}


