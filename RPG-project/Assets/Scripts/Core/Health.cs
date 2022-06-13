using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Core {

    public class Health : MonoBehaviour, ISaveable {

        [SerializeField] float healthPoints = 100f;
        private bool isDead = false;

        public void TakeDamage(float damage) {

            healthPoints = Mathf.Max(healthPoints - damage, 0);
            if (healthPoints == 0) {
                Die();                
            }
        }

        private void Die() {
            if (isDead) { return; };
            
            isDead = true;
            GetComponent<Animator>().SetTrigger("death");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public bool GetIsDead() {
            return this.isDead;
        }

        public object CaptureState() {
            return healthPoints;             
        }

        public void RestoreState(object state) {
            healthPoints = (float)state;

            if (healthPoints == 0) {
                Die();
            }
        }
    }

}

