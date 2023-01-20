using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters.AI;

namespace ProjectGTA2_Unity
{
    public class RunningState : BaseState
    {
        private NonPlayerStateMachine sm;

        public RunningState(string name, NonPlayerStateMachine stateMachine) : base(name, stateMachine)
        {
            sm = stateMachine;
        }
    }
}

