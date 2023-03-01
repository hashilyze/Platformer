using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPhys2D
{
    public class PlatformMovement2D : MonoBehaviour
    {
        #region Public
        public Vector2 Velocity { get => _velocity; set => _velocity = value; }
        public float AngularVelocity { get => _angularVelocity; set => _angularVelocity = value; }


        public void Move (Vector2 distance)
        {
            _rb2d.position += distance;
            transform.position += (Vector3)distance;
        }

        public void Turn(float angle)
        {
            _rb2d.rotation += angle;
            transform.rotation = Quaternion.Euler(.0f, .0f, transform.rotation.eulerAngles.z + angle);
        }

        public void Teleport (Vector2 destination)
        {
            _rb2d.position = destination;
            transform.position = destination;
        }

        public void Look (float angle)
        {
            _rb2d.rotation = angle;
            transform.rotation = Quaternion.Euler(.0f, .0f, angle);
        }

        /// <summary>Update process of movement for platform which managed by UPhysSystem</summary>
        public void Simulate(float deltaTime)
        {
            CacheCurrentTransform();

            _controller.UpdateController(deltaTime, this);
            InternalMove(deltaTime);

            CommitNextTransform();
        }
        #endregion

        #region Public
        [Header("Base")]
        [ReadOnly] [SerializeField] private Vector2 _velocity;
        [ReadOnly] [SerializeField] private float _angularVelocity;
        // Components
        private Rigidbody2D _rb2d;
        private PlatformControllerBase2D _controller;
        // Transform
        private Vector2 _initPos;
        private float _initRot;
        private Vector2 _nextPos;
        private float _nextRot;


        private void Awake ()
        {
            // Initialize _rb2d
            _rb2d = GetComponent<Rigidbody2D>();
            if(_rb2d == null)
            {
                Debug.LogError(Utility.TextManager.MakeNullComponentReferenceMessage(_rb2d.GetType()));
            }
            _rb2d.isKinematic = true;
            // Initialize _controller
            _controller = GetComponent<PlatformControllerBase2D>();
            if (_controller == null)
            {
                Debug.LogError(Utility.TextManager.MakeNullComponentReferenceMessage(_controller.GetType()));
            }
        }

        private void OnEnable ()
        {
            UPhysSystem2D.RegisterPlatform(this);
        }
        private void OnDisable ()
        {
            UPhysSystem2D.UnregisterPlatform(this);
        }

        private void InternalMove (float deltaTime)
        {
            _rb2d.velocity = _velocity;
            _rb2d.angularVelocity = _angularVelocity;

            _nextPos += _velocity * deltaTime;
            _nextRot += _angularVelocity * deltaTime;
        }


        private void CacheCurrentTransform ()
        {
            _initPos = _nextPos = _rb2d.position;
            _initRot = _nextRot = _rb2d.rotation;
        }
        private void CommitNextTransform ()
        {
            _rb2d.position = _nextPos;
            _rb2d.rotation = _nextRot;

            transform.SetPositionAndRotation(_nextPos, Quaternion.Euler(0.0f, 0.0f, _nextRot));
        }
        #endregion
    }
}