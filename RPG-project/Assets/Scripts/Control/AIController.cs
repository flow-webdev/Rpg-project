using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using UnityEngine.AI;
using RPG.Utils;

namespace RPG.Control {

    public class AIController : MonoBehaviour {

        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPathScript patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 3f;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.25f;

        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;

        LazyValue<Vector3> guardPosition; // Lazy Value wrapper/container makes sure initialization is called just before first use

        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int curentWaypointIndex = 0;

        private void Awake() {
            fighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();

            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition() {
            return transform.position;
        }

        private void Start() {
            guardPosition.ForceInit();
        }

        private void Update() {
            if (health.GetIsDead()) return;

            if (InAttackDistanceOfPlayer() && fighter.CanAttack(player)) {                
                AttackBehaviour();

            } else if (timeSinceLastSawPlayer < suspicionTime) {
                SuspicionBehaviour();

            } else {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers() {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour() {
            Vector3 nextPosition = guardPosition.value;

            if (patrolPath != null) {                
                if (AtWaypoint()) {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if (timeSinceArrivedAtWaypoint > waypointDwellTime) {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        private bool AtWaypoint() { //! Patrol        
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint() { //! Patrol 
            curentWaypointIndex = patrolPath.GetNextIndex(curentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint() { //! Patrol 
            return patrolPath.GetWaypoint(curentWaypointIndex);
        }

        private void SuspicionBehaviour() {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour() {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
        }

        private bool InAttackDistanceOfPlayer() {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer < chaseDistance;
        }

        // Called by Unity
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}

