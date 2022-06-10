using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat {

    public class Fighter : MonoBehaviour, IAction {

        [SerializeField] float weaponRange = 1f;
        [SerializeField] float timeBetweenAttacks = 2f;
        [SerializeField] float weaponDamage = 5f;
        [Range(0, 1)]
        [SerializeField] float attackSpeedFraction = 1f;

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;        

        private void Update() {

            timeSinceLastAttack += Time.deltaTime;

            if (target == null) { return; };
            if (target.GetIsDead()) { return; };
                           
            if (!GetIsInRange()) {
                GetComponent<Mover>().MoveTo(target.transform.position, attackSpeedFraction); // moves toward target until is in range
            } else {                    
                GetComponent<Mover>().Cancel();
                AttackBehaviour();                
            }                     
        }

        private void AttackBehaviour() {
            transform.LookAt(target.transform);

            if (timeSinceLastAttack > timeBetweenAttacks) {
                TriggerAttack(); // Start animation, will trigger Hit() Event      
                timeSinceLastAttack = 0; // Reset wait time for animation                                              
            }            
        }

        private void TriggerAttack() { // Before set trigger("stopAttack") had a glitch. SO reset previous trigger
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack"); 
        }

        // Animation Hit Event, called within animator
        void Hit() {
            if (target == null) { return; }
            target.TakeDamage(weaponDamage); // Damage enemy
        }

        private bool GetIsInRange() { // Check if distance between player/enemy is less than 2 meters
            return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
        }

        public void Attack(GameObject combatTarget) {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();            
        }

        public void Cancel() {
            StopAttackTrigger();
            target = null;
        }

        private void StopAttackTrigger() {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public bool CanAttack(GameObject combatTarget) {
            if (combatTarget == null) { return false; }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.GetIsDead();
        }
        
    }
}

