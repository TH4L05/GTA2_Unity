/// <author>Thoams Krahl</author>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Tiles;
using ProjectGTA2_Unity.Characters;
using ProjectGTA2_Unity.Cars;
using ProjectGTA2_Unity.Weapons;
using UnityEngine.Pool;

namespace ProjectGTA2_Unity
{
    public class UnitSpawner : MonoBehaviour
    {
        #region SerializedFields

        [Header("Base")]
        [SerializeField] private Transform PlayersParentTransform;
        [SerializeField] private Transform NpcParentTransform;
        [SerializeField] private Transform CarsParentTransform;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float maxUnitDistanceToPlayer = 20f;
        [SerializeField] private Vector3 checkSize = new Vector3(15f, 1f, 10f);
        [SerializeField] private Vector2 spawnPositionOffsetMinMax = new Vector2(-0.25f, 0.25f);

        [Space(2f), Header("Player")]
        [SerializeField] private List<HumanPlayer> humanPlayers = new List<HumanPlayer>();
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform[] playerSpawns;
        [SerializeField] private float playerRespawnTime = 2f;

        [Space(2f), Header("NPC")]
        [SerializeField] private bool NpcSpawnIsActive;
        [SerializeField] private CharacterNonPlayable ncpPrefab;
        [SerializeField, Range(1, 100)] private int maxActiveNpc = 25;

        [Space(2f), Header("Pooling")]
        [SerializeField, Range(1, 1000)] private int npcPoolSize = 100;
        //[SerializeField, Range(1, 1000)] private int carPoolSize = 100;

        [Space(2f), Header("Cars")]
        [SerializeField] private bool CarSpawnIsActive;
        [SerializeField] private GameObject[] carPrefabs;
        [SerializeField] private int maxActiveCars = 10;

        #endregion

        #region PrivateFields

        private ObjectPool<CharacterNonPlayable> nonPlayableCharactersPool;
        private List<CharacterNonPlayable> spawnedNpcs = new List<CharacterNonPlayable>();
        private List<CharacterNonPlayable> spawnedNpcsOutOfRange = new List<CharacterNonPlayable>();
        private List<GameObject> spawnedCars = new List<GameObject>();
        private Player player;
 
        Transform target
        {
            get 
            { 
                if (PlayerCamera.targetObj != null) return PlayerCamera.targetObj.transform;              
                return null;                                
            }
        }

        #endregion

        #region UnityFunctions

        private void Awake()
        {
            NpcSpawnIsActive = false;
            Player.OnDeath += RespawnPlayer;
            Character.CharacterisDead += CharacterDied;
            Weapon.WeaponAttacked += AWeaponWasFired;
        }

        private void Start()
        {
            NpcPoolSetup();
            Initialize();
        }

        private void LateUpdate()
        {
            CheckSpawnedNpcToPlayerDistance();
            if (spawnedNpcs.Count < maxActiveNpc)
            {
               SpawnNpc();
            }
           
            /*CheckSpawnedCarToPlayerDistance();
            if (spawnedCars.Count < maxActiveCars)
            {
                SpawnCar();
            }*/    
        }

        private void OnDestroy()
        {
            Player.OnDeath -= RespawnPlayer;
            Character.CharacterisDead -= CharacterDied;
            Weapon.WeaponAttacked -= AWeaponWasFired;
        }

        #endregion;

        #region ObjectPools

        private void NpcPoolSetup()
        {
            Debug.Log("CreatePool");
            nonPlayableCharactersPool = new ObjectPool<CharacterNonPlayable>(CreateNpc, GetNpcFromPool, ReturnNpcToPool, OnNpcDestroy, true, npcPoolSize, npcPoolSize*2);
        }

        private CharacterNonPlayable CreateNpc()
        {
            CharacterNonPlayable newNpc = Instantiate(ncpPrefab, NpcParentTransform);
            newNpc.SetPool(nonPlayableCharactersPool);
            newNpc.gameObject.SetActive(false);
            return newNpc;
        }

        private void GetNpcFromPool(CharacterNonPlayable npc)
        {
            npc.gameObject.SetActive(true);
        }
        private void ReturnNpcToPool(CharacterNonPlayable npc)
        {
            npc.gameObject.SetActive(false);
        }

        private void OnNpcDestroy(CharacterNonPlayable npc)
        {
            Destroy(npc.gameObject);
        }

