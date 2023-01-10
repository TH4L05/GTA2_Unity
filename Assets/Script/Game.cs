using ProjectGTA2_Unity.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;
    public Player player;

    public Color[] carColors;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
    }
}
