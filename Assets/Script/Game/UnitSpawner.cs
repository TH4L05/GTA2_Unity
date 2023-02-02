

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters;
using ProjectGTA2_Unity.Cars;
using UnityEngine.UIElements;
using System.Collections.Specialized;

namespace ProjectGTA2_Unity
{
    public class UnitSpawner : MonoBehaviour
    {
        [Header("Base")]
        [SerializeField] private Transform charactersParentObject;
        [SerializeField] private Transform carsParentObject;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float maxDistanceToPlayer = 20f;

        [Space(2f), Header("Player")]
        [SerializeField] private List<HumanPlayer> humanPlayers = new List<HumanPlayer>();
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform[] playerSpawns;
        [SerializeField] private float playerRespawnTime = 2f;

        [Space(2f), Header("NPC")]
        [SerializeField] private GameObject[] npcPrefabs;
        [SerializeField] private int maxNpc = 10;

        [Space(2f), Header("Cars")]
        [SerializeField] private GameObject[] carPrefabs;
        [SerializeField] private int maxCars = 10;


        private List<GameObject> spawnedNpcs = new List<GameObject>();
        private List<GameObject> spawnedNpcsForDelete = new List<GameObject>();
        private List<GameObject> spawnedCars = new List<GameObject>();
        private List<GameObject> spawnedCarsForDelete = new List<GameObject>();
        private Player player;

        public bool NpcSpawnIsActive;
        public bool CarSpawnIsActive;

        Transform target => PlayerCamera.targetObj.transform;

        private void Awake()
        {
            NpcSpawnIsActive = false;
            Player.OnDeath += RespawnPlayer;
            Character.CharacterisDead += CharacterDied;
        }

        private void Start()
        {
            Initialize();          
        }

        private void LateUpdate()
        {
            CheckSpawnedNpcToPlayerDistance();
            if (spawnedNpcs.Count < maxNpc)
            {
                SpawnNpc();
            }
           
            CheckSpawnedCarToPlayerDistance();
            if (spawnedCars.Count < maxCars)
            {
                SpawnCar();
            }                 
        }

        private void OnDestroy()
        {
            Player.OnDeath -= RespawnPlayer;
            Character.CharacterisDead -= CharacterDied;
        }

        /*private void OnDrawGizmosSelected()
        {
            if (player == null) return;
            Gizmos.color = new Color(1f, 0f, 0f, 0.45f);
            Gizmos.DrawWireCube(player.transform.position, new Vector3(maDistanceToPlayer, 3f, maDistanceToPlayer));
        }*/

        private void Initialize()
        {
            if (playerPrefab == null || playerSpawns.Length == 0)
            {
                Debug.LogError("Missing Player Prefab or PlayerSpawns !!!");
                UnityEditor.EditorApplication.isPlaying = false;
            }
        
            foreach (var humanPlayer in humanPlayers)
            {
                humanPlayer.Initialize();
            }
         
            StartCoroutine(SpawnThePlayer(0.5f,0));
        }

        #region Player

        public void SpawnPlayer(int index)
        {
            for (int i = 0; i < humanPlayers.Count; i++)
            {
                Vector3 spawnPosition = GetSpawnPosition();
                GameObject newPlayerObj = Instantiate(playerPrefab, spawnPosition, Quaternion.identity, charactersParentObject);

                Player player = newPlayerObj.GetComponent<Player>();
                humanPlayers[i].SetPlayer(player);

                string name = "Player" + i.ToString("00");
                humanPlayers[i].SetName(name);
                newPlayerObj.name = name;               
            }

            player = humanPlayers[0].GetPlayer();
            Game.Instance.player = player;
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
                    var player = hum.GetPlayer();
                    if (player == null) return;

                    Destroy(hum.GetPlayer().gameObject, playerRespawnTime - 0.1f);
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
            
            if (npcPrefabs.Length == 0)
            {
                Debug.LogError("Cant Spawn Npc -> No Prefabs available !!");
                return;
            }

            List<Tile> tiles = GetSpawnableTiles(TileTypeSecond.Pavement);
            if (tiles.Count == 0)
            {
                Debug.LogError("Cant Spawn Npc -> no spawnable Tiles found !!");
                return;
            }

            int randomIndex = Util.RandomIntNumber(0, tiles.Count);
            Vector3 spawnPosition = tiles[randomIndex].transform.position;
            spawnPosition = RandomSpawnPositionWithOffset(spawnPosition, new Vector2(-0.25f, 0.25f));

            randomIndex = Util.RandomIntNumber(0, npcPrefabs.Length);
            GameObject newNpc = Instantiate(npcPrefabs[randomIndex], spawnPosition, Quaternion.identity, charactersParentObject);
            newNpc.name = "NPC" + Time.time;
            spawnedNpcs.Add(newNpc);
        }

        private void CheckSpawnedNpcToPlayerDistance()
        {
            if (!NpcSpawnIsActive) return;
             
            foreach (var npc in spawnedNpcs)
            {
                Vector3 camPos =Camera.main.transform.position;
                Vector3 npcPos = npc.transform.position;

                float distance = Util.GetDistance(npcPos, camPos);

                if (distance > maxDistanceToPlayer)
                {
                    spawnedNpcsForDelete.Add(npc); 
                }
            }

            if(spawnedNpcsForDelete.Count > 0)
            {
                foreach (var npc in spawnedNpcsForDelete)
                {
                    spawnedNpcs.Remove(npc);
                    Destroy(npc, 0.25f);
                }
            }
            spawnedNpcsForDelete.Clear();
        }

        private void CharacterDied(string name, string damageType, string killer)
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
            GameObject newCar = Instantiate(carPrefabs[randomIndex], spawnPosition, Quaternion.identity, carsParentObject);
            newCar.name = "Car" + Time.time;

            Vector3 rot = Vector3.zero;
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

            foreach (var car in spawnedCars)
            {
                Vector3 camPos = Camera.main.transform.position;
                Vector3 npcPos = car.transform.position;

                float distance = Util.GetDistance(npcPos, camPos);

                if (distance > maxDistanceToPlayer)
                {
                    spawnedCarsForDelete.Add(car);
                }
            }

            if (spawnedCarsForDelete.Count > 0)
            {
                foreach (var car in spawnedCarsForDelete)
                {
                    spawnedNpcs.Remove(car);
                    Destroy(car, 0.25f);
                }
            }
            spawnedCarsForDelete.Clear();
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

            Collider[] colliders = Physics.OverlapBox(target.position, new Vector3(15f, 1f, 10f), target.rotation, groundLayer);

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
    }
}

