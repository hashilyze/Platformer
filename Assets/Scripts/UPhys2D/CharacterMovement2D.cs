using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UPhys2D
{
    [System.Serializable]
    public struct GroundHitReport
    {
        public bool HitAnyGround { get => _hitAnyGround; set => _hitAnyGround = value; }
        public bool IsStable { get => _isStable; set => _isStable = value; }

        public Collider2D Collider { get => _collider; set => _collider = value; }
        public Vector2 Point { get => _point; set => _point = value; }
        public Vector2 Normal { get => _normal; set => _normal = value; }
        public float Angle { get => _angle; set => _angle = value; }
        public float Distance { get => _distance; set => _distance = value; }

        [SerializeField] private bool _hitAnyGround;
        [SerializeField] private bool _isStable;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private Vector2 _point;
        [SerializeField] private Vector2 _normal;
        [SerializeField] private float _angle;
        [SerializeField] private float _distance;
    }

    [RequireComponent(typeof(CollisionHandler2D))]
    public class CharacterMovement2D : MonoBehaviour
    {
        #region Public
        public Vector2 Velocity { get => _velocity; set => _velocity = value; }
        public float MinDistance { get => _minDistance; set => _minDistance = value; }
        public float Mass { get => _mass; set => _mass = value; }
        public Vector2 CharacterUp => Vector2.up;

        public bool UseGroundSnap { get => _useGroundSnap; set => _useGroundSnap = value; }
        public float SloopLimit { get => _slopeLimit; set => _slopeLimit = Mathf.Clamp(value, 0.0f, 90.0f); }
        public float StepOffset { get => _stepOffset; set => _stepOffset = Mathf.Max(0.0f, value); }

        public GroundHitReport GroundReport { get => _groundReport; }

        public LayerMask CollidableMask { get => _collidableMask; set => _collidableMask = value; }
        public LayerMask BlockMask { get => _blockMask; set => _blockMask = value; }
        public LayerMask ColliableBlockMask { get => _collidableMask & _blockMask; }

        public void Move(Vector2 distance) { }
        public void Teleprot(Vector2 destination) { }

        /// <summary>Untact from ground until time over</summary>
        public void ForceUnground(float time)
        {
            _isForceUnground = true;
            _leftUngroundTime = time;
            ClearGroundReport();
        }

        public void Simulate(float deltaTime)
        {
            CacheCurrentTransform();
            // Handle riding
            HandleRiding(deltaTime);
            // Overlap recovery
            SolveOverlap();
            // Update Controller
            _controller.UpdateController(deltaTime, this);
            // Move by velocity
            if(_minDistance * _minDistance <= _velocity.sqrMagnitude * deltaTime * deltaTime)
            {
                InternalSafeMoveWithSlide(_velocity * deltaTime);
            }
            else
            {
                _velocity = Vector2.zero;
            }
            // Probe and snap ground
            ProbeGround(deltaTime);

            ApplyNextTransform();
        }
        #endregion
        #region Private
        // Memory Cache
        private readonly Collider2D[] _colliderBuffer = new Collider2D[8];
        private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[8];

        [Header("Base")]
        [ReadOnly][SerializeField] private Vector2 _velocity;
        [SerializeField] private float _minDistance = 0.1f;
        [SerializeField] private float _mass = 100.0f;
        [Header("Ground Movement")]
        [SerializeField] private bool _useGroundSnap = true;
        [Range(0.0f, 90.0f)][SerializeField] private float _slopeLimit = 50.0f;
        [SerializeField] private float _stepOffset = 0.2f;
        [ReadOnly][SerializeField] private GroundHitReport _groundReport;
        private bool _isForceUnground = false;
        private float _leftUngroundTime = 0.0f;
        [Header("Riding")]
        [ReadOnly][SerializeField] private Rigidbody2D _riding;
        [ReadOnly][SerializeField] private Vector2 _ridingContactPoint;
        private bool _inHandleRiding = false;
        [Header("Misc")]
        [SerializeField] private LayerMask _collidableMask = -1;
        [SerializeField] private LayerMask _blockMask = -1;
        // Components
        private Rigidbody2D _rb2d;
        private BoxCollider2D _body;
        private CollisionHandler2D _collisionHandler;
        private CharacterControllerBase2D _controller;
        // Transform
        private Vector2 _initPos;
        private Vector2 _nextPos;

        // Life Cycle Management
        private void Awake ()
        {
            // Setup _rb2d
            if(!TryGetComponent(out _rb2d))
            {
                Debug.LogError(Utility.TextManager.MakeNullComponentReferenceMessage(_rb2d.GetType()));
            }
            _rb2d.isKinematic = true;
            _rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
            // Setup _body
            if (!TryGetComponent(out _body))
            {
                Debug.LogError(Utility.TextManager.MakeNullComponentReferenceMessage(_body.GetType()));
            }
            _body.isTrigger = false;
            // Setup _collisionHandler
            if (!TryGetComponent(out _collisionHandler))
            {
                Debug.LogError(Utility.TextManager.MakeNullComponentReferenceMessage(_collisionHandler.GetType()));
            }
            _collisionHandler.Setup(_body);
            // Setup _controller
            if (!TryGetComponent(out _controller))
            {
                Debug.LogError(Utility.TextManager.MakeNullComponentReferenceMessage(_controller.GetType()));
            }
        }
        private void OnEnable ()
        {
            UPhysSystem2D.RegisterCharacter(this);
        }
        private void OnDisable () 
        {
            UPhysSystem2D.UnregisterCharacter(this);
        }
        // Optimizer for transform update
        private void CacheCurrentTransform ()
        {
            _initPos = _nextPos = _rb2d.position;
        }
        private void ApplyNextTransform ()
        {
            _rb2d.position = _nextPos;
            transform.position = new Vector3(_nextPos.x, _nextPos.y, transform.position.z);
        }
        // Handle Riding
        private void HandleRiding (float deltaTime)
        {
            if (!CanSnapRiding())
            {
                return;
            }

            Vector3 snapDistance = UPhysUtility2D.GetPointVelocity(_riding.velocity, _riding.angularVelocity, _ridingContactPoint - _riding.position) * deltaTime;

            _inHandleRiding = true;
            InternalSafeMoveWithCollide(snapDistance);
            _inHandleRiding = false;
        }
        private bool CanSnapRiding ()
        {
            //return _riding != null;

            _riding = null;
            // Ride moving platform
            if (_groundReport.HitAnyGround && _groundReport.IsStable)
            {
                // Ground Object is removed or destoryed 
                if (_groundReport.Collider == null)
                {
                    return false;
                }
                _riding = _groundReport.Collider.attachedRigidbody;
                _ridingContactPoint = _groundReport.Point;
                return true;
            }
            return false;
        }
        // Handle Ground
        private void ProbeGround (float deltaTime)
        {
            if (UpdateForceUngroundTimer(deltaTime))
            {
                return;
            }

            // Before update grounding report, cache previous report; used for checking OnLanded
            bool wasStable = _groundReport.HitAnyGround && _groundReport.IsStable;

            float probeDistance = UPhysSettings2D.Instance.SkinWidth * 2.0f;

            Vector2 movementDistance = _nextPos - _initPos;
            float horizontalDistance = movementDistance.x;
            float verticalDistance = movementDistance.y;

            // Snapable ground probing range
            float maxSnapDistance = 0.0f;
            if (wasStable)
            {
                maxSnapDistance = GetSnapGroundDistance(horizontalDistance, verticalDistance, _slopeLimit);
            }

            // Cast to downward from bottom of character
            if (_collisionHandler.Sweep(_nextPos, -CharacterUp, probeDistance + maxSnapDistance, _hitBuffer, out RaycastHit2D closestHit, _collidableMask & _blockMask, IsValidCollider) > 0)
            {
                float angle = Vector2.Angle(CharacterUp, closestHit.normal);

                // Accurated snapable range because of ground's angleFor
                float minSnapDistance = 0.0f;
                // Don't snap when on steep ramp
                if (wasStable && angle <= _slopeLimit)
                {
                    minSnapDistance = GetSnapGroundDistance(horizontalDistance, verticalDistance, angle);
                }
                // Grounded when on ground or snapable
                if (closestHit.distance <= probeDistance + minSnapDistance)
                {
                    _groundReport.HitAnyGround = true;

                    _groundReport.Collider = closestHit.collider;
                    _groundReport.Point = closestHit.point;
                    _groundReport.Distance = closestHit.distance;
                    _groundReport.Normal = closestHit.normal;
                    _groundReport.Angle = angle;
                    _groundReport.IsStable = angle <= _slopeLimit;
                }
                else
                {
                    ClearGroundReport();
                }
            }
            else
            {
                ClearGroundReport();
            }

            // Events after ground probing
            // On Ground
            if (_groundReport.HitAnyGround && _groundReport.IsStable)
            {
                if (_useGroundSnap)
                {
                    _nextPos -= (_groundReport.Distance - UPhysSettings2D.Instance.SkinWidth) * CharacterUp;
                    _groundReport.Distance = UPhysSettings2D.Instance.SkinWidth;
                }

                // On Landed (first tick when grounded)
                if (!wasStable)
                {
                    // Discard vertical velocity
                    _velocity.y = 0.0f;

                    _controller.OnLand();
                }
            }
        }
        /// <summary>Update Timer for ForceUnground</summary>
        /// <returns>True when have left time after updated</returns>
        private bool UpdateForceUngroundTimer (float deltaTime)
        {
            return _isForceUnground && (_isForceUnground = (_leftUngroundTime -= deltaTime) > 0.0f);
        }

        private void ClearGroundReport ()
        {
            _groundReport.HitAnyGround = false;
            _groundReport.Collider = null;
            _groundReport.Point = Vector2.zero;
            _groundReport.Distance = 0.0f;
            _groundReport.Normal = Vector2.zero;
            _groundReport.Angle = 0.0f;
            _groundReport.IsStable = false;
        }
        private float GetSnapGroundDistance (float horizontalDist, float verticalDist, float groundAngle)
        {
            float snapDistance = horizontalDist * Mathf.Tan(groundAngle * Mathf.Deg2Rad);
            if (verticalDist > 0.0f)
            {
                snapDistance += verticalDist;
            }
            return snapDistance;
        }


        /// <summary>Descrete conllision handle; Depentrate character from blocks</summary>
        private void SolveOverlap ()
        {
            Vector2 backupPosition = _nextPos;

            int currentIteration = 0;
            int maxIteration = UPhysSettings2D.Instance.DepentrationIteration;
            while (currentIteration < maxIteration)
            {
                // If there are overlaped collider with character, detach character from these
                int overlapCount = _collisionHandler.Overlap(_nextPos, _colliderBuffer, _collidableMask, IsValidCollider);
                // If no more found overlaped collider
                if (overlapCount == 0)
                {
                    break;
                }
                for (int cur = 0; cur < overlapCount; ++cur)
                {
                    Collider2D overlapCollider = _colliderBuffer[cur];
                    UPhysUtility2D.GetPosAndRot(overlapCollider, out Vector2 overlapPos, out float overlapRot);

                    // Depentrate if colliders are overlaped with deeper than zero
                    if (UPhysUtility2D.ComputePenetration(_body, _nextPos, 0.0f, overlapCollider, overlapPos, overlapRot, out Vector2 dir, out float dist))
                    {
                        if (InternalSafeMoveWithCollide(dist * dir, out Vector2 remainingDistance, out RaycastHit2D hitInfo))
                        {
                            if (remainingDistance.sqrMagnitude > 0.0f)
                            {
                                // * Caution: Very very unstable process, need to more test;
                                // recommnad using squish instead of this soultion or dynamic rigidbody
                                // Slided push without projection of distance unless squished
                                if (Vector2.Angle(-dir, hitInfo.normal) > 5.0f)
                                {
                                    Vector2 tangent = UPhysUtility2D.GetTangent(dir, hitInfo.normal);

                                    float leftDistOnDir = dist - (hitInfo.distance - UPhysSettings2D.Instance.SkinWidth);
                                    float tangentOnDir = Vector2.Dot(tangent, dir);
                                    if (tangentOnDir > 0.001f)
                                    {
                                        float leftDistOnTangent = leftDistOnDir / tangentOnDir;

                                        InternalSafeMoveWithCollide(leftDistOnTangent * tangent);
                                    }
                                }
                            }
                        }
                    }
                }
                ++currentIteration;
            }

            if (currentIteration >= maxIteration)
            {
                // Break overlap recovery because character probably located in unsafed zone
                if (UPhysSettings2D.Instance.KillPositionWhenExceedDepentrationIteration)
                {
                    _nextPos = backupPosition;
                }
            }
        }

        /// <summary>Continous movement until collide</summary>
        private bool InternalSafeMoveWithCollide (Vector2 distance, out Vector2 remainingDistance, out RaycastHit2D hit)
        {
            Vector2 direction = distance.normalized;
            float magnitude = distance.magnitude;
            // Character react to obstacles when crush
            if (_collisionHandler.Sweep(_nextPos, direction, magnitude + UPhysSettings2D.Instance.SkinWidth,
                    _hitBuffer, out RaycastHit2D closestHit, _collidableMask & _blockMask, IsValidCollider) > 0)
            {
                hit = closestHit;
                // Move character until collide
                closestHit.distance -= UPhysSettings2D.Instance.SkinWidth;
                _nextPos += closestHit.distance * direction;
                // Check having remaining distance
                if ((magnitude -= closestHit.distance) > 0.0f)
                {
                    remainingDistance = magnitude * direction;
                    return true;
                }
                else
                {
                    remainingDistance = Vector2.zero;
                    return false;
                }
            }
            else
            {
                // There are no obstacles blocking movement
                _nextPos += distance;
                hit = default;
                remainingDistance = Vector2.zero;
                return false;
            }
        }
        private void InternalSafeMoveWithCollide(Vector2 distance)
        {
            InternalSafeMoveWithCollide(distance, out Vector2 _, out RaycastHit2D _);
        }
        /// <summary>Continous movement and slide surface when collide</summary>
        private void InternalSafeMoveWithSlide (Vector3 distance)
        {
            Vector2 backupPosition = _nextPos;
            Vector2 nextDistance = distance;

            int currentIteration = 0;
            int maxIteration = UPhysSettings2D.Instance.VelocityIteration;
            while (currentIteration < maxIteration && nextDistance.sqrMagnitude > 0.0f)
            {
                if (InternalSafeMoveWithCollide(nextDistance, out Vector2 remainingDistance, out RaycastHit2D hitInfo))
                {
                    // Project remaining distance to surface
                    Vector2 tangent = UPhysUtility2D.GetTangent(remainingDistance, hitInfo.normal);
                    nextDistance = Vector3.Dot(tangent, remainingDistance) * tangent;
                    
                    // Don't climbing unstable ground when on stable ground
                    // Unstable ground is regarded as a wall
                    if (_groundReport.HitAnyGround && _groundReport.IsStable)
                    {
                        if (Vector2.Angle(CharacterUp, hitInfo.normal) > _slopeLimit)
                        {
                            // Discard upward distance
                            nextDistance.y = 0.0f;
                        }
                    }
                    ++currentIteration;
                }
                else
                {
                    nextDistance = Vector3.zero;
                    break;
                }
            }

            // Exceed velocity solve iteration
            if (currentIteration >= maxIteration)
            {
                // Discard calculated movement 
                if (UPhysSettings2D.Instance.KillPositionWhenExceedVelocityIteration)
                {
                    _nextPos = backupPosition;
                }
                // Appand remained distance to movement
                if (!UPhysSettings2D.Instance.KillRemainedDistanceWhenExceedVelocityIteration)
                {
                    _nextPos += nextDistance;
                }
            }
        }

        // Filter
        private bool IsValidCollider (Collider2D col)
        {
            // Ignore itself
            if (col == _body) return false;

            Rigidbody2D rb = col.attachedRigidbody;
            if (rb != null)
            {
                // Ignore dynamic rigidbody
                if (!rb.isKinematic) return false;
                // Ignore my object
                if (rb == _body.attachedRigidbody) return false;
                // Ignore movoing block in override update
                if (_inHandleRiding && rb == _riding) return false;
            }

            return true;
        }
        private bool IsValidCollider (RaycastHit2D hit)
        {
            return IsValidCollider(hit.collider);
        }
        #endregion
    }
}