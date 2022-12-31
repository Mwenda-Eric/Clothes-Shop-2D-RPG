using System.Collections;
using Assets.HeroEditor.Common.Enums;
using Assets.MilitaryHeroes.Scripts.Enums;
using UnityEngine;

namespace Assets.HeroEditor.Common.CharacterScripts
{
    /// <summary>
    /// Firearm reload process.
    /// </summary>
    public class FirearmReload : MonoBehaviour
    {
        public Character Character;
        public AudioSource AudioSource;

        /// <summary>
        /// Should be set outside (by input manager or AI).
        /// </summary>
        [HideInInspector] public bool ReloadButtonDown;
        [HideInInspector] public bool Reloading;

        public void Update()
        {
            if (ReloadButtonDown && !Reloading && Character.Firearm.AmmoShooted > 0)
            {
                StartCoroutine(Reload());
            }
        }

        public IEnumerator Reload()
        {
            var firearm = Character.Firearm;
            var clip = firearm.Params.ReloadAnimation;
            var duration = firearm.Params.MagazineType == MagazineType.Removable ? clip.length : clip.length * firearm.AmmoShooted;

            Reloading = true;
            Character.Animator.SetBool("Reloading", true);

            switch (Character.Firearm.Params.LoadType)
            {
	            case FirearmLoadType.Drum:
		            for (var i = 0; i < Character.Firearm.AmmoShooted; i++)
		            {
			            Character.Firearm.Fire.CreateShell();
		            }

		            break;
	            case FirearmLoadType.Lamp:
		            Character.Firearm.Fire.SetLamp(Character.Firearm.Params.GetColorFromMeta("LampReload"));
					break;
            }

            yield return new WaitForSeconds(duration);

	        if (Character.Firearm.Params.LoadType == FirearmLoadType.Lamp)
	        {
		        Character.Firearm.Fire.SetLamp(Character.Firearm.Params.GetColorFromMeta("LampReady"));
			}

			firearm.AmmoShooted = 0;
            Character.Animator.SetBool("Reloading", false);
            Character.Animator.SetInteger("HoldType", (int) firearm.Params.HoldType);
            Reloading = false;
        }

        public void PlayAudioEffect()
        {
            AudioSource.Play();
        }
    }
}