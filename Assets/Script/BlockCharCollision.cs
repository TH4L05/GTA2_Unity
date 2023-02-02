using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCharCollision : MonoBehaviour
{
    public Collider coll;
    public Collider collBlock;

    void Start()
    {
        Physics.IgnoreCollision(coll, collBlock, true);
    }
}
