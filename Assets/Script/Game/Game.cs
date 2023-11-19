/// <author>Thoams Krahl</author>

using UnityEngine;
using ProjectGTA2_Unity.Tiles;
using ProjectGTA2_Unity.Characters;
using ProjectGTA2_Unity.UI;
using ProjectGTA2_Unity.Weapons;
using UnityEngine.InputSystem;

namespace ProjectGTA2_Unity
{
    public class Game : MonoBehaviour
    {
        public static Game Instance;
        public Player player;
        public Hud hud;
        public UnitSpawner characterSpawner;
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
        }

        private void Update()
        {
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                player.TakeDamage(999f, DamageType.Normal, "");
            }

            if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                player.IncreaseMoney(100);
            }

            if (Keyboard.current.f3Key.wasPressedThisFrame)
            {
                player.TakeDamage(1f, DamageType.Fire, "");
            }

            if (Keyboard.current.f4Key.wasPressedThisFrame)
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
    }  
}