        #endregion

        #region Setup

        private void Initialize()
        {
            if (playerPrefab == null || playerSpawns.Length == 0)
            {
                Debug.LogError("Missing Player Prefab or PlayerSpawns !!!");
                UnityEditor.EditorApplication.isPlaying = false;
            }
        
            HumanPlayer newPlayer = new HumanPlayer("Player" + humanPlayers.Count.ToString("00"), 5, 999999);
            humanPlayers.Add(newPlayer);
            StartCoroutine(SpawnThePlayer(0.25f,humanPlayers.Count -1));        
        }

        #endregion

        #region Player

        public void SpawnPlayer(int index)
        {
            Vector3 spawnPosition = GetSpawnPosition();
            GameObject newPlayerObj = Instantiate(playerPrefab, spawnPosition, Quaternion.identity, PlayersParentTransform);
            newPlayerObj.name = humanPlayers[index].GetName();

            Player player = newPlayerObj.GetComponent<Player>();
            Game.Instance.player = player;
            humanPlayers[index].SetPlayer(player);

            NpcSpawnIsActive = true;
        }

        private Vector3 GetSpawnPosition()
        {
            int spawnPositionIndex = Util.RandomIntNumber(0, playerSpawns.Length);
            return playerSpawns[spawnPositionIndex].position;
        }

        public void RespawnPlayer(DamageType type, string playerName)
        {

            int index = 0;
            foreach (var hum in humanPlayers)
            {
                if (hum.GetName() == playerName)
                {
                    hum.DecreaseLife(1);
                    var player = hum.GetPlayer();
                    Destroy(player.gameObject, playerRespawnTime - 0.1f);
                    hum.SetPlayer(null);
                    Game.Instance.player = null;
                    StartCoroutine(SpawnThePlayer(playerRespawnTime, index));
                }
                index++;
            }
        }

        IEnumerator SpawnThePlayer(float time, int index)
        {
            yield return new WaitForSeconds(time);
            SpawnPlayer(index);
        }

        #endregion

        #region NpcSpawn

        private void SpawnNpc()
        {
            if (!NpcSpawnIsActive) return;
            if (ncpPrefab == null) return;

            List<Tile> tiles = GetSpawnableTiles(TileTypeSecond.Pavement);
            if (tiles.Count == 0)
            {
                Debug.LogError("Cant Spawn Npc -> no spawnable Tiles found !!");
                return;
            }

            int randomIndex = Util.RandomIntNumber(0, tiles.Count);
            Vector3 spawnPosition = tiles[randomIndex].transform.position;
            spawnPosition = RandomSpawnPositionWithOffset(spawnPosition, spawnPositionOffsetMinMax);

            var npc = nonPlayableCharactersPool.Get();
            npc.transform.position = spawnPosition;
            spawnedNpcs.Add(npc);
        }

        private void CheckSpawnedNpcToPlayerDistance()
        {
            if (!NpcSpawnIsActive) return;
         
            foreach (var npc in spawnedNpcs)
            {
                Vector3 camPos =Camera.main.transform.position;
                Vector3 npcPos = npc.transform.position;           
                float distance = Util.GetDistance(npcPos, camPos);

                if (distance > maxUnitDistanceToPlayer)
                {
                    spawnedNpcsOutOfRange.Add(npc);                 
                }
            }
            RemoveNpcOutOfRange();
        }

        private void RemoveNpcOutOfRange()
        {
            if (spawnedNpcsOutOfRange.Count == 0) return;
            
            foreach (var npc in spawnedNpcsOutOfRange)
            {
                spawnedNpcs.Remove(npc);
                nonPlayableCharactersPool.Release(npc);
            }          
            spawnedNpcsOutOfRange.Clear();
        }

        private void CharacterDied(string name, string killer)
        {
            if (name.Contains("Player")) return;
  
            if (killer.Contains("Player"))
            {
                Debug.Log("PlayerKilledCharacter");
                foreach (var player in humanPlayers)
                {
                    if (player.GetName() == killer)
                    {
                        Debug.Log("AddMoney");
                        player.IncreaseMoney(10);
                    }
                }
            }

            foreach (var npc in spawnedNpcs)
            {
                if (npc.gameObject.name == name)
                {
                    nonPlayableCharactersPool.Release(npc);
                    spawnedNpcs.Remove(npc);
                    return;
                }
            }
        }

