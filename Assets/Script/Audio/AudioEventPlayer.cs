/// <author>Thoams Krahl</author>

using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace ProjectGTA2_Unity.Audio
{
    public class AudioEventPlayer
    {
        public static void PlayAudioEventOneShot(EventReference eventPath)
        {
            if (eventPath.IsNull) return;
            RuntimeManager.PlayOneShot(eventPath);
            Debug.Log("PlayAudioEvent");
        }

        public static void PlayAudioEventOneShotAttached(EventReference eventPath, GameObject gameObject)
        {
            if (eventPath.IsNull) return;
            if (gameObject == null) return;
            RuntimeManager.PlayOneShotAttached(eventPath, gameObject);
            Debug.Log("PlayAudioEvent");
        }

        public static void PlayAudioEvent(EventReference eventPath)
        {
            if (eventPath.IsNull) return;
            EventInstance i = new EventInstance();

            i = RuntimeManager.CreateInstance(eventPath);
            i.start();
            i.release();
            Debug.Log("PlayAudioEvent");
        }

        public static void PlayAudioEventAttached(EventReference eventPath, GameObject gameObject)
        {
            if (eventPath.IsNull) return;
            if (gameObject == null) return;

            EventInstance i = new EventInstance();
            i = RuntimeManager.CreateInstance(eventPath);
            i.start();
            i.release();
            RuntimeManager.AttachInstanceToGameObject(i, gameObject.transform);
            Debug.Log("PlayAudioEvent");
        }

        public static EventInstance CreateEvent(EventReference eventPath)
        {
            EventInstance i = new EventInstance();

            if (eventPath.IsNull)
            {
                return i;
            }

            i = RuntimeManager.CreateInstance(eventPath);
            i.start();
            return i;

        }

        public static EventInstance Create3DEvent(EventReference eventPath, Transform transform)
        {
            EventInstance i = new EventInstance();

            if (eventPath.IsNull)
            {
                return i;
            }

            i = RuntimeManager.CreateInstance(eventPath);
            i.start();

            EventDescription description = new EventDescription();
            bool is3D = false;

            i.getDescription(out description);
            description.is3D(out is3D);

            if (is3D)
            {
                i.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            }

            RuntimeManager.AttachInstanceToGameObject(i, transform);
            return i;
        }

        public static void StopEvent(EventInstance eventInstance, bool fadeOut)
        {
            if (!eventInstance.isValid()) return;

            if (fadeOut)
            {
                eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            else
            {
                eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
        }

        public static void ReleaseEvent(EventInstance eventInstance)
        {
            if (!eventInstance.isValid()) return;
            eventInstance.release();
        }

        public static void RestartEvent(EventInstance eventInstance)
        {
            PLAYBACK_STATE state;
            eventInstance.getPlaybackState(out state);
            if (state == PLAYBACK_STATE.PLAYING) return;
            eventInstance.start();

        }

        public static bool EventIsPlaying(EventInstance eventInstance)
        {
            PLAYBACK_STATE state;
            eventInstance.getPlaybackState(out state);

            if (state == PLAYBACK_STATE.PLAYING)
            {
                return true;
            }
            return false;
        }
    }

}
