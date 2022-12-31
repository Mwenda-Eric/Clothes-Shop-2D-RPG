using System;
using System.Linq;
using Assets.HeroEditor.Common.CharacterScripts;
using Assets.HeroEditor.Common.ExampleScripts;
using HeroEditor.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.HeroEditor.Common.EditorScripts
{
    /// <summary>
    /// Character editor UI and behaviour.
    /// </summary>
    public class CharacterEditor : CharacterEditorBase
    {
        [Header("Other")]
        public FirearmCollection FirearmCollection;
        public bool UseEditorColorField = true;
        public string PrefabFolder;
        public string TestRoomSceneName;
	    
        private static Character _temp;

        /// <summary>
        /// Called automatically on app start.
        /// </summary>
        public void Awake()
        {
            RestoreTempCharacter();
        }

		/// <summary>
        /// Remove all equipment.
        /// </summary>
        public void Reset()
        {
            ((Character) Character).ResetEquipment();
            InitializeDropdowns();
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Save character to prefab.
        /// </summary>
        public void Save()
        {
            PrefabFolder = UnityEditor.EditorUtility.SaveFilePanel("Save character prefab", PrefabFolder, "New character", "prefab");

	        if (PrefabFolder.Length > 0)
	        {
		        Save("Assets" + PrefabFolder.Replace(Application.dataPath, null));
	        }
		}

	    /// <summary>
		/// Load character from prefab.
		/// </summary>
		public void Load()
        {
	        PrefabFolder = UnityEditor.EditorUtility.OpenFilePanel("Load character prefab", PrefabFolder, "prefab");

            if (PrefabFolder.Length > 0)
            {
                Load("Assets" + PrefabFolder.Replace(Application.dataPath, null));
            }

			//FeatureTip();
		}

	    /// <summary>
	    /// Save character to json.
	    /// </summary>
	    public void SaveToJson()
	    {
		    PrefabFolder = UnityEditor.EditorUtility.SaveFilePanel("Save character to json", PrefabFolder, "New character", "json");

		    if (PrefabFolder.Length > 0)
		    {
			    var path = "Assets" + PrefabFolder.Replace(Application.dataPath, null);
			    var json = Character.ToJson();

			    System.IO.File.WriteAllText(path, json);
			    Debug.LogFormat("Json saved to {0}: {1}", path, json);
		    }

		    //FeatureTip();
		}

		/// <summary>
		/// Load character from json.
		/// </summary>
		public void LoadFromJson()
	    {
		    PrefabFolder = UnityEditor.EditorUtility.OpenFilePanel("Load character from json", PrefabFolder, "json");

		    if (PrefabFolder.Length > 0)
		    {
				var path = "Assets" + PrefabFolder.Replace(Application.dataPath, null);
			    var json = System.IO.File.ReadAllText(path);

				Character.LoadFromJson(json);
			}

		    //FeatureTip();
	    }

		public override void Save(string path)
		{
			Character.transform.localScale = Vector3.one;

			#if UNITY_2018_3_OR_NEWER

			UnityEditor.PrefabUtility.SaveAsPrefabAsset(Character.gameObject, path);

			#else

			UnityEditor.PrefabUtility.CreatePrefab(path, Character.gameObject);

			#endif

            Debug.LogFormat("Prefab saved as {0}", path);
        }

        public override void Load(string path)
        {
			var character = UnityEditor.AssetDatabase.LoadAssetAtPath<Character>(path);

	        Character.GetComponent<Character>().Firearm.Params = character.Firearm.Params; // TODO: Workaround
			Load(character);
			FindObjectOfType<CharacterBodySculptor>().OnCharacterLoaded(character);
        }

	    #else

        public override void Save(string path)
        {
            throw new NotSupportedException();
        }

        public override void Load(string path)
        {
            throw new NotSupportedException();
        }

        #endif

        /// <summary>
        /// Test character with demo setup.
        /// </summary>
        public void Test()
        {
            #if UNITY_EDITOR

            if (UnityEditor.EditorBuildSettings.scenes.All(i => !i.path.Contains(TestRoomSceneName)))
            {
	            UnityEditor.EditorUtility.DisplayDialog("Hero Editor", string.Format("Please add '{0}.scene' to Build Settings!", TestRoomSceneName), "OK");
				return;
            }

            #endif

            var controller = Character.gameObject.AddComponent<CharacterController>();

            controller.center = new Vector3(0, 1.125f);
            controller.height = 2.5f;
	        controller.radius = 0.75f;
            Character.GetComponent<WeaponControls>().enabled = true;
            Character.gameObject.AddComponent<CharacterControl>();
            DontDestroyOnLoad(Character);
            _temp = Character as Character;
            SceneManager.LoadScene(TestRoomSceneName);

	        //FeatureTip();
		}

		protected override void SetFirearmParams(SpriteGroupEntry entry)
        {
            if (entry == null) return;

            if (FirearmCollection.Firearms.Count(i => i.Name == entry.Name) != 1) throw new Exception("Please check firearms params for: " + entry.Name);

			((Character) Character).Firearm.Params = FirearmCollection.Firearms.Single(i => i.Name == entry.Name);
		}

	    protected override void OpenPalette(GameObject palette, Color selected)
        {
            #if UNITY_EDITOR_WIN

            if (UseEditorColorField)
            {
                EditorColorField.Open(selected);
            }
            else

            #endif

            {
                Editor.SetActive(false);
                palette.SetActive(true);
            }
        }

        private void RestoreTempCharacter()
        {
            if (_temp == null) return;

	        Character.GetComponent<Character>().Firearm.Params = _temp.Firearm.Params; // TODO: Workaround
			Load(_temp);
	        FindObjectOfType<CharacterBodySculptor>().OnCharacterLoaded(_temp);
			Destroy(_temp.gameObject);
            _temp = null;
        }

	    protected override void FeedbackTip()
	    {
			#if UNITY_EDITOR

		    var success = UnityEditor.EditorUtility.DisplayDialog("Hero Editor", "Hi! Thank you for using my asset! I hope you enjoy making your game with it. The only thing I would ask you to do is to leave a review on the Asset Store. It would be awasome support for my asset, thanks!", "Review", "Later");
			
			RequestFeedbackResult(success, false);

			#endif
	    }

	    private void FeatureTip()
	    {
			#if UNITY_EDITOR

		    if (UnityEditor.EditorUtility.DisplayDialog("Hero Editor", "This feature is available only in PRO asset version!", "Navigate", "Cancel"))
		    {
			    Application.OpenURL(LinkToProVersion);
		    }

			#endif
		}
    }
}