        #endregion

        #region CarSpawn

        private void SpawnCar()
        {
            if (!CarSpawnIsActive) return;

            if (carPrefabs.Length == 0)
            {
                Debug.LogError("Cant Spawn Car -> No Prefabs available !!");
                return;
            }

            List<Tile> tiles = GetSpawnableTiles(TileTypeSecond.Road);
            if (tiles.Count == 0)
            {
                Debug.LogError("Cant Spawn Car -> no spawnable Tiles found !!");
                return;
            }

            int randomIndex = Util.RandomIntNumber(0, tiles.Count);
            var rd = tiles[randomIndex].GetRoadDirections();
            Vector3 spawnPosition = tiles[randomIndex].transform.position;

            randomIndex = Util.RandomIntNumber(0, carPrefabs.Length);
            GameObject newCar = Instantiate(carPrefabs[randomIndex], spawnPosition, Quaternion.identity, CarsParentTransform);
            newCar.name = "Car" + Time.time;

            Vector3 rot = Vector3.zero;
            if (rd.Length == 0) return;
            switch (rd[0])
            {
                case RoadDirection.None:
                    rot = Vector3.zero;
                    break;
                case RoadDirection.Up:
                    rot = Vector3.forward;
                    break;

                case RoadDirection.Down:
                    rot = Vector3.back;
                    break;

                case RoadDirection.Left:
                    rot = Vector3.left;
                    break;

                case RoadDirection.Right:
                    rot = Vector3.right;
                    break;
                default:
                    break;
            }
            Vector3 direction = Vector3.RotateTowards(newCar.transform.forward.normalized, rot, 360f, 0f);
            newCar.transform.localRotation = Quaternion.LookRotation(direction);

            Car car  = newCar.GetComponent<Car>();
            car.ChangeActivationAndMovementStatus(true, false);
            spawnedCars.Add(newCar);
        }

        private void CheckSpawnedCarToPlayerDistance()
        {
            if (!CarSpawnIsActive) return;

            List<GameObject> spawnedCarsForDelete = new List<GameObject>();
            foreach (var car in spawnedCars)
            {
                Vector3 camPos = Camera.main.transform.position;
                Vector3 carPos = car.transform.position;

                float distance = Util.GetDistance(carPos, camPos);

                if (distance > maxUnitDistanceToPlayer)
                {
                    spawnedCarsForDelete.Add(car);
                }
            }

            if (spawnedCarsForDelete.Count > 0)
            {
                foreach (var car in spawnedCarsForDelete)
                {
                    spawnedCars.Remove(car);
                    Destroy(car, 0.25f);
                }
            }
        }

        #endregion


        private Vector3 RandomSpawnPositionWithOffset(Vector3 position, Vector2 minMaxOffset)
        {
            float xOffset = Util.RandomFloatNumber(minMaxOffset.x, minMaxOffset.y);
            float zOffset = Util.RandomFloatNumber(minMaxOffset.x, minMaxOffset.y);
            return new Vector3(position.x + xOffset, position.y, position.z + zOffset);
        }

        private List<Tile> GetSpawnableTiles(TileTypeSecond tileTypeSecond)
        {
            List<Tile> tilePositions = new List<Tile>();          
            if (target == null) return tilePositions;

            Collider[] colliders = Physics.OverlapBox(target.position, checkSize, target.rotation, groundLayer);

            foreach (var collider in colliders)
            {
                var tile = collider.GetComponent<Tile>();

                if (tile != null && tile.GetTileTypeA() == TileTypeMain.Floor && tile.GetTileTypeB() == tileTypeSecond)
                {
                    tilePositions.Add(tile);
                }
            }
            
            return tilePositions;
        }

        private void AWeaponWasFired(GameObject initiator, Weapon.AttackTypes attackType)
        {
            if (attackType == Weapon.AttackTypes.Melee) return;
            if (spawnedNpcs.Count == 0) return;

            Vector3 attackPos = initiator.transform.position;

            foreach (var npc in spawnedNpcs)
            {             
                Vector3 npcPos = npc.transform.position;

                float distance = Util.GetDistance(npcPos, attackPos);

                if (distance < 8f)
                {
                    var npcBehaviour = npc.GetComponent<NonPlayableBehaviour>();
                    npcBehaviour.GunWasFiredNearby(initiator, attackType);
                }
            }
        }
    }
}

