using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPhys2D;

public class SimpleMovor : PlatformControllerBase2D
{
    [SerializeField] private Vector2 _velocity;
    [SerializeField] private float _angularVelocity;


    public override void UpdateController (float deltaTime, PlatformMovement2D platform)
    {
        platform.Velocity = _velocity;
        platform.AngularVelocity = _angularVelocity;
    }
}
