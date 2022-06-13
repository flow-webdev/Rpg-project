using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;
using RPG.Saving;

namespace RPG.Combat {

    public class Fighter : MonoBehaviour, IAction, ISaveable {
        
        [SerializeField] float timeBetweenAttacks = 2f;        
        [Range(0, 1)]
        [SerializeField] float attackSpeedFraction = 1f;        
        [SerializeField] Transform rightHandTransform = null;    
        [SerializeField] Transform leftHandTransform = null;    
        [SerializeField] Weapon defaultWeapon = null;

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        Weapon currentWeapon = null;   

        private void Start() {
            if (currentWeapon == null) { EquipWeapon(defaultWeapon); }
        }        

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

        public void EquipWeapon(Weapon weapon) {
            currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
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

        //! Animation Hit Event, called within animator
        void Hit() {
            if (target == null) { return; }

            if (currentWeapon.HasProjectile()) {
                currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target);
            } else {
                target.TakeDamage(currentWeapon.GetDamage());
            }
        }

        //! Animation Shoot Event, called within animator for arrows
        void Shoot() {
            Hit();
        }

        private bool GetIsInRange() { // Check if distance between player/enemy is less than 2 meters
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.GetRange();
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

        public object CaptureState() {
            return currentWeapon.name;
        }

        public void RestoreState(object state) {
            string weaponName = (string)state;
            // Load with string in order to being able to save/load from level to level
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}

