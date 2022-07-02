using System.Collections;
using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats {

    public class Experience : MonoBehaviour, ISaveable {

        [SerializeField] float experiencePoints = 0;

        //! OBSERVER PATTERN
        // Where we want other classes to subscribe to our events without us having to know who they are.
        // Delegates are a list of pointers to methods. We can have a Delegate to be the type of the lists. 
        // Event is a protection on top of a Delegate instance to prevent overwriting those lists from outside the class.
        // Action is a predefined Delegate with no return value. So use a single liner instead of the 2 in example.
        
        //public delegate void ExperienceGainedDelegate();
        //public event ExperienceGainedDelegate onExperienceGained;
        public event Action onExperienceGained;

        public void GainExperience(float experience) {
            experiencePoints += experience;
            onExperienceGained();
        }

        public float GetPoints() {
            return experiencePoints;
        }


        // SAVING SYSTEM
        public object CaptureState() {
            return experiencePoints;
        }

        public void RestoreState(object state) {
            experiencePoints = (float)state;
        }
    }
}

