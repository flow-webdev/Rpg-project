using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core {

    public class PersistentObjectSpawner : MonoBehaviour {

        //! Prefab of object that we want to persist between levels instead of a Singleton
        [SerializeField] GameObject persistentObjectPrefab;

        static bool hasSpawn = false;

        private void Awake() {
            if (hasSpawn) { return; }

            SpawnPersistentObject();
            hasSpawn = true;
        }

        private void SpawnPersistentObject() {
            GameObject persistentObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}

