/// <author>Thoams Krahl</author>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using ProjectGTA2_Unity.Cars;

namespace ProjectGTA2_Unity
{
    public class ZoneInfo : MonoBehaviour
    {
        #region SerializedFields

        [SerializeField] private Text zoneNameTextField;
        [SerializeField] private Text carNameTextField;
        [SerializeField] private PlayableDirector playableDirector;
        [SerializeField] private PlayableAsset showZonePlayable;
        [SerializeField] private PlayableAsset showCarPlayable;

        #endregion

        #region UnityFunctions

        private void Start()
        {
            Car.PlayerEntersCar += ShowCarName;
        }

        private void OnDestroy()
        {
            Car.PlayerEntersCar -= ShowCarName;
        }

        #endregion

        public void SetZoneName(string zoneName)
        {
            zoneName = zoneName.ToUpper();
            if (zoneNameTextField != null) zoneNameTextField.text = zoneName;
        }

        public void SetCarName(string carName)
        {
            carName = carName.ToUpper();
            if (carNameTextField != null) carNameTextField.text = carName;
        }

        public void SetPlayable(PlayableAsset playableAsset)
        {
            playableDirector.playableAsset = playableAsset;
        }

        public void ShowCarName()
        {
            if (playableDirector == null || showCarPlayable == null) return;
            playableDirector.Play(showCarPlayable);
        }
           
        public void ShowCarName(string name)
        {
            name = name.ToUpper();
            SetCarName(name);
            if (playableDirector == null || showCarPlayable == null) return;
            playableDirector.Play(showCarPlayable);
        }
       
        public void ShowZoneName()
        {
            if (playableDirector == null || showZonePlayable == null) return;
            playableDirector.Play(showZonePlayable);
        }
    }
}

