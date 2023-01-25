/// <author>Thoams Krahl</author>

using System;
using UnityEngine;

namespace ProjectGTA2_Unity.Cars
{
    public class CarCollider : MonoBehaviour
    {
        public enum HitDirection { FontLeft, FrontRight, BackLeft, BackRight }
        public Action<HitDirection> onHit;

        [SerializeField] private HitDirection hitDirection;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Tile"))
            {
                var tile = collider.GetComponent<Tile>();

                if (tile != null && tile.GetTileTypeA() == TileTypeMain.Wall)
                {
                    onHit?.Invoke(hitDirection);
                    return;
                }
            }

            var car = collider.GetComponent<Car>();

            if (car)
            {
                onHit?.Invoke(hitDirection);              
            }
        }
    }
}
