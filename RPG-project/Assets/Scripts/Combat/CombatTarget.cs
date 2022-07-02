using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat {

    [RequireComponent(typeof(Health))] // Will automatically add an Health component (script)
    public class CombatTarget : MonoBehaviour { }
    
}


