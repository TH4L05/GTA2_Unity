/// <author>Thoams Krahl</author>

using ProjectGTA2_Unity;
using UnityEngine;

namespace ProjectGTA2_Unity.Collectables
{
    public class CollectableSpawner : MonoBehaviour
    {
        #region SerializedFields

        [Header("Settings")]
        [SerializeField] protected bool isEnabled = true;
        [SerializeField] protected GameObject collectableObject;
        [SerializeField] protected Transform collectablesParentTransform;

        [Header("Options")]
        [SerializeField] protected bool canRespawn;
        [Range(1, 100), SerializeField, Tooltip("RespawnInterval in s")] private int respawnInterval = 5;

        #endregion

        #region UnityFunctions

        #region PrivateFields

        protected GameObject spawnedCollectable;
        protected Collectable collectable;
        protected bool collected;
        protected float respawnTimer;

        #endregion

        private void Start()
        {
            StartSetup();
        }

        private void Update()
        {
            if (!collected) return;
            if (!canRespawn) return;
            Respawn();
        }

        #endregion

        protected void StartSetup()
        {
            if (collectableObject == null)
            {
                Debug.LogError("Collectable Prefab is Missing -> Spawner get's Disabled");
                gameObject.SetActive(false);
                return;
            }

            SpawnPickupObject();
        }

        public void SpawnPickupObject()
        {
            if (collectableObject == null) return;
            spawnedCollectable = Instantiate(collectableObject, transform.position, Quaternion.Euler(90f, 180f, 0f), collectablesParentTransform);
            collectable = spawnedCollectable.GetComponent<Collectable>();
            collectable.Collected += Collected;
            collected = false;
        }

        protected void Respawn()
        {
            respawnTimer += Time.deltaTime;

            if (respawnTimer >= respawnInterval)
            {
                respawnTimer = 0;
                spawnedCollectable.SetActive(true);
                collected = false;
            }
        }

        protected void Collected()
        {
            if (!canRespawn)
            {
                collectable.Collected -= Collected;
                Destroy(spawnedCollectable);
                collectable = null;
                gameObject.SetActive(false);
                return;
            }

            spawnedCollectable.SetActive(false);
            collected = true;
        }
    }
}

