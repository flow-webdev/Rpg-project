using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText {

    public class DamageTextSpawner : MonoBehaviour {
        
        [SerializeField] DamageText damageTextPrefab = null;

        public void SpawnText(float damageAmount) {
            DamageText instance = Instantiate<DamageText>(damageTextPrefab, transform);
            instance.SetValue(damageAmount);
            // Text damageText = instance.GetComponentInChildren<Text>(true);
            // if (damageText != null) { damageText.text = damageAmount.ToString(); }
        }

    }

}

