/// <author>Thoams Krahl</author>

using UnityEngine;

namespace ProjectGTA2_Unity
{
    public enum TileTypeA
    {
        Invalid = -1,
        Floor,
        Wall
    }

    public enum TileTypeB
    {
        Invalid = -1,
        Road,
        Pavement,
        Air
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
        [SerializeField] private TileTypeA tileTypeA = TileTypeA.Invalid;
        [SerializeField] private TileTypeB tileTypeB = TileTypeB.Invalid;
        [SerializeField] private SurfaceType surfaceType = SurfaceType.Invalid;

        public TileTypeA GetTileTypeA()
        {
            return tileTypeA;
        }

        public TileTypeB GetTileTypeB()
        {
            return tileTypeB;
        }

        public SurfaceType GetSurfaceType()
        {
            return surfaceType;
        }
    }
}

