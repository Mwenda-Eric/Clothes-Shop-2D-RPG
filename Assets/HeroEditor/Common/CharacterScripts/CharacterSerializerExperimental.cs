using System;
using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor.Common.Data;
using Assets.HeroEditor.Common.EditorScripts;
using HeroEditor.Common;
using HeroEditor.Common.Data;
using HeroEditor.Common.Enums;
using UnityEngine;

namespace Assets.HeroEditor.Common.CharacterScripts
{
	public partial class Character
	{
		public override string ToJson()
		{
			var sc = SpriteCollection.Instance;

			if (sc == null) throw new Exception("SpriteCollection is missed on scene!");

			var description = new SerializableDictionary<string, string>
			{
				{ "Head", SpriteToString(sc.Head, HeadRenderer) },
				{ "Body", SpriteToString(sc.Body, BodyRenderers[0]) },
				{ "Ears", SpriteToString(sc.Ears, EarsRenderer) },
				{ "Hair", SpriteToString(sc.Hair, HairRenderer) },
				{ "Beard", SpriteToString(sc.Beard, BeardRenderer) },
				{ "Helmet", SpriteToString(sc.Helmet, HelmetRenderer) },
				{ "Glasses", SpriteToString(sc.Glasses, GlassesRenderer) },
				{ "Mask", SpriteToString(sc.Mask, MaskRenderer) },
				{ "Armor", SpriteToString(sc.Armor, ArmorRenderers[0]) },
				{ "PrimaryMeleeWeapon", SpriteToString(GetWeaponCollection(WeaponType), PrimaryMeleeWeaponRenderer) },
				{ "SecondaryMeleeWeapon", SpriteToString(GetWeaponCollection(WeaponType), SecondaryMeleeWeaponRenderer) },
				{ "Cape", SpriteToString(sc.Cape, CapeRenderer) },
				{ "Back", SpriteToString(sc.Back, BackRenderer) },
				{ "Shield", SpriteToString(sc.Shield, ShieldRenderer) },
				{ "Bow", SpriteToString(sc.Bow, BowRenderers[0]) },
				{ "Firearms", SpriteToString(GetWeaponCollection(WeaponType), FirearmsRenderers[0]) },
				{ "FirearmParams", Firearm.Params.Name },
				{ "WeaponType", WeaponType.ToString() },
				{ "Expression", Expression },
				{ "BodyScale", JsonUtility.ToJson(BodyScale) }
			};

			foreach (var expression in Expressions)
			{
				description.Add(string.Format("Expression.{0}.Eyebrows", expression.Name), SpriteToString(sc.Eyebrows, expression.Eyebrows, EyebrowsRenderer.color));
				description.Add(string.Format("Expression.{0}.Eyes", expression.Name), SpriteToString(sc.Eyes, expression.Eyes, EyesRenderer.color));
				description.Add(string.Format("Expression.{0}.Mouth", expression.Name), SpriteToString(sc.Mouth, expression.Mouth, MouthRenderer.color));
			}

			return JsonUtility.ToJson(description);
		}

		public override void LoadFromJson(string serialized)
		{
			var description = JsonUtility.FromJson<SerializableDictionary<string, string>>(serialized);
			var sc = SpriteCollection.Instance;

			if (sc == null) throw new Exception("SpriteCollection is missed on scene!");

			RestoreFromString(ref Head, HeadRenderer, sc.Head, description["Head"]);
			RestoreFromString(ref Body, BodyRenderers, sc.Body, description["Body"]);
			RestoreFromString(ref Ears, EarsRenderer, sc.Ears, description["Ears"]);
			RestoreFromString(ref Hair, HairRenderer, sc.Hair, description["Hair"]);
			RestoreFromString(ref Beard, BeardRenderer, sc.Beard, description["Beard"]);
			RestoreFromString(ref Helmet, HelmetRenderer, sc.Helmet, description["Helmet"]);
			RestoreFromString(ref Glasses, GlassesRenderer, sc.Glasses, description["Glasses"]);
			RestoreFromString(ref Mask, MaskRenderer, sc.Mask, description["Mask"]);
			RestoreFromString(ref Armor, ArmorRenderers, sc.Armor, description["Armor"]);
			WeaponType = (WeaponType) Enum.Parse(typeof(WeaponType), description["WeaponType"]);
			RestoreFromString(ref PrimaryMeleeWeapon, PrimaryMeleeWeaponRenderer, GetWeaponCollection(WeaponType), description["PrimaryMeleeWeapon"]);
			RestoreFromString(ref SecondaryMeleeWeapon, SecondaryMeleeWeaponRenderer, GetWeaponCollection(WeaponType), description["SecondaryMeleeWeapon"]);
			RestoreFromString(ref Cape, CapeRenderer, sc.Cape, description["Cape"]);
			RestoreFromString(ref Back, BackRenderer, sc.Back, description["Back"]);
			RestoreFromString(ref Shield, ShieldRenderer, sc.Shield, description["Shield"]);
			RestoreFromString(ref Bow, BowRenderers, sc.Bow, description["Bow"]);
			RestoreFromString(ref Firearms, FirearmsRenderers, GetWeaponCollection(WeaponType), description["Firearms"]);
			Expression = description["Expression"];
			Expressions = new List<Expression>();

			if (string.IsNullOrEmpty(description["FirearmParams"]))
			{
				Firearm.Params = new FirearmParams();
			}
			else
			{
				if (FirearmCollection.Instance == null) throw new Exception("FirearmCollection is missed on scene!");

				var firearmParams = FirearmCollection.Instance.Firearms.Single(i => i.Name == description["FirearmParams"]);

				if (firearmParams == null) throw new Exception(string.Format("FirearmCollection doesn't contain a definition for {0}!", description["FirearmParams"]));

				Firearm.Params = firearmParams;
			}

			foreach (var key in description.Keys)
			{
				if (key.Contains("Expression."))
				{
					var parts = key.Split('.');
					var expressionName = parts[1];
					var expressionPart = parts[2];
					var expression = Expressions.SingleOrDefault(i => i.Name == expressionName);

					if (expression == null)
					{
						expression = new Expression { Name = expressionName };
						Expressions.Add(expression);
					}

					switch (expressionPart)
					{
						case "Eyebrows":
							RestoreFromString(ref expression.Eyebrows, EyebrowsRenderer, sc.Eyebrows, description[key]);
							break;
						case "Eyes":
							RestoreFromString(ref expression.Eyes, EyesRenderer, sc.Eyes, description[key]);
							break;
						case "Mouth":
							RestoreFromString(ref expression.Mouth, MouthRenderer, sc.Mouth, description[key]);
							break;
						default:
							throw new NotSupportedException(expressionPart);
					}
				}
			}

			BodyScale = JsonUtility.FromJson<Vector2>(description["BodyScale"]);
			Initialize();
			UpdateAnimation();
		}

