using System;
using Assets.HeroEditor.Common.CharacterScripts;
using UnityEngine;

namespace Assets.HeroEditor.Common.ExampleScripts
{
    /// <summary>
    /// Character move and jump example. Built-in component CharacterController (3D) is used. It can be raplaced by 2D colliders.
    /// </summary>
    public class CharacterControl : MonoBehaviour
    {
        public KeyCode LeftButton = KeyCode.LeftArrow;
        public KeyCode RightButton = KeyCode.RightArrow;
        public KeyCode JumpButton = KeyCode.Space;

        private Vector3 _speed = Vector3.zero;
        private Character _character;
        private CharacterController _controller; // https://docs.unity3d.com/ScriptReference/CharacterController.html

		public void Start()
        {
            _character = GetComponent<Character>();
            _character.Animator.SetBool("Ready", true);
            _controller = GetComponent<CharacterController>();
        }
 
        public void Update()
        {
            Move(Input.GetKey(LeftButton), Input.GetKey(RightButton), Input.GetKey(JumpButton));
        }

        public void Move(bool left, bool right, bool jump)
        {
            if (_controller.isGrounded)
            {
                _speed = Vector3.zero;

                if (left) _speed.x = -5;
                if (right) _speed.x = 5;
                if (jump) _speed.y = 10;
                if (_speed.magnitude > 0) Turn(_speed.x);
            }

            _character.Animator.SetBool("Run", _controller.isGrounded && Math.Abs(_speed.x) > 0.01f); // Go to animator transitions for more details
            _character.Animator.SetBool("Jump", !_controller.isGrounded);

            _speed.y -= 25 * Time.deltaTime; // Depends on project physics settings
            _controller.Move(_speed * Time.deltaTime);
        }

        public void Turn(float direction)
        {
            if (direction * transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(direction), 1, 1);
            }
        }
    }
}