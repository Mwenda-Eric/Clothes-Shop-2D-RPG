using System;
using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor.Common.Data;
using Assets.HeroEditor.Common.EditorScripts;
using Assets.HeroEditor.Common.Enums;
using HeroEditor.Common;
using HeroEditor.Common.Enums;
using UnityEngine;

namespace Assets.HeroEditor.Common.CharacterScripts
{
    /// <summary>
    /// Character presentation in editor. Contains sprites, renderers, animation and so on.
    /// </summary>
    public partial class Character : CharacterBase
    {
        [Header("Weapons")]
        public MeleeWeapon MeleeWeapon;
        public Firearm Firearm;
        public BowShooting BowShooting;

	    [Header("Service")]
		public LayerManager LayerManager;

	    public Vector2 BodyScale
	    {
		    get { return BodyRenderers.Single(i => i.name == "Torso").transform.localScale; }
		    set { FindObjectOfType<CharacterBodySculptor>().OnCharacterLoaded(value); }
	    }

	    /// <summary>
		/// Called automatically when something was changed.
		/// </summary>
		public void OnValidate()
        {
            if (Head == null) return;

            Initialize();
        }
        
        /// <summary>
        /// Called automatically when object was enabled.
        /// </summary>
        public void OnEnable()
        {
            HairMask.isCustomRangeActive = true;
            HairMask.frontSortingOrder = HelmetRenderer.sortingOrder;
            HairMask.backSortingOrder = HairRenderer.sortingOrder;
			UpdateAnimation();
        }

	    public void OnDisable()
	    {
		    _animationState = -1;
	    }

	    private int _animationState = -1;

		/// <summary>
		/// Refer to Animator window to learn animation params, states and transitions!
		/// </summary>
		public override void UpdateAnimation()
        {
	        if (!Animator.isInitialized) return;

			var state = 100 * (int) WeaponType;

			Animator.SetInteger("WeaponType", (int) WeaponType);

	        if (WeaponType == WeaponType.Firearms1H || WeaponType == WeaponType.Firearms2H || WeaponType == WeaponType.FirearmsPaired)
	        {
		        Animator.SetInteger("MagazineType", (int)Firearm.Params.MagazineType);
		        Animator.SetInteger("HoldType", (int)Firearm.Params.HoldType);
		        state += (int) Firearm.Params.HoldType;
	        }

	        if (state == _animationState) return; // No need to change animation.

	        _animationState = state;
			Animator.SetBool("Ready", true);
            Animator.SetBool("Stand", true);

	        if (WeaponType == WeaponType.Firearms1H || WeaponType == WeaponType.Firearms2H)
	        {
		        Animator.Play("IdleFirearm", 0); // Upper body
			}
	        else
	        {
				Animator.Play("IdleMelee", 0); // Upper body
			}

            Animator.Play("Stand", 1); // Lower body
        }

        /// <summary>
        /// Initializes character renderers with selected sprites.
        /// </summary>
        public override void Initialize()
        {
			try // Disable try/catch for debugging.
            {
                TryInitialize();
            }
            catch (Exception e)
            {
                Debug.LogWarningFormat("Unable to initialize character {0}: {1}", name, e.Message);
            }
        }

		/// <summary>
		/// Set character's expression.
		/// </summary>
	    public void SetExpression(string expression)
		{
			if (Expressions.Count < 3) throw new Exception("Character must have at least 3 basic expressions: Default, Angry and Dead.");
			
			var e = Expressions.Single(i => i.Name == expression);

			Expression = expression;
			EyebrowsRenderer.sprite = e.Eyebrows;
			EyesRenderer.sprite = e.Eyes;
			MouthRenderer.sprite = e.Mouth;

			if (EyebrowsRenderer.sprite == null) EyebrowsRenderer.sprite = Expressions[0].Eyebrows;
			if (EyesRenderer.sprite == null) EyesRenderer.sprite = Expressions[0].Eyes;
			if (MouthRenderer.sprite == null) MouthRenderer.sprite = Expressions[0].Mouth;
		}

	    /// <summary>
	    /// Set character's body.
	    /// </summary>
	    public void SetBody(Sprite head, List<Sprite> body)
	    {
		    Head = head;
		    Body = body;
			Initialize();
		}

		#region Equipment

		/// <summary>
		/// Remove all equipment.
		/// </summary>
		public void ResetEquipment()
	    {
		    for (var i = 0; i < Armor.Count; i++)
		    {
			    Armor[i] = null;
		    }

		    for (var i = 0; i < Bow.Count; i++)
		    {
			    Bow[i] = null;
		    }

		    Helmet = Cape = Back = PrimaryMeleeWeapon = SecondaryMeleeWeapon = Shield = null;
		    Initialize();
	    }

