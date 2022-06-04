using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control {

    public class PatrolPathScript : MonoBehaviour {

        const float waypointGizmosRadius = 0.3f;

        private void OnDrawGizmos() {
            for (int i = 0; i < transform.childCount; i++) {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmosRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        public int GetNextIndex(int i) {
            if (i + 1 == transform.childCount) { return 0; }
            return i + 1;
        }

        public Vector3 GetWaypoint(int i) {
            return transform.GetChild(i).position;
        }

        // public Vector3 GetCurrentWaypoint(Vector3 enemyPosition) {
        //     int waypointIndex = 0;
        //     for (int i = 0; i < transform.childCount; i++) {
        //         int 
        //     }
        //     return GetWaypoint(waypointIndex);
        // }
    }
}

// private void OnDrawGizmos() {
//     Vector3 firstPointPosition = new Vector3(0, 0, 0);
//     for (int i = 0; i < transform.childCount; i++) {
//         Gizmos.color = Color.red;
//         if (i == 0) { firstPointPosition = transform.GetChild(i).position; }
//         if (i == transform.childCount - 1) {
//             Gizmos.DrawLine(transform.GetChild(i).position, firstPointPosition);
//             break;
//         }
//         Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
//     }
// }