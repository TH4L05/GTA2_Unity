using ProjectGTA2_Unity.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectGTA2_Unity
{
    [System.Serializable]
    public class HumanPlayer
    {
        [SerializeField] private string name;
        [SerializeField] private int lifes = 1;
        [SerializeField] private int lifesMax = 99;
        [SerializeField] private int money = 0;
        [SerializeField] private int moneyMax = 9999999;
        [SerializeField] private Player player;

        public void IncreaseMoney(int amount)
        {
            money += amount;

            if (money > moneyMax)
            {
                money = moneyMax;
            }
        }

        public void DecreaseMoney(int amount)
        {
            money -= amount;

            if (money < 0)
            {
                money = 0;
            }
        }

        public void IncreaseLife(int amount)
        {
            lifes += amount;

            if (lifes > lifesMax)
            {
                lifes = lifesMax;
            }
        }

        public void DecreaseLife(int amount)
        {
            lifes -= amount;

            if (lifes < 0)
            {
                lifes = 0;
            }
        }

        public int GetLifes()
        {
            return lifes;
        }

        public int GetMoney()
        {
            return money;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void SetPlayer(Player player)
        {
            this.player = player;
        }
    }
}

