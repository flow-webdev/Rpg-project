using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! interface is a contract anything that implements it has to have a certain method
namespace RPG.Core {

    public interface IAction {
        void Cancel();
    }

}
