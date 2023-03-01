using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBinder : Utility.SingletonComponent<InputBinder>
{
    private PlayerController _player;

    public void Bind (PlayerController player) { _player = player; }
    public void Unbind () { _player = null; }

    public void OnMove(InputAction.CallbackContext ctx) 
    { 
        Vector2 direction = ctx.ReadValue<Vector2>();
        _player.Move(direction);
    }
    public void OnJump(InputAction.CallbackContext ctx) 
    {
        switch (ctx.phase)
        {
        case InputActionPhase.Started:
        {
            _player.Jump();
        }
        break;
        case InputActionPhase.Canceled:
        {
            _player.StopJump();
        }
        break;
        }
    }
    public void OnAttack(InputAction.CallbackContext ctx) 
    { 

    }
    public void OnDash (InputAction.CallbackContext ctx)
    {

    }

    protected override void InitializeInstance ()
    {
        this.hideFlags = HideFlags.NotEditable;
        this.gameObject.hideFlags = HideFlags.NotEditable;
    }
}
