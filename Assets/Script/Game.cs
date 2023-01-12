/// <author>Thoams Krahl</author>

using ProjectGTA2_Unity.Characters;
using UnityEngine;

namespace ProjectGTA2_Unity
{
    public class Game : MonoBehaviour
    {
        public static Game Instance;
        public Player player;

        public Color[] carColors;

        private void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            PlayerCamera.SetCameraTarget(player.transform);
        }

        public Color GetRandomCarColor()
        {
            int rnd = UnityEngine.Random.Range(0, carColors.Length);
            return carColors[rnd];
        }
    }
}

