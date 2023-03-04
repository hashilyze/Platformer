using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPhys2D
{
    public class UPhysSystem2D : Utility.Singleton.SingletonComponent<UPhysSystem2D>
    {
        private static readonly HashSet<PlatformMovement2D> _platforms = new HashSet<PlatformMovement2D>();
        private static readonly HashSet<CharacterMovement2D> _characters = new HashSet<CharacterMovement2D>();

        // Platform(Movable solid block) Collecting
        public static void RegisterPlatform(PlatformMovement2D platform)
        {
            _platforms.Add(platform);
        }
        public static void UnregisterPlatform(PlatformMovement2D platform)
        {
            _platforms.Remove(platform);
        }
        // Character Collecting
        public static void RegisterCharacter (CharacterMovement2D character)
        {
            _characters.Add(character);
        }
        public static void UnregisterCharacter (CharacterMovement2D character)
        {
            _characters.Remove(character);
        }
        // Simulation (Integrated Update)
        private void FixedUpdate ()
        {
            float deltaTime = Time.fixedDeltaTime;
            if (!CanSimulate(deltaTime))
            {
                return;
            }
            Simulate(deltaTime);
        }

        private bool CanSimulate(float deltaTime)
        {
            return deltaTime > 0.0f;
        }

        private void Simulate(float deltaTime)
        {
            foreach(PlatformMovement2D platform in _platforms)
            {
                platform.Simulate(deltaTime);
            }
            foreach(CharacterMovement2D character in _characters)
            {
                character.Simulate(deltaTime);
            }
        }

        protected override void InitializeInstance () 
        {
            this.hideFlags = HideFlags.NotEditable;
            this.gameObject.hideFlags = HideFlags.NotEditable;
        }
    }
}