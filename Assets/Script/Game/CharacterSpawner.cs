

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters;

namespace ProjectGTA2_Unity
{
    public class CharacterSpawner : MonoBehaviour
    {
        [Header("Base")]
        [SerializeField] private Transform charactersParentObject;
        [SerializeField] private LayerMask groundLayer;

        [Space(2f), Header("Player")]
        [SerializeField] private List<HumanPlayer> humanPlayers = new List<HumanPlayer>();
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform[] playerSpawns;
        [SerializeField] private float playerRespawnTime = 2f;

        [Space(2f), Header("NPC")]
        [SerializeField] private GameObject[] npcPrefabs;
        [SerializeField] private int maxNpc = 100;
        [SerializeField] private float maDistanceToPlayer = 20f;

        private List<GameObject> spawnedNpcs = new List<GameObject>();
        private Player player;

        public bool NpcSpawnIsActive {get; set;}

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
            CheckNpcToPlayerDistance();
            if (spawnedNpcs.Count < maxNpc)
            {
                SpawnNpc();                
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
                Debug.LogError("Cant Spawn Npc's -> prefabs missing !!");
                return;
            }
            
            Transform target = PlayerCamera.targetObj.transform;
            if (target == null) return;

            Collider[] colliders = Physics.OverlapBox(target.position, new Vector3(15f, 1f, 10f), target.rotation, groundLayer); 
            List<Vector3> tilePositions = new List<Vector3>();

            foreach (var collider in colliders)
            {
                var tile = collider.GetComponent<Tile>();

                if (tile != null && tile.GetTileTypeA() == TileTypeMain.Floor && tile.GetTileTypeB() == TileTypeSecond.Pavement)
                {
                    tilePositions.Add(tile.transform.position);
                }
            }

            if (tilePositions.Count == 0)
            {
                Debug.LogError("Cant Spawn Npc's -> no spawn positions found !!");
                return;
            }

            Vector3 spawnPosition = RandomSpawnPosition(tilePositions);  
            GameObject newNpc = Instantiate(npcPrefabs[0], spawnPosition, Quaternion.identity, charactersParentObject);
            newNpc.name = "NPC" + Time.time;
            spawnedNpcs.Add(newNpc);
        }

        private Vector3 RandomSpawnPosition(List<Vector3> positions)
        {
            int randomIndex = Util.RandomIntNumber(0, positions.Count);
            float xOffset = Util.RandomFloatNumber(-0.25f, 0.25f);
            float zOffset = Util.RandomFloatNumber(-0.25f, 0.25f);
            return new Vector3(positions[randomIndex].x + xOffset, positions[randomIndex].y, positions[randomIndex].z + zOffset);
        }

        private void CheckNpcToPlayerDistance()
        {
            if (!NpcSpawnIsActive) return;

            List<GameObject> npcForDelete = new List<GameObject>();
            
            foreach (var npc in spawnedNpcs)
            {
                Vector3 camPos = PlayerCamera.targetObj.transform.position;
                Vector3 npcPos = npc.transform.position;

                float distance = Util.GetDistance(npcPos, camPos);

                if (distance > maDistanceToPlayer)
                {
                    npcForDelete.Add(npc); 
                }
            }

            if(npcForDelete.Count > 0)
            {
                foreach (var npc in npcForDelete)
                {
                    spawnedNpcs.Remove(npc);
                    Destroy(npc, 0.5f);
                }
            }            
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
    }
}

