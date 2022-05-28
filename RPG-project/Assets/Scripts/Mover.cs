using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    [SerializeField] Transform target;
    Ray lastRay;

    private void Update() {
        if (Input.GetMouseButton(0)) {            
            MoveToCursor();
        }
        UpdateAnimator();
        //Debug.DrawRay(lastRay.origin, lastRay.direction * 400); //* draw the casted ray
    }

    private void UpdateAnimator() {
        //! 1- Get global velocity from navMesh
        //! 2- Convert it into local value relative to the character
        //! 3- Set animator blend value to be equal to the desired forward speed (on Z axis)
        Vector3 velocity = GetComponent<NavMeshAgent>().velocity; //Global velocity, but the animator needs the local one
        Vector3 localVelocity = transform.InverseTransformDirection(velocity); // Transforms a direction from world space to local space
        float speed = localVelocity.z;
        GetComponent<Animator>().SetFloat("forwardSpeed", speed);
    }

    private void MoveToCursor() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // cast a ray from the camera
        RaycastHit hitDetails;
        bool hasHit = Physics.Raycast(ray, out hitDetails); // We store in the out var info (position) on where the raycast hit
        if (hasHit) {
            GetComponent<NavMeshAgent>().destination = hitDetails.point;
        }
    }
}

//! RAYCASTING 
//! Is the process of shooting an invisible ray from a point, in a specified directioon, to detect whether 
//! any colliders lay in the path of the ray. A Ray is the origin point and the direction.


//! ANIMATOR COMPONENT
//! Assigns animations to GameObject through an Animator Controller 

//! ANIMATOR CONTROLLER
//! Arrangement of animations and transition (state machine)

//! ANIMATION
//! Specific pieces of motion 

//! BLEND TREE
//! Allows multiple animations to be blended together smoothly

//! VELOCITY
//! Rate of change of position overtime (car velocity is North at 50km/h)
//! NavMeshAgent is thinking in terms of velocity
//! Character locomotion animations are interested only in forward speed (along Z axis)