using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBinder : Utility.Singleton.SingletonComponent<InputBinder>
{
    private static readonly List<PlayerController> _players = new List<PlayerController>();

    // Player object storage
    public static void Possess(PlayerController player) 
    {
        if(!_players.Contains(player))
        {
            _players.Add(player);
        }
    }
    public static void Unpossess(PlayerController player) 
    {
        _players.Remove(player);
    }
    // Input adapter
    public void OnMove(InputAction.CallbackContext ctx) 
    {
        _inputAxis = ctx.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext ctx) 
    {
        switch (ctx.phase)
        {
        case InputActionPhase.Started:
        {
            for(int beg = 0, end = _players.Count; beg < end; beg++)
            {
                PlayerController player = _players[beg];
                player.JumpInput();
            }
        }
        break;
        case InputActionPhase.Canceled:
        {
            for (int beg = 0, end = _players.Count; beg < end; beg++)
            {
                PlayerController player = _players[beg];
                player.JumpInputUp();
            }
        }
        break;
        }
    }
    public void OnAttack(InputAction.CallbackContext ctx)  
    {
        switch (ctx.phase)
        {
        case InputActionPhase.Started:
        {
            for (int beg = 0, end = _players.Count; beg < end; beg++)
            {
                PlayerController player = _players[beg];
                
            }
        }
        break;
        }
    }
    public void OnDash (InputAction.CallbackContext ctx) 
    {
        switch (ctx.phase)
        {
        case InputActionPhase.Started:
        {
            for (int beg = 0, end = _players.Count; beg < end; beg++)
            {
                PlayerController player = _players[beg];
                player.DashInput();
            }
        }
        break;
        }
    }
    

    private Vector2 _inputAxis;

    private void Update ()
    {
        for(int beg = 0, end = _players.Count; beg < end; ++beg)
        {
            PlayerController player = _players[beg];
            player.MoveInput(_inputAxis);
        }
    }


    protected override void InitializeInstance ()
    {
        this.hideFlags = HideFlags.NotEditable;
        this.gameObject.hideFlags = HideFlags.NotEditable;
    }
}
