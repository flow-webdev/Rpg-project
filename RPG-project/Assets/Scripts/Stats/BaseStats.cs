using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Utils;
using UnityEngine;

namespace RPG.Stats {

    public class BaseStats : MonoBehaviour {
        
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = false;

        public event Action onLevelUp;

        LazyValue<int> currentLevel; // Lazy Value wrapper/container makes sure initialization is called just before first use

        Experience experience;

        private void Awake() {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
            // calculateLevel already returns a value, no need for another delegate
        }

        private void Start() {
            currentLevel.ForceInit();
        }

        private void OnEnable() {
            if (experience != null) {
                // Add UpdateLevel to the list of methods on onExperienceGained(). We subscribe to the Event.
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable() {
            if (experience != null) {
                // Add UpdateLevel to the list of methods on onExperienceGained(). We subscribe to the Event.
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel() {
            int newLevel = CalculateLevel();

            if (newLevel > currentLevel.value) {
                currentLevel.value = newLevel;                
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect() {
            if (levelUpParticleEffect != null) {
                Instantiate(levelUpParticleEffect, transform);
            }
        }

        //! METODO PRINCIPALE CHE RACCHIUDE ALTRI 3 METODI
        public float GetStat(Stat stat) {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100); // if 10, become 110%, as fraction is 1.1
        }

        private float GetBaseStat(Stat stat) {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        public int GetLevel() {
            // if (currentLevel.value < 1) { // No need to check if is initialized
            //     currentLevel.value = CalculateLevel();
            // }

            return currentLevel.value;
        }

        private float GetPercentageModifier(Stat stat) {
            if (!shouldUseModifiers) { return 0; }
            
            IEnumerable modifierProviderList = GetComponents<IModifierProvider>(); // faccio una lista di tutte le classi che implementano IModifierProvider
            float total = 0;

            foreach (IModifierProvider provider in modifierProviderList) { // per ogni singolo IModifierProvider in lista,
                IEnumerable<float> test = provider.GetPercentageModifiers(stat); // prendo la lista di float che contiene

                foreach (float modifier in test) { // per ogni float nella lista,
                    total += modifier; // lo aggiungo a total
                }
            }
            return total;
        }

        private float GetAdditiveModifier(Stat stat) {
            if (!shouldUseModifiers) { return 0; }

            IEnumerable modifierProviderList = GetComponents<IModifierProvider>(); // faccio una lista di tutte le classi che implementano IModifierProvider
            float total = 0;

            foreach (IModifierProvider provider in modifierProviderList) { // per ogni singolo IModifierProvider in lista,
                IEnumerable<float> test = provider.GetAdditiveModifiers(stat); // prendo la lista di float che contiene

                foreach (float modifier in test) { // per ogni float nella lista,
                    total += modifier; // lo aggiungo a total
                }                
            }
            return total;
        }

        private int CalculateLevel() {
            Experience experience = GetComponent<Experience>();
            if (experience == null) { return startingLevel; }

            float currentXp = experience.GetPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelup, characterClass);            

            for (int level = 1; level <= penultimateLevel; level++) {

                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelup, characterClass, level);

                if (XPToLevelUp > currentXp) {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }        
    }

}