		/// <summary>
		/// Equip melee weapon.
		/// </summary>
		/// <param name="sprite">Weapon sprite. It can be obtained from SpriteCollection.Instance.MeleeWeapon1H/2H[].Sprites.</param>
		/// <param name="trail">Melee weapon trail. It is LinkedSprite of SpriteGroupEntry.</param>
		/// <param name="twoHanded">If two-handed melee weapon.</param>
		public void EquipMeleeWeapon(Sprite sprite, Sprite trail, bool twoHanded = false)
	    {
		    PrimaryMeleeWeapon = sprite;
		    PrimaryMeleeWeaponTrailRenderer.sprite = trail;
			WeaponType = twoHanded ? WeaponType.Melee2H : WeaponType.Melee1H;
		    Initialize();
	    }
	   
	    /// <summary>
	    /// Equip paired melee weapons.
	    /// </summary>
	    public void EquipMeleeWeaponPaired(Sprite spritePrimary, Sprite trailPrimary, Sprite spriteSecondary, Sprite trailSecondary)
	    {
		    PrimaryMeleeWeapon = spritePrimary;
		    PrimaryMeleeWeaponTrailRenderer.sprite = trailPrimary;
			SecondaryMeleeWeapon = spriteSecondary;
		    SecondaryMeleeWeaponTrailRenderer.sprite = trailSecondary;
			WeaponType = WeaponType.MeleePaired;
		    Initialize();
	    }

		/// <summary>
		/// Equip bow.
		/// </summary>
		/// <param name="sprites">A list of sprites from bow atlas (multiple sprite). It can be obtained from SpriteCollection.Instance.Bow[].Sprites.</param>
		public void EquipBow(List<Sprite> sprites)
	    {
		    Bow = sprites;
		    WeaponType = WeaponType.Bow;
			Initialize();
	    }

		/// <summary>
		/// Equip firearm.
		/// </summary>
		/// <param name="sprites">A list of sprites from armor atlas (multiple sprite). It can be obtained from SpriteCollection.Instance.Firearms1H/2H[].Sprites.</param>
		/// <param name="firearmParams">Firearm params. Can be obtained from FirearmeCollection.Instance.Firearms[].</param>
		/// <param name="twoHanded">If two-handed firearm.</param>
		public void EquipFirearm(List<Sprite> sprites, FirearmParams firearmParams, bool twoHanded = false)
	    {
			Firearms = sprites;
		    Firearm.Params = firearmParams;
		    WeaponType = twoHanded ? WeaponType.Firearms2H : WeaponType.Firearms1H;
			Initialize();
		}

		/// <summary>
		/// Equip shield.
		/// </summary>
		/// <param name="sprite">Shield sprite. It can be obtained from SpriteCollection.Instance.Shield[].Sprite.</param>
		public void EquipShield(Sprite sprite)
	    {
		    Shield = sprite;
		    WeaponType = WeaponType.Melee1H;
		    Initialize();
	    }

		/// <summary>
		/// Equip helmet.
		/// </summary>
		/// <param name="sprite">Helmet sprite. It can be obtained from SpriteCollection.Instance.Helmet[].Sprite.</param>
		public void EquipHelmet(Sprite sprite)
	    {
		    Helmet = sprite;
		    Initialize();
	    }

		/// <summary>
		/// Equip armor.
		/// </summary>
		/// <param name="sprites">A list of sprites from armor atlas (multiple sprite). It can be obtained from SpriteCollection.Instance.Armor[].Sprites.</param>
		public void EquipArmor(List<Sprite> sprites)
		{
			Armor = sprites;
			Initialize();
		}

		/// <summary>
		/// Equip armor.
		/// </summary>
		/// <param name="sprites">A list of sprites from armor atlas (multiple sprite). It can be obtained from SpriteCollection.Instance.Armor[].Sprites.</param>
		public void EquipUpperArmor(List<Sprite> sprites)
		{
			foreach (var part in new[] { "ArmL", "ArmR", "Finger", "ForearmL", "ForearmR", "HandL", "HandR", "SleeveR", "Torso" })
			{
				SetArmorPart(part, sprites);
			}

			Initialize();
		}

		/// <summary>
		/// Equip lower armor.
		/// </summary>
		/// <param name="sprites">A list of sprites from armor atlas (multiple sprite). It can be obtained from SpriteCollection.Instance.Armor[].Sprites.</param>
		public void EquipLowerArmor(List<Sprite> sprites)
		{
			foreach (var part in new[] { "Leg", "Pelvis", "Shin" })
			{
				SetArmorPart(part, sprites);
			}

			Initialize();
		}

		/// <summary>
		/// Equip body armor.
		/// </summary>
		/// <param name="sprites">A list of sprites from armor atlas (multiple sprite). It can be obtained from SpriteCollection.Instance.Armor[].Sprites.</param>
		public void EquipBodyArmor(List<Sprite> sprites)
		{
			foreach (var part in new[] { "ArmL", "ArmR", "Torso", "Pelvis" })
			{
				SetArmorPart(part, sprites);
			}

			Initialize();
		}

