/// <author>Thoams Krahl</author>

using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

namespace ProjectGTA2_Unity.Audio
{
    [System.Serializable]
    internal class AudioEvent
    {
        public string eventName = "";
        public EventReference eventPath;
    }

    public class AudioEventList : MonoBehaviour
    {
        [SerializeField] private List<AudioEvent> audioEvents = new List<AudioEvent>();
        [SerializeField] private bool showDebugMessages = true;
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
                    AudioEventPlayer.PlayAudioEvent(audioEvent.eventPath);
                    return;
                }
            }
            Debug.LogError($"AudioEvent : {eventName} not found");
        }

        public void PlayAudioEventOneShot(int eventListIndex)
        {
            if (eventListIndex < 0 || eventListIndex > audioEvents.Count)
            {
                Debug.LogError($"AudioEvent with at Index: {eventListIndex} not found");
                return;
            }

            if (showDebugMessages) Debug.Log($"<color=#FFB12B>Play Audio {audioEvents[eventListIndex].eventName}</color>");
            AudioEventPlayer.PlayAudioEvent(audioEvents[eventListIndex].eventPath);
        }

        public void PlayAudioEventOneShotAttached(string eventName, GameObject gameObject)
        {
            if (gameObject == null) return;

            foreach (var audioEvent in audioEvents)
            {
                if (audioEvent.eventName == eventName)
                {
                    if (showDebugMessages) Debug.Log($"<color=#FFB12B>Play Audio {audioEvent.eventName}</color>");
                    AudioEventPlayer.PlayAudioEventOneShotAttached(audioEvent.eventPath, gameObject);
                    return;
                }
            }
            Debug.LogError($"AudioEvent : {eventName} not found");
        }

        public void PlayAudioEventOneShotAttached(int eventListIndex, GameObject gameObject)
        {
            if (gameObject == null) return;

            if (eventListIndex < 0 || eventListIndex > audioEvents.Count)
            {
                Debug.LogError($"AudioEvent with at Index: {eventListIndex} not found");
                return;
            }

            if (showDebugMessages) Debug.Log($"<color=#FFB12B>Play Audio {audioEvents[eventListIndex].eventName}</color>");
            AudioEventPlayer.PlayAudioEventOneShotAttached(audioEvents[eventListIndex].eventPath, gameObject);
        }

        public void PlayAudioEvent(string eventName)
        {
            foreach (var audioEvent in audioEvents)
            {
                if (audioEvent.eventName == eventName)
                {
                    AudioEventPlayer.PlayAudioEvent(audioEvent.eventPath);
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
            AudioEventPlayer.PlayAudioEvent(audioEvents[eventListIndex].eventPath);
        }

        public void PlayAudioEventAttached(string eventName, GameObject gameObject)
        {
            foreach (var audioEvent in audioEvents)
            {
                if (audioEvent.eventName == eventName)
                {
                    AudioEventPlayer.PlayAudioEventAttached(audioEvent.eventPath, gameObject);
                    return;
                }
            }
            if (showDebugMessages) Debug.LogError($"AudioEvent : {eventName} not found");
        }

        public void CreateEvent(string eventName)
        {
            if (eventInstances.ContainsKey(eventName))
            {
                //RestartEvent(eventName);
                return;
            }
          

            foreach (var audioEvent in audioEvents)
            {
                if (audioEvent.eventName == eventName)
                {
                    EventInstance i = AudioEventPlayer.CreateEvent(audioEvent.eventPath);
                    if (i.isValid())
                    {
                        eventInstances.Add(eventName, i);
                    }
                    return;
                }
            }           
        }

        public void Create3DEvent(string eventName, Transform transform)
        {
            if (eventInstances.ContainsKey(eventName))
            {
                //RestartEvent(eventName);
                return;
            }

            foreach (var audioEvent in audioEvents)
            {
                if (audioEvent.eventName == eventName)
                {
                    EventInstance i = AudioEventPlayer.Create3DEvent(audioEvent.eventPath, transform);
                    if (i.isValid())
                    {
                        eventInstances.Add(eventName, i);
                    }
                    return;
                }
            }
        }

        public void StopEvent(string eventName, bool fadeOut)
        {
            if (eventInstances.ContainsKey(eventName))
            {
                //if (showDebugMessages) Debug.Log($"<color=#FFB12B>Stop Audio {eventName}</color>");
                AudioEventPlayer.StopEvent(eventInstances[eventName],fadeOut);
            }
        }

        public void RestartEvent(string eventName)
        {
            if (eventInstances.ContainsKey(eventName))
            {
                //if (showDebugMessages) Debug.Log($"<color=#FFB12B>Restart Audio {eventName}</color>");
                AudioEventPlayer.RestartEvent(eventInstances[eventName]);
            }
        }

        public bool EventIsPlaying(string eventName)
        {
            if (eventInstances.ContainsKey(eventName))
            {
                return AudioEventPlayer.EventIsPlaying(eventInstances[eventName]);
            }
            return false;
        }

        public void RemoveEvent(string eventName)
        {
            if (eventInstances.ContainsKey(eventName))
            {
                StopEvent(eventName, false);
                AudioEventPlayer.ReleaseEvent(eventInstances[eventName]);
                eventInstances.Remove(eventName);
                if (showDebugMessages) Debug.Log($"<color=#FFB12B>AudioEvent {eventName} removed</color>");
            }
        }

        public void RemoveAllEvents()
        {
            foreach (var audioEvent in eventInstances)
            {
                AudioEventPlayer.StopEvent(audioEvent.Value, false);
                AudioEventPlayer.ReleaseEvent(audioEvent.Value);
                if (showDebugMessages) Debug.Log($"<color=#FFB12B>AudioEvent {audioEvent.Key} removed</color>");
            }
            eventInstances.Clear();
        }
    }
}


