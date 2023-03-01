using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace UPhys2D
{
    public class UPhysSystem2D : SingletonComponent<UPhysSystem2D>
    {
        public static void RegisterPlatform(PlatformMovement2D platform)
        {
            if (_platforms.Contains(platform))
            {
                return;
            }
            _platforms.Add(platform);
        }
        public static void UnregisterPlatform(PlatformMovement2D platform)
        {
            _platforms.Remove(platform);
        }

        public static void RegisterCharacter (CharacterMovement2D character)
        {
            if (_characters.Contains(character))
            {
                return;
            }
            _characters.Add(character);
        }
        public static void UnregisterCharacter (CharacterMovement2D character)
        {
            _characters.Remove(character);
        }


        private static readonly List<PlatformMovement2D> _platforms = new List<PlatformMovement2D>();
        private static readonly List<CharacterMovement2D> _characters = new List<CharacterMovement2D>();

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
            for(int beg = 0, end = _platforms.Count; beg < end; beg++)
            {
                PlatformMovement2D platform = _platforms[beg];
                platform.Simulate(deltaTime);
            }
            for (int beg = 0, end = _characters.Count; beg < end; beg++)
            {
                CharacterMovement2D character = _characters[beg];
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