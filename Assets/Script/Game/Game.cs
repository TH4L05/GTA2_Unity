/// <author>Thoams Krahl</author>

using ProjectGTA2_Unity.Characters;
using ProjectGTA2_Unity.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity
{
    public class Game : MonoBehaviour
    {
        public static Game Instance;
        public Player player;
        public Hud hud;
        public GameObject playerObj;

        public Color[] carColors;
        public Transform[] playerSpawns;
        public string[] infoTexts;

        public List<HumanPlayer> humanPlayers = new List<HumanPlayer>();

        #region UnityFunctions

        private void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            Initialize();
            Player.PlayerDied += RespawnPlayer;
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                player.TakeDamage(999f, DamageType.Normal);
            }
        }

        private void OnDestroy()
        {
            Player.PlayerDied -= RespawnPlayer;
        }

        #endregion

        public Color GetRandomCarColor()
        {
            int rnd = UnityEngine.Random.Range(0, carColors.Length);
            return carColors[rnd];
        }

        private void Initialize()
        {
            var human = new HumanPlayer();
            humanPlayers.Add(human);
            human.SetName("Player0");

            if (playerObj == null || playerSpawns.Length == 0)
            {
                Debug.LogError("Missing Player Prefab or PlayerSpawns !!!");
                UnityEditor.EditorApplication.isPlaying = false;
            }
            StartCoroutine(SpawnThePlayer(0.5f));         
        }

        public int RandomIntNumber(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public float RandomFloatNumber(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        IEnumerator SpawnThePlayer(float time)
        {
            yield return new WaitForSeconds(time);
            SpawnPlayer();
        }

        public void SpawnPlayer()
        {         
            int index = RandomIntNumber(0, playerSpawns.Length - 1);
            Vector3 spawnPosition = playerSpawns[index].position;
            var newPlayer = Instantiate(playerObj, spawnPosition, Quaternion.identity);
            player = newPlayer.GetComponent<Player>();
            humanPlayers[0].SetPlayer(player);
        }

        public void RespawnPlayer(DamageType type)
        {
            player = null;
           
            StartCoroutine(SpawnThePlayer(2f));
        }    
    }  
}

