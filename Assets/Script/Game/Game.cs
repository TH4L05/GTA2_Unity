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
        public GameObject playerPrefab;
        public Color[] carColors;
        public Transform[] playerSpawns;
        public string[] infoTexts;
        public List<HumanPlayer> humanPlayers = new List<HumanPlayer>();
        private int moneyMultiplier = 0;

        #region UnityFunctions

        private void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 60;
            Character.CharacterisDead += CharacterKilled;
        }

        private void Start()
        {
            Initialize();
            Player.OnDeath += RespawnPlayer;
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                player.TakeDamage(999f, DamageType.Normal, "");
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                player.IncreaseMoney(100);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                player.TakeDamage(999f, DamageType.Fire, "");
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                player.TakeDamage(4f, DamageType.Normal, "");
            }
        }

        private void OnDestroy()
        {
            Player.OnDeath -= RespawnPlayer;
            Character.CharacterisDead -= CharacterKilled;
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

            if (playerPrefab == null || playerSpawns.Length == 0)
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
            int index = RandomIntNumber(0, playerSpawns.Length);
            Vector3 spawnPosition = playerSpawns[index].position;
            var newPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            player = newPlayer.GetComponent<Player>();
            humanPlayers[0].SetPlayer(player);
        }

        public void RespawnPlayer(DamageType type)
        {
            Destroy(player.gameObject, 1.95f);
            player = null;
           
            StartCoroutine(SpawnThePlayer(2f));
        }    

        public void CharacterKilled(string tag, string killer)
        {
            if (tag == "Player") return;

            if (tag == "NPC" && killer == "Player")
            {
                Debug.Log("Increase Player Money for kill a NPC");
                player.IncreaseMoney(10);
            }
            else if (tag == "Car")
            {
                Debug.Log("Increase Player Money for destroy a Car");
                player.IncreaseMoney(10);
            }
        }

    }  
}

