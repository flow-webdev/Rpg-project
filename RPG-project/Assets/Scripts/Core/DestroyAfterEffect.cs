using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Core {

    public class DestroyAfterEffect : MonoBehaviour {

        [SerializeField] GameObject targetToDestroy = null;
        string target;

        private void Update() {
            if (!GetComponent<ParticleSystem>().IsAlive()) {

                if (targetToDestroy != null) { 
                    Destroy(targetToDestroy); 
                } else {
                    Destroy(gameObject);
                }
            }
        }


    }
}

