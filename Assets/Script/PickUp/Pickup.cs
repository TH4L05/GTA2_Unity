using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectGTA2_Unity
{
    public enum PickupType
    {
        Invalid = -1,
        Health,
        Armor,
        Pistol,         
        Machinegun,         
        Shotgun,         
    }

    public class Pickup : MonoBehaviour
    {
        public static Action<PickupType, int> PickupCollected;

        #region SerializedFields

        [Space(5)]
        [Header("Events")]
        public UnityEvent OnCollection;

        [Header("Base Settings")]

        [SerializeField] private PickupType type = PickupType.Invalid;
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private int amount = 1;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private GameObject pickupObject;

        [Header("Options")]
        [SerializeField] private bool canRespawn;
        [Range(1,100),SerializeField, Tooltip("RespawnInterval in s")] private int RespawnInterval = 5;

        [Space(5)]
        [Header("Dev")]
        [SerializeField] private Color gizmoColor = Color.cyan;

        #endregion

        #region PrivateFields

        private GameObject spawnedPickup;
        private bool collected;
        private float respawnTimer;

        #endregion

        #region UnityFunctions

        private void Start()
        {
            if (type == PickupType.Invalid)
            {
                Debug.LogError("Invalid PickupType -> Object get's Disabled");
                gameObject.SetActive(false);
                return;
            }

            SpawnPickupObject();
        }

        private void Update()
        {
            if (!collected) return;
            if (!canRespawn) return;
            Respawn();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.layer != playerLayer.value - 5) return;
            if (collected) return;

            Debug.Log($"<color=#597FFF>Player Collected {type} Pickup -> {pickupObject.name}</color>");
            OnCollection?.Invoke();
            OnPickupCollection();
            return;   
        }

        #endregion

        public void SpawnPickupObject()
        {
            if (pickupObject == null) return;
            spawnedPickup = Instantiate(pickupObject, transform.position, Quaternion.Euler(90f,180f,0f));
            //newPickup.transform.parent = transform;
        }

        public virtual void OnPickupCollection()
        {
            collected = true;
            PickupCollected?.Invoke(type, amount);
            Destroy(spawnedPickup);
            spawnedPickup = null;

            if (!canRespawn) Destroy(gameObject, 0.1f);                           
        }

        private void Respawn()
        {
            respawnTimer += Time.deltaTime;

            if(respawnTimer >= RespawnInterval)
            {
                respawnTimer= 0;
                collected = false;
                SpawnPickupObject();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(transform.position, transform.localScale);
        }
    }
}

