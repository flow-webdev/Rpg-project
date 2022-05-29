using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;

namespace RPG.Combat {

    public class Fighter : MonoBehaviour {

        Transform target;
        [SerializeField] float weaponRange = 2f;

        private void Update() {

            if (target == null) {
                return;
            
            } else {
                if (!GetIsInRange()) {
                    GetComponent<Mover>().MoveTo(target.position); // moves toward target until is in range
                } else {
                    GetComponent<Mover>().Stop();
                }
            }            
        }

        private bool GetIsInRange() { // Check if distance between player/enemy is less than 2 meters
            return Vector3.Distance(transform.position, target.position) < weaponRange;
        }

        public void Attack(CombatTarget combatTarget) {
            target = combatTarget.transform;
        }

        public void Cancel() {
            target = null;
        }
    }
}

