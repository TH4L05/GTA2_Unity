using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters;

namespace ProjectGTA2_Unity
{
    public class NonPlayableBehaviour : MonoBehaviour
    {
        private enum State
        {
            Invalid = -1,
            Idle,
            Walk,
            Run,
            Chase,
            Flee,
        }

        [SerializeField] private CharacterData characterData;
        [SerializeField] private bool canFlee;
        private State currentstate;

        private void Awake()
        {
            currentstate = State.Idle;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            
        }
    }
}

