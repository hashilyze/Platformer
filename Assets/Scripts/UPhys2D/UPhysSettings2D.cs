using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPhys2D
{
    public enum ECharacterInteraction
    {
        PassThrough = 0,    // None
        Push = 1,           // OverlapRecovery
        Block = 2,          // MoveWithCollide, OverlapRecovery
    }

    [CreateAssetMenu(fileName = "UPhys Settings 2D", menuName = "UPHys2D/UPhysSettings2D")]
    public class UPhysSettings2D : Utility.Singleton.SingletonScriptableObject<UPhysSettings2D>
    {
        // Solve Overlap
        public int DepentrationIteration => _depentrationIteration;
        public bool KillPositionWhenExceedDepentrationIteration => _killPositionWhenExceedDepentrationIteration;
        // Solve Velocity
        public int VelocityIteration => _velocityIteration;
        public bool KillPositionWhenExceedVelocityIteration => _killPositionWhenExceedVelocityIteration;
        public bool KillRemainedDistanceWhenExceedVelocityIteration => _killRemainedDistanceWhenExceedVelocityIteration;
        // Misc
        public float SkinWidth => _skinWidth;
        public ECharacterInteraction CharacterInteraction => _characterInteraction;

        [Header("Solve Overlap")]
        [Tooltip("Accuracy of depentration solver; Higher costs more")]
        [SerializeField] private int _depentrationIteration = 1;
        [Tooltip("Canceal depentration interation")]
        [SerializeField] private bool _killPositionWhenExceedDepentrationIteration = false;
        [Tooltip("Accuracy of velocity solver; Higher costs more")]
        [Header("Solve Velocity")]
        [SerializeField] private int _velocityIteration = 3;
        [Tooltip("Canceal velocity interation")]
        [SerializeField] private bool _killPositionWhenExceedVelocityIteration = true;
        [Tooltip("Discard remained deistance but not appand")]
        [SerializeField] private bool _killRemainedDistanceWhenExceedVelocityIteration = true;
        [Header("Misc")]
        [Tooltip("Gap between character and others")]
        [SerializeField] private float _skinWidth = 0.02f;
        [Tooltip("Interaction style between characters")]
        [SerializeField] private ECharacterInteraction _characterInteraction = ECharacterInteraction.PassThrough;
    }
}