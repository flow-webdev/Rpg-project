using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement {

    public class SavingWrapper : MonoBehaviour {

        [SerializeField] float fadeInTime = 0.2f;

        const string defaultSaveFile = "save";

        IEnumerator Start() {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            yield return fader.FadeIn(fadeInTime);
        }

        private void Update() {

            if (Input.GetKeyDown(KeyCode.L)) { 
                Load(); 
            }

            if (Input.GetKeyDown(KeyCode.S)) { 
                Save(); 
            }
        }

        public void Save() {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load() {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }
    }
}

