using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectGTA2_Unity
{
    public enum CollectableType
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
        public static Action<CollectableType, int, float> CollectableGathered;
        public Action Collected;

        #region SerializedFields

        [Space(5)]
        [Header("Events")]
        public UnityEvent OnCollection;

        [Header("Settings")]

        [SerializeField] protected CollectableType type = CollectableType.Invalid;
        [SerializeField] protected int amount = 1;
        [SerializeField] protected float duration = 0f;
        [SerializeField] protected LayerMask playerLayer;

        #endregion

        #region UnityFunctions

        private void OnTriggerEnter(Collider collider)
        {
            Collect(collider);
        }

        #endregion
             
        protected virtual void Collect(Collider collider)
        {
            if (collider.gameObject.layer != playerLayer.value - 5) return;
            Debug.Log($"<color=#597FFF>Player collect {type} Collectable </color>");          
            OnCollect();
        }

        public virtual void OnCollect()
        {
            OnCollection?.Invoke();
            CollectableGathered?.Invoke(type, amount, 0f);
            Collected?.Invoke();
        }
    }
}

