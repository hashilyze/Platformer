using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPhys2D;

public enum EState
{
    Walking,
    Flying
}

public class PlayerController : CharacterControllerBase2D
{
    #region Public
    // Actions for Player Input
    public void MoveInput (Vector2 direction) { }
    public void MoveInput (float direction) 
    {
        MoveInput(direction * Vector2.right);
    }
    public void JumpInput () { }
    public void JumpInputUp () { }
    public void DashInput () { }
    // Actions for AI Input

    // Actions
    public void Move (Vector2 direction) { }
    public void Move (float direction) { }
    public void Jump () { }
    public void StopJump () { }
    public void Dash () { }

    public void Teleport (Vector2 destination) { }
    public void TeleportUponGround(Vector2 destination, float probeDistance = float.MaxValue) 
    {
        // Sweep to bottom
        // Teleport upon ground if find ground
    }

    #endregion

    #region Private
    [Header("Fly")]
    [SerializeField] private float _maxFlySpeed;
    [SerializeField] private float _flyAcceleration;
    [SerializeField] private float _flyDrag;
    [Header("Walk")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _firction;
    [Header("Fall")]
    [SerializeField] private float _maxFallSpeed;
    [SerializeField] private float _fallAcceleration;
    [SerializeField] private float _fallDrag;
    [Header("Gravity")]
    [SerializeField] private bool _useGravity = true;
    [SerializeField] private float _gravity = 40.0f;
    [SerializeField] private float _fallLimit = 25.0f;
    [SerializeField] private float _fallGravityMultifier = 1.2f; // Enforce grvity when fall
    [Header("Jump")]
    [SerializeField] private float _maxJumpHeight = 5.0f;
    [SerializeField] private float _minJumpHeight = 1.0f;
    [SerializeField] private float _moreJumpCount = 0;
    private float _leftMoreJumpCount;
    [SerializeField] private float _jumpBuffer = 0.1f;  // Retry jump during few times after faild jump
    [SerializeField] private float _coyoteTime = 0.1f;  // Keep enable jump during few times after leave ground
    // Components
    CharacterMovement2D _movement;
    // Inputs
    private Vector2 _inputAxis;

    // Life cycle management
    private void Awake ()
    {
        if (!TryGetComponent(out _movement))
        {
            Debug.LogError(Utility.TextManager.MakeNullComponentReferenceMessage(_movement.GetType()));
        }
    }
    private void OnEnable ()
    {
        InputBinder.Possess(this);
    }
    private void OnDisable ()
    {
        InputBinder.Unpossess(this);
    }


    private void CalculateVelocity (float deltaTime, ref Vector2 velocity)
    {
        // Walking
        {
            // Surface Tangent velocity
            Vector2 tangentVelocity = UPhysUtility2D.GetTangent(velocity, _movement.GroundReport.Normal);
            if(_inputAxis.x != 0.0f)
            {
                Vector2 tangentInput = UPhysUtility2D.GetTangent(Vector2.right, _movement.GroundReport.Normal);
                velocity = Vector2.Lerp(tangentVelocity, _maxSpeed * tangentInput, 1f - Mathf.Exp(-_acceleration * deltaTime));
            }
            else
            {
                velocity = Vector2.Lerp(tangentVelocity, Vector2.zero, 1f - Mathf.Exp(-_firction * deltaTime));
            }

        }
        // Falling (effected gravity on airbone)
        {
            // Horizontal velocity
            if (_inputAxis.x != 0.0f)
            {
                velocity.x = Mathf.Lerp(velocity.x, _maxFallSpeed * _inputAxis.x, 1f - Mathf.Exp(-_fallAcceleration * deltaTime));
            }
            else
            {
                velocity.x = Mathf.Lerp(velocity.x, 0.0f, 1f - Mathf.Exp(-_fallDrag * deltaTime));
            }
            // Vertical velocity
            ApplyGravity(deltaTime, ref velocity);
        }
        // Flying
        {
            if (_inputAxis != Vector2.zero)
            {
                velocity = Vector2.Lerp(velocity, _maxFlySpeed * _inputAxis, 1f - Mathf.Exp(-_flyAcceleration * deltaTime));
            }
            else
            {
                velocity = Vector2.Lerp(velocity, Vector2.zero, 1f - Mathf.Exp(-_flyDrag * deltaTime));
            }
        }
        // Swiming
        {

        }
    }

    private void ApplyGravity (float deltaTime, ref Vector2 velocity)
    {
        if (_useGravity)
        {
            //velocity.y = Mathf.Max(_fallLimit, velocity.y - _gravity * deltaTime * (velocity.y < 0.0f ? _fallGravityMultifier : 1.0f)); // same execution, another style
            velocity.y -= _gravity * deltaTime * (velocity.y < 0.0f ? _fallGravityMultifier : 1.0f);
            if (velocity.y < _fallLimit)
            {
                velocity.y = _fallLimit;
            }
        }
    }

    private bool CanJump ()
    {
        return true;
    }
    private void PerformJump ()
    {
        // Ground Jump
        // More Jump
        // Wall (bounce) Jump
    }
    private void ReleaseJump ()
    {

    }
    
    private float GetJumpSpeed(float jumpHeight)
    {
        return Mathf.Sqrt(2.0f * _gravity * jumpHeight);
    }


    private void Movement ()
    {
        
    }

    #endregion

    public override void UpdateController (float deltaTIme, CharacterMovement2D movement)
    {
        base.UpdateController(deltaTIme, movement);
    }
}
