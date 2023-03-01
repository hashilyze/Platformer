using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPhys2D;

public enum EFaceDir
{
    Left = -1,
    Right = 1
}

public enum EState
{
    Walking,
    Flying
}

public class PlayerController : CharacterControllerBase2D
{
    #region Public
    public void Move(float direction)
    {
        Debug.Log("Move: " + direction);
    }
    public void Move(Vector2 direction)
    {
        Debug.Log("Move: " + direction);
    }

    public void Jump ()
    {
        Debug.Log("Jump");
    }
    public void StopJump ()
    {
        Debug.Log("Stop Jump");
    }

    public void Dash ()
    {

    }
    #endregion

    #region Private
    [Header("Ground Movement")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _firction;
    [Header("Air Movement")]
    [SerializeField] private float _maxAirSpeed;
    [SerializeField] private float _airAcceleration;
    [SerializeField] private float _drag;
    [Header("Jump Movement")]
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _moreJumpCount;
    [Header("Gravity")]
    [SerializeField] private bool _useGravity;
    [SerializeField] private float _gravity;
    [SerializeField] private float _fallLimit;
    // Components
    CharacterMovement2D _movement;
    // Inputs
    private Vector2 _inputAxis;

    private void Awake ()
    {
        _movement = GetComponent<CharacterMovement2D>();
        if(_movement == null) 
        {
            Debug.LogError(Utility.TextManager.MakeNullComponentReferenceMessage(_movement.GetType()));
        }
        InputBinder.Instance.Bind(this);
    }
    private void OnEnable ()
    {
        
    }
    private void OnDisable ()
    {
        
    }


    private void ApplyGravity(float deltaTime, ref Vector2 velocity)
    {
        if (_useGravity)
        {
            velocity.y -= _gravity * deltaTime;
            if (velocity.y < _fallLimit)
            {
                velocity.y = _fallLimit;
            }
        }
    }

    private float GetJumpSpeed(float jumpHeight)
    {
        return Mathf.Sqrt(2.0f * _gravity * jumpHeight);
    }
    #endregion

    public override void UpdateController (float deltaTIme, CharacterMovement2D movement)
    {
        base.UpdateController(deltaTIme, movement);
    }
}
