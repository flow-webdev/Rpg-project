using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control {

    public class PlayerController : MonoBehaviour {

        Health health;        

        [System.Serializable]
        struct CursorMapping {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;        
        [SerializeField] float raycastRadius = 0.5f;        

        private void Awake() {
            health = GetComponent<Health>();
        }

        private void Update() {
            if (InteractWithUI()) { return; }
            
            if (health.GetIsDead()) {
                SetCursor(CursorType.None);
                return; 
            }

            if (InteractWithComponent()) { return; }
            if (InteractWithMovement()) { return; }

            SetCursor(CursorType.None);
        }

        private bool InteractWithUI() {
            if (EventSystem.current.IsPointerOverGameObject()) {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private bool InteractWithComponent() {
            RaycastHit[] hits = RaycastAllSorted(); // All the point where the raycast hit are in the array

            foreach (RaycastHit hit in hits) {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                
                foreach (IRaycastable raycastable in raycastables) {

                    if (raycastable.HandleRaycast(this)) {
                        SetCursor(raycastable.GetCursorType());
                        return true; 
                    }
                }                
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted() {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];            
            
            for (int i = 0; i < hits.Length; i++) {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithMovement() {
            // RaycastHit hitDetails;
            // bool hasHit = Physics.Raycast(GetMouseRay(), out hitDetails); // We store in the out var info (position) on where the raycast hit

            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);

            if (hasHit) { // it found some collider

                if (!GetComponent<Mover>().CanMoveTo(target)) { return false; }
                
                if (Input.GetMouseButton(0)) {
                    GetComponent<Mover>().StartMoveAction(target, 1f); //hitDetails.point
                }
                SetCursor(CursorType.Movement);
                return true;            
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target) { //! Disable movement if not hovering over a navmesh
            target = new Vector3();
            RaycastHit hitDetails;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hitDetails); // We store in the out var info (position) on where the raycast hit
            if (!hasHit) { return false; } //! Did not hit anything

            NavMeshHit navMeshHit;
            bool hasHitNavMesh = NavMesh.SamplePosition(hitDetails.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);

            if (!hasHitNavMesh) { return false; } //! Did not hit navMesh

            target = navMeshHit.position;

            //! Spostato in Mover CanMoveTo
            // NavMeshPath path = new NavMeshPath();
            // bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            // if (!hasPath) { return false; } 
            // if (path.status != NavMeshPathStatus.PathComplete) { return false; } //! Exclude unreachable routes (top building)
            // if (GetPathLength(path) > maxNavPathLength) { return false; } //! Exclude routes of more than a certain length

            return hasHitNavMesh;
        }        

        private void SetCursor(CursorType type) {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type) {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type) { return mapping; }
            } 
            return cursorMappings[0];
        }

        private static Ray GetMouseRay() { // cast a ray from the camera
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}

//! RAYCASTING 
//! Is the process of shooting an invisible ray from a point, in a specified directioon, to detect whether 
//! any colliders lay in the path of the ray. A Ray is the origin point and the direction.

// private bool InteractWithCombat() {
//     RaycastHit[] hits = Physics.RaycastAll(GetMouseRay()); // All the point where the raycast hit are in the array

//     foreach (RaycastHit hit in hits) {
//         CombatTarget target = hit.transform.GetComponent<CombatTarget>(); // check if one of the transform point is a combat target

//         if (target == null) { continue; }     
//         if (!GetComponent<Fighter>().CanAttack(target.gameObject)) { continue; } // if is hitting terrain or dead enemy, nothing happens

//         if (Input.GetMouseButtonDown(0)) { // else if clicked, call attack method in Fighter
//             GetComponent<Fighter>().Attack(target.gameObject);
//         }
//         SetCursor(CursorType.Combat);
//         return true;                                                        
//     }
//     return false;
// }
