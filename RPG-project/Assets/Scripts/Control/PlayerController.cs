using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using System;

namespace RPG.Control {

    public class PlayerController : MonoBehaviour {

        private void Update() {

            if (InteractWithCombat()) {
                return;
            } else if (InteractWithMovement()) {
                return;
            } else {
                print("Nothing to do");
            }       
        }

        private bool InteractWithCombat() {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay()); // All the point where the raycast hit are in the array

            foreach (RaycastHit hit in hits) {

                CombatTarget target = hit.transform.GetComponent<CombatTarget>(); // check if one of the transform point is a combat target
                
                if (target == null) {
                    continue; // if is not, nothing happens
                
                } else {
                    if (Input.GetMouseButtonDown(0)) { // else if is clicked, call attack method in Fighter
                        GetComponent<Fighter>().Attack(target);
                    }
                    return true;
                }                                          
            }
            return false;
        }

        private bool InteractWithMovement() {

            RaycastHit hitDetails;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hitDetails); // We store in the out var info (position) on where the raycast hit
            //! Casts a ray, from point origin, in direction direction, of length maxDistance, against all colliders in the Scene.

            if (hasHit) { // it found some collider
                if (Input.GetMouseButton(0)) {
                    GetComponent<Mover>().StartMoveAction(hitDetails.point);
                }
                return true;            
            }
            return false;
        }

        private static Ray GetMouseRay() { // cast a ray from the camera
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}

//! RAYCASTING 
//! Is the process of shooting an invisible ray from a point, in a specified directioon, to detect whether 
//! any colliders lay in the path of the ray. A Ray is the origin point and the direction.