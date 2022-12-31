﻿using Assets.HeroEditor.Common.CharacterScripts;
using HeroEditor.Common;
using UnityEngine;

namespace Assets.HeroEditor.FantasyHeroes.TestRoom.Scarecrow
{
    /// <summary>
    /// Enemy example behaviour.
    /// </summary>
    public class Scarecrow : MonoBehaviour
    {
        public Animator Animator;
        public Character Character;

        public void Start()
        {
            Character = FindObjectOfType<Character>();

            if (Character != null)
            {
                Character.Animator.GetComponent<AnimationEvents>().OnCustomEvent += OnAnimationEvent;
            }
        }

        public void OnDestroy()
        {
            if (Character != null)
            {
                Character.Animator.GetComponent<AnimationEvents>().OnCustomEvent -= OnAnimationEvent;
            }
        }

        private void OnAnimationEvent(string eventName)
        {
            if (eventName == "Hit" && Vector2.Distance(Character.MeleeWeapon.Edge.position, transform.position) < 1.5)
            {
                Animator.SetTrigger("Impact");
            }
        }
    }
}