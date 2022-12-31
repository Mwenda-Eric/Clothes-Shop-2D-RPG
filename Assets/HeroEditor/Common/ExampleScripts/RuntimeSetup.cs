using System.Linq;
using Assets.HeroEditor.Common.CharacterScripts;
using Assets.HeroEditor.Common.EditorScripts;
using HeroEditor.Common;
using UnityEngine;

namespace Assets.HeroEditor.Common.ExampleScripts
{
	/// <summary>
	/// Changing equipment at runtime examples.
	/// </summary>
	public class RuntimeSetup : MonoBehaviour
	{
		public Character Character;

		/// <summary>
		/// Example call: SetBody("HeadScar", "Basic", "Human", "Basic");
		/// </summary>
		public void SetBody(string headName, string headCollection, string bodyName, string bodyCollection)
		{
			var head = SpriteCollection.Instance.Head.Single(i => i.Name == headName && i.Collection == headCollection);
			var body = SpriteCollection.Instance.Body.Single(i => i.Name == bodyName && i.Collection == bodyCollection);

			Character.SetBody(head.Sprite, body.Sprites);
		}

		public void EquipMeleeWeapon1H(string sname, string collection)
		{
			var entry = SpriteCollection.Instance.MeleeWeapon1H.Single(i => i.Name == sname && i.Collection == collection);
			
			Character.EquipMeleeWeapon(entry.Sprite, entry.LinkedSprite);
		}

		public void EquipMeleeWeapon2H(string sname, string collection)
		{
			var entry = SpriteCollection.Instance.MeleeWeapon2H.Single(i => i.Name == sname && i.Collection == collection);

			Character.EquipMeleeWeapon(entry.Sprite, entry.LinkedSprite, twoHanded: true);
		}

		public void EquipBow(string sname, string collection)
		{
			var sprites = SpriteCollection.Instance.Bow.Single(i => i.Name == sname && i.Collection == collection).Sprites;

			Character.EquipBow(sprites);
		}

		public void EquipFirearm1H(string sname, string collection)
		{
			var sprites = SpriteCollection.Instance.Firearms1H.Single(i => i.Name == sname && i.Collection == collection).Sprites;
			var firearmParams = FirearmCollection.Instance.Firearms.Single(i => i.Name == sname);

			Character.EquipFirearm(sprites, firearmParams);
		}

		public void EquipShield(string sname, string collection)
		{
			var sprite = SpriteCollection.Instance.Shield.Single(i => i.Name == sname && i.Collection == collection).Sprite;

			Character.EquipShield(sprite);
		}

		public void EquipArmor(string sname, string collection)
		{
			var sprites = SpriteCollection.Instance.Armor.Single(i => i.Name == sname && i.Collection == collection).Sprites;

			Character.EquipArmor(sprites);
		}

		public void EquipHelmet(string sname, string collection)
		{
			var sprite = SpriteCollection.Instance.Helmet.Single(i => i.Name == sname && i.Collection == collection).Sprite;

			Character.EquipHelmet(sprite);
		}
	}
}