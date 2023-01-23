/// <author>Thoams Krahl</author>

using UnityEngine;

namespace ProjectGTA2_Unity
{
    public enum TileTypeMain
    {
        Invalid = -1,
        Floor,
        Wall
    }

    public enum TileTypeSecond
    {
        Invalid = -1,
        None = 0,
        Road,
        Pavement,
        Air,
        RoadJunction,
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
        [SerializeField] private TileTypeMain tileTypeA = TileTypeMain.Invalid;
        [SerializeField] private TileTypeSecond tileTypeB = TileTypeSecond.Invalid;
        [SerializeField] private SurfaceType surfaceType = SurfaceType.Invalid;

        public TileTypeMain GetTileTypeA()
        {
            return tileTypeA;
        }

        public TileTypeSecond GetTileTypeB()
        {
            return tileTypeB;
        }

        public SurfaceType GetSurfaceType()
        {
            return surfaceType;
        }
    }
}

