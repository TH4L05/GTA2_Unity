

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters.AI;
using UnityEngine.AI;
using Unity.VisualScripting;

namespace ProjectGTA2_Unity
{
    public class WalkingState : BaseState
    {
        private NonPlayerStateMachine sm;
        private bool needDestination;

        public WalkingState(string name, NonPlayerStateMachine stateMachine) : base(name, stateMachine)
        {
            sm = stateMachine;          
        }

        public override void Enter()
        {
            sm.ShowMessage("Enter " + name + " State");
            //sm.navAgent.isStopped = false;
            //sm.navAgent.stoppingDistance = 0.15f;
            //sm.navAgent.speed = sm.charData.Walkspeed;
            //sm.navAgent.SetDestination(sm.destination);
            sm.groundTiles.Clear();
        }

        public override void UpdateState()
        {
            if (NeedsDestination())
            {
                sm.ChangeState(sm.idleState);
            }

            sm.transform.LookAt(sm.destination);
            Debug.DrawRay(sm.transform.position, sm.direction * 10f, Color.green);

            sm.transform.Translate(sm.transform.forward * Time.deltaTime * sm.charData.Walkspeed);

            /*if (sm.navAgent.remainingDistance <= sm.navAgent.stoppingDistance)
            {
                sm.ShowMessage("Destination Reached");
                sm.ChangeState(sm.idleState);
            }*/
        }

        public override void Exit()
        {
            base.Exit();
            sm.navAgent.isStopped = true;
        }

        private bool NeedsDestination()
        {
            if (sm.destination == Vector3.zero)
                return true;

            var distance = Vector3.Distance(sm.transform.position, sm.destination);
            if (distance <= sm._stoppingDistance)
            {
                return true;
            }

            return false;
        }
    }
}
