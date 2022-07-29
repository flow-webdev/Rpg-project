using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using RPG.Utils;

namespace RPG.Combat {

    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider {
        
        [SerializeField] float timeBetweenAttacks = 2f;        
        [Range(0, 1)]
        [SerializeField] float attackSpeedFraction = 1f;        
        [SerializeField] Transform rightHandTransform = null;    
        [SerializeField] Transform leftHandTransform = null;    
        [SerializeField] WeaponConfig defaultWeapon = null;

        Health target;        
        float timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig currentWeaponConfig; // Lazy Value wrapper/container makes sure initialization is called just before first use
        LazyValue<Weapon> currentWeapon;

        private void Awake() {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon() { //! DELEGATE for lazy Initialization           
            return AttachWeapon(defaultWeapon);
        }

        private void Start() {
            currentWeapon.ForceInit();
        }        

        private void Update() {

            timeSinceLastAttack += Time.deltaTime;

            if (target == null) { return; };
            if (target.GetIsDead()) { return; };
                           
            if (!GetIsInRange(target.transform)) {
                GetComponent<Mover>().MoveTo(target.transform.position, attackSpeedFraction); // moves toward target until is in range
            } else {                    
                GetComponent<Mover>().Cancel();
                AttackBehaviour();                
            }                     
        }

        public void EquipWeapon(WeaponConfig weapon) {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon) {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget() {
            return target;
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
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (currentWeapon.value != null) {
                currentWeapon.value.OnHit();
            }

            if (currentWeaponConfig.HasProjectile()) {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            } else {
                
                target.TakeDamage(gameObject, damage);
            }
        }

        //! Animation Shoot Event, called within animator for arrows
        void Shoot() {
            Hit();
        }

        private bool GetIsInRange(Transform targetTransform) { // Check if distance between player/enemy is less than 2 meters
            return Vector3.Distance(transform.position, targetTransform.transform.position) < currentWeaponConfig.GetRange();
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
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform)) { 
                return false; 
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.GetIsDead();
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat) {
            if (stat == Stat.Damage) {
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat) {
            if (stat == Stat.Damage) {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }



        //SAVING SYSTEM
        public object CaptureState() {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state) {
            string weaponName = (string)state;
            // Load with string in order to being able to save/load from level to level
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

    }
}

