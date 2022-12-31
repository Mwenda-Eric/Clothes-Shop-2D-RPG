using System.Collections;
using Assets.HeroEditor.Common.CharacterScripts;
using HeroEditor.Common.Enums;
using UnityEngine;

namespace Assets.HeroEditor.Common.ExampleScripts
{
	/// <summary>
	/// Playing different animations example. For full list of animation params and states please open Animator window and select Human.controller.
	/// </summary>
	public class PlayingAnimations : MonoBehaviour
	{
		public Character Character;

		public void Reset()
		{
			Character.UpdateAnimation();
		}

		public void Idle()
		{
			Character.Animator.SetBool("Idle", true);
		}

		public void Ready()
		{
			Character.Animator.SetBool("Ready", true);
		}

		public void Walk()
		{
			Character.Animator.SetBool("Walk", true);
		}

		public void Run()
		{
			Character.Animator.SetBool("Run", true);
		}

		public void Jump()
		{
			Character.Animator.SetBool("Jump", true);
		}

		public void SlashMelee1H()
		{
			Character.WeaponType = WeaponType.Melee1H;
			Character.Animator.SetTrigger("Slash"); // Slash / Jab
		}

		public void JabMelee2H()
		{
			Character.WeaponType = WeaponType.Melee2H;
			Character.Animator.SetTrigger("Jab"); // Slash / Jab
		}

		public IEnumerator ShootBow()
		{
			Character.WeaponType = WeaponType.Bow;
			Character.Animator.SetInteger("Charge", 1); // 0 = ready, 1 = charging, 2 = release, 3 = cancel

			yield return new WaitForSeconds(1);

			Character.Animator.SetInteger("Charge", 2);

			yield return new WaitForSeconds(1);

			Character.Animator.SetInteger("Charge", 0);
		}
	}
}