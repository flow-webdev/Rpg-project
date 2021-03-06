using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat {

    public class Projectile : MonoBehaviour {

        [SerializeField] float speed = 0;
        [SerializeField] bool isHoming = true;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] UnityEvent onProjectileHit;

        Health target = null;
        GameObject instigator = null;
        float damage = 0;

        private void Start() {
            transform.LookAt(GetAimLocation());
        }

        private void Update() {
            if (target == null) { return; }
            if (isHoming && !target.GetIsDead()) { 
                transform.LookAt(GetAimLocation()); 
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage) {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;

            Destroy(gameObject, maxLifeTime);
        }

        private Vector3 GetAimLocation() {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null) { return target.transform.position; }
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
            // Vector3.up in order to add only on the y axis
        }

        private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<Health>() != target) { return; }
            if (target.GetIsDead()) { return; }
            target.TakeDamage(instigator, damage);

            speed = 0;
            onProjectileHit.Invoke();
            
            if (hitEffect != null) { 
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);                
            }

            foreach (GameObject toDestroy in destroyOnHit) { // destroy different GO at different times
                Destroy(toDestroy);
            }
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}

