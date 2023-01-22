

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters.AI;
using UnityEngine.AI;
using Unity.VisualScripting;
using System.Linq;

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
            sm.navAgent.isStopped = false;
            sm.navAgent.stoppingDistance = 0.15f;
            sm.navAgent.speed = sm.charData.Walkspeed;
            sm.navAgent.SetDestination(sm.destination);
            sm.groundTiles.Clear();
            sm.transform.LookAt(sm.destination);
        }

        public override void UpdateState()
        {
            if(IsPathBlocked() || NeedsDestination())
            {
                sm.ChangeState(sm.idleState);
            }            
            Debug.DrawRay(sm.transform.position, sm.destination * 10f, Color.green);
            
            //sm.transform.Translate(sm.transform.forward * Time.deltaTime * sm.charData.Walkspeed);

            
        }

        private bool IsPathBlocked()
        {
            /*Ray ray = new Ray(sm.transform.position, sm.transform.forward);
            var hitSomething = Physics.RaycastAll(ray, 2f, sm.groundLayer);
            return hitSomething.Any();*/
            return false;
        }

        public override void Exit()
        {
            base.Exit();
            sm.navAgent.isStopped = true;
        }

        private bool NeedsDestination()
        {
            if (sm.destination == Vector3.zero) return true;

            /*var distance = Vector3.Distance(sm.transform.position, sm.destination);
            if (distance <= sm._stoppingDistance)
            {
                return true;
            }*/
            if (sm.navAgent.remainingDistance <= sm.navAgent.stoppingDistance)
            {
                sm.ShowMessage("Destination Reached");
                return true;
            }

            return false;
        }
    }
}
