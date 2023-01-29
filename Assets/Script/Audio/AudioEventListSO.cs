/// <author>Thoams Krahl</author>

using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity.Audio
{
    [CreateAssetMenu(fileName = "NewAudioEventList", menuName = "Data/AudioEventList")]
    public class AudioEventListSO : ScriptableObject
    {
        [SerializeField] private List<AudioEvent> audioEvents = new List<AudioEvent>();
        private GameObject attachedObj;


        public void PlayAudioEventOneShot(string eventName)
        {
            foreach (var audioEvent in audioEvents)
            {
                if (audioEvent.eventName == eventName)
                {
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

            AudioEventPlayer.PlayAudioEvent(audioEvents[eventListIndex].eventPath);
        }

        public void PlayAudioEventOneShotAttached(string eventName, GameObject gameObject)
        {
            if (gameObject == null) return;

            foreach (var audioEvent in audioEvents)
            {
                if (audioEvent.eventName == eventName)
                {
                    AudioEventPlayer.PlayAudioEventOneShotAttached(audioEvent.eventPath, gameObject);
                    return;
                }
            }
            Debug.LogError($"AudioEvent : {eventName} not found");
        }

        public void PlayAudioEventOneShotAttached(string eventName)
        {
            if (attachedObj == null) return;

            foreach (var audioEvent in audioEvents)
            {
                if (audioEvent.eventName == eventName)
                {
                    AudioEventPlayer.PlayAudioEventOneShotAttached(audioEvent.eventPath, attachedObj);
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

            AudioEventPlayer.PlayAudioEventOneShotAttached(audioEvents[eventListIndex].eventPath, gameObject);
        }

        public void PlayAudioEventOneShotAttached(int eventListIndex)
        {
            if(attachedObj== null) return;

            if (eventListIndex < 0 || eventListIndex > audioEvents.Count)
            {
                Debug.LogError($"AudioEvent with at Index: {eventListIndex} not found");
                return;
            }

            AudioEventPlayer.PlayAudioEventOneShotAttached(audioEvents[eventListIndex].eventPath, attachedObj);
        }

        public void SetAttachedGameObject(GameObject gameObject)
        {
            attachedObj = gameObject;
        }
    }
}

