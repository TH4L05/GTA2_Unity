

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters;

namespace ProjectGTA2_Unity
{
    public class CharacterSpawner : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private Transform[] playerSpawns;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private List<HumanPlayer> humanPlayers = new List<HumanPlayer>();
        [SerializeField] private float playerRespawnTime = 2f;
        [SerializeField] private Transform charactersRootObject;

        [Header("NPC")]
        [SerializeField] private int maxNpc = 100;
        [SerializeField] private float maDistanceToPlayer = 20f;

        [SerializeField] private GameObject[] npcPrefabs;
        public List<GameObject> currentSpawnedNpc = new List<GameObject>();

        [SerializeField] private LayerMask groundLayer;
        public List<GameObject> npcForDelete = new List<GameObject>();
        private Player player;

        private void Awake()
        {
            Player.OnDeath += RespawnPlayer;
        }

        private void Start()
        {
            Initialize();          
        }

        private void LateUpdate()
        {
            CheckNpcToPlayerDistance();
            if (currentSpawnedNpc.Count < maxNpc)
            {
                SpawnNpc();                
            }
        }

        private void OnDestroy()
        {
            Player.OnDeath -= RespawnPlayer;
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

        public void SpawnPlayer(int index)
        {
            for (int i = 0; i < humanPlayers.Count; i++)
            {
                Vector3 spawnPosition = GetSpawnPosition();
                GameObject newPlayerObj = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

                Player player = newPlayerObj.GetComponent<Player>();
                humanPlayers[i].SetPlayer(player);

                string name = "Player" + i.ToString("00");
                humanPlayers[index].SetName(name);
                newPlayerObj.name = name;
                Game.Instance.player = player;

                if (charactersRootObject == null) return;
                newPlayerObj.transform.parent = charactersRootObject;
            }
            player = humanPlayers[0].GetPlayer();
        }

        private Vector3 GetSpawnPosition()
        {
            int spawnPositionIndex = Util.RandomIntNumber(0, playerSpawns.Length);
            Vector3 spawnPosition = playerSpawns[spawnPositionIndex].position;
            return spawnPosition;
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

                    Destroy(hum.GetPlayer().gameObject);
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

        private void SpawnNpc()
        {
            if(npcPrefabs.Length == 0) return;
            if (Game.Instance.player == null) return;

            var player = Game.Instance.player;
            var camera = Game.Instance.mainCamera;

            Collider[] colliders = Physics.OverlapBox(player.transform.position, new Vector3(15f, 1f, 10f), player.transform.rotation, groundLayer); 

            List<Tile> tiles = new List<Tile>();

            foreach (var collider in colliders)
            {
                var tile = collider.GetComponent<Tile>();

                if (tile != null && tile.GetTileTypeA() == TileTypeMain.Floor && tile.GetTileTypeB() == TileTypeSecond.Pavement)
                {
                    tiles.Add(tile);
                }
            }

            int r = Util.RandomIntNumber(0, tiles.Count +1);

            var newNpc = Instantiate(npcPrefabs[0], tiles[r].transform.position, Quaternion.identity);
            newNpc.transform.parent = charactersRootObject;
            currentSpawnedNpc.Add(newNpc);
        }

        private void CheckNpcToPlayerDistance()
        {
            if (player == null) return;

            //List<GameObject> npcForDelete = new List<GameObject>();
            npcForDelete.Clear();

            foreach (var npc in currentSpawnedNpc)
            {
                float distance = Util.GetDistance(npc.transform.position, player.transform.position);

                if (distance > maDistanceToPlayer)
                {
                    npcForDelete.Add(npc); 
                }
            }

            if(npcForDelete.Count > 0)
            {
                foreach (var npc in npcForDelete)
                {
                    currentSpawnedNpc.Remove(npc);
                    Destroy(npc, 0.5f);
                }
            }
            
        }
    }
}

