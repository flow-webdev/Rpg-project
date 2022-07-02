using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;
using RPG.Utils;

namespace RPG.Attributes {

    public class Health : MonoBehaviour, ISaveable {

        [SerializeField] float regenerationPercentage = 70;
        LazyValue<float> healthPoints; // Lazy Value wrapper/container makes sure initialization is called just before first use
        private bool isDead = false;

        BaseStats baseStats;

        private void Awake() {
            baseStats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetInitialHealth);
            // we pass GetInitialHealth which will be used when we need to initialize the var.
            // Now to access healthPoints we use healthPoints.value (need to make sure it is fully initialize)
        }

        private float GetInitialHealth() {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start() {            
            healthPoints.ForceInit(); // If at this point is not initialized, we will do it now.

            //! Not safe to initialize states in start: maybe used by some other class in the Awake
            // if (healthPoints < 0) {
            //     healthPoints = baseStats.GetStat(Stat.Health);
            // }
        }

        private void OnEnable() {
            baseStats.onLevelUp += RegenerateHealth;
        }

        private void OnDisable() {
            baseStats.onLevelUp -= RegenerateHealth;
        }

        public void TakeDamage(GameObject instigator, float damage) {

            print(gameObject.name + " took damage: " + damage);

            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            if (healthPoints.value == 0) {
                Die();
                AwardExperience(instigator);                
            }
        }

        public float GetHealthPoints() {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints() {
            return baseStats.GetStat(Stat.Health);
        }

        public float GetPercentage() {
            return 100 * (healthPoints.value / baseStats.GetStat(Stat.Health)); 
        }

        private void Die() {
            if (isDead) { return; };
            
            isDead = true;
            GetComponent<Animator>().SetTrigger("death");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AwardExperience(GameObject instigator) {
            Experience experience = instigator.GetComponent<Experience>();

            if (!experience) { return; }
            float experiencePoints = baseStats.GetStat(Stat.ExperienceReward);
            experience.GainExperience(experiencePoints);
        }

        private void RegenerateHealth() {
            // creo var con il 70% del total Health e setto health tra il maggiore di healthPoints e regenerateHealthPoints
            float regenerateHealthPoints = baseStats.GetStat(Stat.Health) * (regenerationPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenerateHealthPoints);
        }

        public bool GetIsDead() {
            return this.isDead;
        }


        // SAVING SYSTEM
        public object CaptureState() {
            return healthPoints;             
        }

        public void RestoreState(object state) {
            healthPoints.value = (float)state;

            if (healthPoints.value == 0) {
                Die();
            }
        }

    }

}

