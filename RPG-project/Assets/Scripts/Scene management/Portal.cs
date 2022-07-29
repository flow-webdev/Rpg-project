using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Saving;
using UnityEngine.AI;
using RPG.Control;

namespace RPG.SceneManagement {

    public class Portal : MonoBehaviour {

        enum DestinationIdentifier {
            A, B
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0.5f;

        private void OnTriggerEnter(Collider other) {            
            
            if (other.tag == "Player") { 
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition() {

            if (sceneToLoad < 0) {
                Debug.LogError("Scene to load not set");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            // Remove control
            PlayerController playerController = GameObject.Find("Player").GetComponent<PlayerController>();
            playerController.enabled = false;

            yield return fader.FadeOut(fadeOutTime);
            
            wrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            // Remove control
            PlayerController newPlayerController = GameObject.Find("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            //Load
            wrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            wrapper.Save();
            
            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            // Restore Control
            newPlayerController.enabled = true;

            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal) {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal() {

            foreach (Portal portal in FindObjectsOfType<Portal>()) {

                if (portal == this) { continue; }
                if (portal.destination != destination) { continue; }
                return portal;                
            }
            return null;
        }        

    }
}