		private static IEnumerable<SpriteGroupEntry> GetWeaponCollection(WeaponType weaponType)
		{
			switch (weaponType)
			{
				case WeaponType.Melee1H: return SpriteCollection.Instance.MeleeWeapon1H;
				case WeaponType.MeleePaired: return SpriteCollection.Instance.MeleeWeapon1H;
				case WeaponType.Melee2H: return SpriteCollection.Instance.MeleeWeapon2H;
				case WeaponType.Bow: return SpriteCollection.Instance.Bow;
				case WeaponType.Firearms1H: return SpriteCollection.Instance.Firearms1H;
				case WeaponType.FirearmsPaired: return SpriteCollection.Instance.Firearms1H;
				case WeaponType.Firearms2H: return SpriteCollection.Instance.Firearms2H;
				case WeaponType.Supplies: return SpriteCollection.Instance.Supplies;
				default:
					throw new NotSupportedException(weaponType.ToString());
			}
		}

		private static string SpriteToString(IEnumerable<SpriteGroupEntry> collection, SpriteRenderer renderer)
		{
			return SpriteToString(collection, renderer.sprite, renderer.color);
		}

		private static string SpriteToString(IEnumerable<SpriteGroupEntry> collection, Sprite sprite, Color color)
		{
			if (sprite == null) return null;

			var entry = collection.SingleOrDefault(i => i.Sprite == sprite);

			if (entry == null) throw new Exception(string.Format("Can't find {0} in SpriteCollection.", sprite.name));

			var result = color == Color.white ? entry.Id : entry.Id + "#" + ColorUtility.ToHtmlStringRGBA(color);

			return result;
		}

		private static void RestoreFromString(ref Sprite sprite, SpriteRenderer renderer, IEnumerable<SpriteGroupEntry> collection, string serialized)
		{
			if (string.IsNullOrEmpty(serialized))
			{
				sprite = renderer.sprite = null;
				renderer.color = Color.white;
				return;
			}

			var parts = serialized.Split('#');
			var id = parts[0];
			var color = Color.white;

			if (parts.Length > 1)
			{
				ColorUtility.TryParseHtmlString("#" + parts[1], out color);
			}

			var entries = collection.Where(i => i.Id == id).ToList();

			switch (entries.Count)
			{
				case 1:
					sprite = entries[0].Sprite;
					renderer.color = color;
					break;
				case 0:
					throw new Exception(string.Format("Entry with id {0} not found in SpriteCollection.", id));
				default:
					throw new Exception(string.Format("Multiple entries with id {0} found in SpriteCollection.", id));
			}
		}

		private static void RestoreFromString(ref List<Sprite> sprites, List<SpriteRenderer> renderers, IEnumerable<SpriteGroupEntry> collection, string serialized)
		{
			if (string.IsNullOrEmpty(serialized))
			{
				sprites = new List<Sprite>();

				foreach (var renderer in renderers)
				{
					renderer.sprite = null;
					renderer.color = Color.white;
				}
				
				return;
			}

			var parts = serialized.Split('#');
			var id = parts[0];
			var color = Color.white;

			if (parts.Length > 1)
			{
				ColorUtility.TryParseHtmlString("#" + parts[1], out color);
			}

			var entries = collection.Where(i => i.Id == id).ToList();

			switch (entries.Count)
			{
				case 1:
					sprites = entries[0].Sprites;
					renderers.ForEach(i => i.color = color);
					break;
				case 0:
					throw new Exception(string.Format("Entry with id {0} not found in SpriteCollection.", id));
				default:
					throw new Exception(string.Format("Multiple entries with id {0} found in SpriteCollection.", id));
			}
		}
	}
}