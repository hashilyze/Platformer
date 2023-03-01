using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPhys2D
{
    public class PlatformControllerBase2D : MonoBehaviour
    {
        public virtual void UpdateController (float deltaTime, PlatformMovement2D movement) { }

    }
}