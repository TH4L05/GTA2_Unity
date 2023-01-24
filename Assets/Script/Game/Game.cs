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
        public CharacterSpawner characterSpawner;
        public Camera mainCamera;

        public Color[] carColors;

        #region UnityFunctions

        private void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            Initialize();                
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


        #endregion

        public Color GetRandomCarColor()
        {
            int rnd = Util.RandomIntNumber(0, carColors.Length);
            return carColors[rnd];
        }

        private void Initialize()
        {     
        }
    }  
}

