using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPhys2D
{
    public class CharacterControllerBase2D : MonoBehaviour
    {
        public virtual void UpdateController(float deltaTIme, CharacterMovement2D movement) { }

        public virtual void OnLand () { }
    }
}