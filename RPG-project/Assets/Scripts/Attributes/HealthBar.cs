using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes {
    
    public class HealthBar : MonoBehaviour {

        [SerializeField] Health health = null;
        [SerializeField] Canvas rootCanvas = null;
        [SerializeField] RectTransform foreground = null;

        private void Update() {
            //! floating point imprecision
            if (Mathf.Approximately(health.GetFraction(), 1) || Mathf.Approximately(health.GetFraction(), 0)) {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;
            foreground.localScale = new Vector3(health.GetFraction(), 1, 1);   
        }
    }
}

