/// <author>Thoams Krahl</author>

using UnityEngine;

namespace ProjectGTA2_Unity.Tiles
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

    public enum RoadDirection
    {
        Invalid = -1,
        None,
        Up,
        Down,
        Left,
        Right,
    }

    public class Tile : MonoBehaviour
    {
        [SerializeField] private TileTypeMain tileTypeMain = TileTypeMain.Invalid;
        [SerializeField] private TileTypeSecond tileTypeSecond = TileTypeSecond.Invalid;
        [SerializeField] private SurfaceType surfaceType = SurfaceType.Invalid;
        [SerializeField] private RoadDirection[] roadDirections;

        public TileTypeMain GetTileTypeA()
        {
            return tileTypeMain;
        }

        public TileTypeSecond GetTileTypeB()
        {
            return tileTypeSecond;
        }

        public SurfaceType GetSurfaceType()
        {
            return surfaceType;
        }

        public RoadDirection[] GetRoadDirections()
        {
            return roadDirections;
        }
    }
}

