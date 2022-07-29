using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement {

    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        //Ray lastRay; //Debug.DrawRay(lastRay.origin, lastRay.direction * 400); //* draw the casted ray
        NavMeshAgent navMeshAgent;
        Health health;
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float maxNavPathLength = 40f;

        private void Awake() {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        private void Update() {
            navMeshAgent.enabled = !health.GetIsDead();
            UpdateAnimator();            
        }

        private void UpdateAnimator() { // Movement animation

            Vector3 velocity = navMeshAgent.velocity; //Global velocity, but the animator needs the local one
            Vector3 localVelocity = transform.InverseTransformDirection(velocity); // Transforms a direction from world space to local space
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed); // Set animator blend value to be equal to the desired forward speed (on Z axis)
        }

        private float GetPathLength(NavMeshPath path) {
            float cornerSum = 0;
            if (path.corners.Length < 2) { return cornerSum; }

            for (int i = 0; i < path.corners.Length - 1; i++) {
                cornerSum += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return cornerSum;
        }

        public bool CanMoveTo(Vector3 destination) {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) { return false; }
            if (path.status != NavMeshPathStatus.PathComplete) { return false; } //! Exclude unreachable routes (top building)
            if (GetPathLength(path) > maxNavPathLength) { return false; } //! Exclude routes of more than a certain length
            
            return true;
        }

        public void MoveTo(Vector3 destination,float speedFraction) { // Movement
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;    
        }

        public void StartMoveAction(Vector3 destination, float speedFraction) { // Action movement
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void Cancel() {
            navMeshAgent.isStopped = true;
        }

        public object CaptureState() {
            //return new SerializableVector3(transform.position);

            //! To return more than one object use Dictionary or Struct
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state) {
            // If not sure if it is a Vector3, use state as SerializableVector3, but check if is null
            Dictionary<string, object> data = (Dictionary<string, object>)state; // SerializableVector3

            // Sometimes weird condition with navmeshagent, so disable it and reanable it
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = ((SerializableVector3)data["position"]).ToVector(); 
            transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector(); 
            // transform.position = position.ToVecto3();
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}

//! ANIMATOR COMPONENT
//! Assigns animations to GameObject through an Animator Controller 

//! ANIMATOR CONTROLLER
//! Arrangement of animations and transition (state machine)

//! ANIMATION
//! Specific pieces of motion 

//! BLEND TREE
//! Allows multiple animations to be blended together smoothly

//! VELOCITY
//! Rate of change of position overtime (car velocity is North at 50km/h)
//! NavMeshAgent is thinking in terms of velocity
//! Character locomotion animations are interested only in forward speed (along Z axis)