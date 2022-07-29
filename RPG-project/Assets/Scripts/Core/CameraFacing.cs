using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core {

    public class CameraFacing : MonoBehaviour {

        private void LateUpdate() {

            //! Unlike Vector3.forward, Transform.forward moves the GameObject while also considering its rotation.
            transform.forward = Camera.main.transform.forward;
            // transform.LookAt(Camera.main.transform.position);
            // transform.Rotate(0, 180, 0);
        }

    }

}

