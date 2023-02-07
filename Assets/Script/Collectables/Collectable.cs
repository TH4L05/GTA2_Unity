/// <author>Thoams Krahl</author>

using System;
using UnityEngine;
using UnityEngine.Events;
using ProjectGTA2_Unity.Audio;

namespace ProjectGTA2_Unity.Collectables
{
    public enum CollectableTypes
    {
        Invalid = -1,
        Health,
        Armor,
        Pistol,
        DualPistol,
        Machinegun,         
        Shotgun,  
        RocketLauncher,
        SilentMachinegun,
        ElectroGun,
        Flamethrower,
        Grenade,
        Molotov,
        Invulnerability,
        Electrofingers,
        JailFreecard,
        CopBribe,
        Respect,
        InstantGang,
        KillFrenzy,
    }

    public class Collectable : MonoBehaviour
    {
        public static Action<CollectableTypes, int, float> CollectableGathered;
        public Action Collected;

        #region SerializedFields

        [Space(5)]
        [Header("Events")]
        public UnityEvent OnCollection;

        [Header("Settings")]
        [SerializeField] protected CollectableTypes collectableType = CollectableTypes.Invalid;
        [SerializeField] protected int amount = 1;
        [SerializeField] protected float duration = -1f;
        [SerializeField] protected LayerMask playerLayer;
        [SerializeField] protected AudioEventListSO audioEventList;
 
        #endregion

        #region UnityFunctions

        private void OnTriggerEnter(Collider collider)
        {
            Collect(collider);
        }

        #endregion
             
        protected virtual void Collect(Collider collider)
        {
            if (!collider.CompareTag("Player")) return;
            Debug.Log($"<color=#597FFF>Player collect {collectableType} Collectable </color>");          
            OnCollect();
        }

        public virtual void OnCollect()
        {
            if(audioEventList != null) audioEventList.PlayAudioEventOneShotAttached("Collect" + collectableType, gameObject);
            OnCollection?.Invoke();
            CollectableGathered?.Invoke(collectableType, amount, 0f);
            Collected?.Invoke();
        }
    }
}

