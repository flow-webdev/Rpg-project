using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Stats {

    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/new Progression", order = 0)]
    public class Progression : ScriptableObject {

        [SerializeField] ProgressionCharacterClass[] classesArray = null;
        
        // Use a dictionary as a lookup table instead of running the function every frame
        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level) {

            BuildLookup();
            float[] levels =  lookupTable[characterClass][stat];

            if (levels.Length < level) { return 0; }
            return levels[level - 1];           
        }

        public int GetLevels(Stat stat, CharacterClass characterClass) {
            BuildLookup();
            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookup() {
            if (lookupTable != null) { return; }

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionClass in classesArray) {
                
                var statLookupTable = new Dictionary<Stat, float[]>();

                foreach (ProgressionStat progressionStat in progressionClass.stats) {

                    statLookupTable[progressionStat.stat] = progressionStat.levels; 
                }

                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }

        [System.Serializable] // To tell Unity that this is a class that we can be used in a serialize field
        class ProgressionCharacterClass {

            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        [System.Serializable] 
        class ProgressionStat {
            public Stat stat;
            public float[] levels;
        }

    }
}

//! PERFORMANCE ISSUES with this build of GetStat
// foreach (ProgressionCharacterClass progressionClass in classesArray) { // Per ogni progressionClass nell'array            
//     if (progressionClass.characterClass != characterClass) { continue; } // Se non e' la progressionClass passata continua

//     foreach (ProgressionStat progressionStat in progressionClass.stats) { // Per ogni progressionStat nella progressionClass
//         if (progressionStat.stat != stat) { continue; } // Se non e' la progressionStat passata continua

//         if (progressionStat.levels.Length < level) { continue; } // Se l'array di livelli non e' della lunghezza giusta continua

//         return progressionStat.levels[level -1]; // Ritorna la progressionStat al livello passato
//     }
// }
//return 0;