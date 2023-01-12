/// <author>Thoams Krahl</author>

using UnityEngine;

public class LimitObjectLifetime : MonoBehaviour
{

    [SerializeField] private float timeBeforeDestroy = 1f;

    void Start()
    {
        Destroy(gameObject, timeBeforeDestroy);
    }
}