		/// <summary>
		/// Equip gloves.
		/// </summary>
		/// <param name="sprites">A list of sprites from armor atlas (multiple sprite). It can be obtained from SpriteCollection.Instance.Armor[].Sprites.</param>
		public void EquipGloves(List<Sprite> sprites)
		{
			foreach (var part in new[] { "ForearmL", "ForearmR", "HandL", "HandR", "SleeveR", "Finger" })
			{
				SetArmorPart(part, sprites);
			}

			Initialize();
		}

		/// <summary>
		/// Equip boots.
		/// </summary>
		/// <param name="sprites">A list of sprites from armor atlas (multiple sprite). It can be obtained from SpriteCollection.Instance.Armor[].Sprites.</param>
		public void EquipBoots(List<Sprite> sprites)
		{
			foreach (var part in new[] { "Shin" })
			{
				SetArmorPart(part, sprites);
			}

			Initialize();
		}

		private void SetArmorPart(string part, List<Sprite> armor)
	    {
		    var sprite = armor.Single(j => j.name == part);

		    Armor.RemoveAll(i => i == null);

		    for (var i = 0; i < Armor.Count; i++)
		    {
			    if (Armor[i] != null && Armor[i].name == part)
			    {
				    Armor[i] = sprite;
				    return;
			    }
		    }

		    Armor.Add(sprite);
	    }

		#endregion

	    /// <summary>
		/// Initializes character renderers with selected sprites.
		/// </summary>
		private void TryInitialize()
        {
			if (Expressions.All(i => i.Name != "Default") || Expressions.All(i => i.Name != "Angry") || Expressions.All(i => i.Name != "Dead"))
				throw new Exception("Character must have at least 3 basic expressions: Default, Angry and Dead.");

			HeadRenderer.sprite = Head;
            HairRenderer.sprite = Hair;
            HairRenderer.maskInteraction = Helmet == null || Helmet.name.Contains("[FullHair]") ? SpriteMaskInteraction.None : SpriteMaskInteraction.VisibleInsideMask;
            EarsRenderer.sprite = Ears;
			SetExpression(Expression);
			BeardRenderer.sprite = Beard;
            MapSprites(BodyRenderers, Body);
            HelmetRenderer.sprite = Helmet;
            GlassesRenderer.sprite = Glasses;
            MaskRenderer.sprite = Mask;
	        EarringsRenderer.sprite = Earrings;
			MapSprites(ArmorRenderers, Armor);
            CapeRenderer.sprite = Cape;
            BackRenderer.sprite = Back;
            PrimaryMeleeWeaponRenderer.sprite = PrimaryMeleeWeapon;
            SecondaryMeleeWeaponRenderer.sprite = SecondaryMeleeWeapon;
            BowRenderers.ForEach(i => i.sprite = Bow.SingleOrDefault(j => j != null && i.name.Contains(j.name)));
            FirearmsRenderers.ForEach(i => i.sprite = Firearms.SingleOrDefault(j => j != null && i.name.Contains(j.name)));
            ShieldRenderer.sprite = Shield;

            PrimaryMeleeWeaponRenderer.enabled = WeaponType != WeaponType.Bow;
            SecondaryMeleeWeaponRenderer.enabled = WeaponType == WeaponType.MeleePaired;
            BowRenderers.ForEach(i => i.enabled = WeaponType == WeaponType.Bow);
            
            if (Hair != null && Hair.name.Contains("[HideEars]") && HairRenderer.maskInteraction == SpriteMaskInteraction.None)
            {
                EarsRenderer.sprite = null;
            }

            switch (WeaponType)
            {
                case WeaponType.Firearms1H:
                case WeaponType.Firearms2H:
                    Firearm.AmmoShooted = 0;
	                BuildFirearms(Firearm.Params);
					break;
            }
		}

	    private void BuildFirearms(FirearmParams firearmParams)
	    {
		    Firearm.Params = firearmParams; // TODO:
		    Firearm.SlideTransform.localPosition = firearmParams.SlidePosition;
		    Firearm.MagazineTransform.localPosition = firearmParams.MagazinePosition;
		    Firearm.FireTransform.localPosition = firearmParams.FireMuzzlePosition;
		    Firearm.AmmoShooted = 0;

		    if (Firearm.Params.LoadType == FirearmLoadType.Lamp)
		    {
			    Firearm.Fire.SetLamp(firearmParams.GetColorFromMeta("LampReady"));
		    }
	    }

		private void MapSprites(List<SpriteRenderer> spriteRenderers, List<Sprite> sprites)
        {
            foreach (var part in spriteRenderers)
            {
                part.sprite = sprites.SingleOrDefault(i => i != null && i.name == part.name.Split('[')[0]);
            }
        }
    }
}