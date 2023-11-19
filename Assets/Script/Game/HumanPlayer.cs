/// <author>Thoams Krahl</author>

using ProjectGTA2_Unity.Characters;
using UnityEngine;

namespace ProjectGTA2_Unity
{
    [System.Serializable]
    public class HumanPlayer
    {
        [SerializeField] private string name;
        [SerializeField] private int lifes;
        [SerializeField] private int lifesMax;
        [SerializeField] private int money;
        [SerializeField] private int moneyMax;
        [SerializeField] private Player player;

        public HumanPlayer(string name, int lifeMax, int moneyMax)
        {
            this.name = name;
            lifesMax = lifeMax;
            lifes = lifesMax;
            money = 0;
            this.moneyMax = moneyMax;
            player = null;
        }

        public void IncreaseMoney(int amount)
        {
            money += amount;
            player.IncreaseMoney(money);

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

        public string GetName()
        {
            return name;
        }

        public int GetLifes()
        {
            return lifes;
        }

        public int GetMoney()
        {
            return money;
        }

        public Player GetPlayer()
        {
            return player;
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

