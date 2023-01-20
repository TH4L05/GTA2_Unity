

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters;

namespace ProjectGTA2_Unity
{
    public class CharacterSpawner : MonoBehaviour
    {
        [SerializeField] private Transform[] playerSpawns;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private List<HumanPlayer> humanPlayers = new List<HumanPlayer>();
        [SerializeField] private float playerRespawnTime = 2f;
        [SerializeField] private Transform charactersRootObject;

        private void Awake()
        {
            Player.OnDeath += RespawnPlayer;
        }

        private void Start()
        {
            Initialize();          
        }

        private void OnDestroy()
        {
            Player.OnDeath -= RespawnPlayer;
        }

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
    }
}

