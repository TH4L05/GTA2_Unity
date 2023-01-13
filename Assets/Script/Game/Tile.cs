/// <author>Thoams Krahl</author>

using UnityEngine;

namespace ProjectGTA2_Unity
{
    public enum TileType
    {
        Invalid = -1,
        Floor,
        Wall
    }

    public enum SurfaceType
    {
        Invalid = -1,
        Normal,
        Grass,
        Metal,
        Water,
        Wood,
        Electrified,
        ElectrifiedPlatform,
        RoadJunction
    }

    public class Tile : MonoBehaviour
    {
        [SerializeField] private TileType tileType = TileType.Invalid;
        [SerializeField] private SurfaceType surfaceType = SurfaceType.Invalid;

        public TileType GetTileType()
        {
            return tileType;
        }

        public SurfaceType GetSurfaceType()
        {
            return surfaceType;
        }
    }
}

