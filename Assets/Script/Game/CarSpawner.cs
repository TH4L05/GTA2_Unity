using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private Transform carsParentObject;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private GameObject[] carPrefabs;
    [SerializeField] private int maxCars = 100;
    [SerializeField] private float maDistanceToPlayer = 20f;

    public bool CarSpawnIsActive { get; set; }
}
