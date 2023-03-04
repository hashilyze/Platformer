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
        
    }
    public void OnJump(InputAction.CallbackContext ctx) 
    {
        switch (ctx.phase)
        {
        case InputActionPhase.Started:
        {
            for(int beg = 0, end = _players.Count; beg < end; beg++)
            {

            }
        }
        break;
        case InputActionPhase.Canceled:
        {
            for (int beg = 0, end = _players.Count; beg < end; beg++)
            {

            }
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

    private void Update ()
    {
        for(int beg = 0, end = _players.Count; beg < end; ++beg)
        {
            
        }
    }


    protected override void InitializeInstance ()
    {
        this.hideFlags = HideFlags.NotEditable;
        this.gameObject.hideFlags = HideFlags.NotEditable;
    }
}
