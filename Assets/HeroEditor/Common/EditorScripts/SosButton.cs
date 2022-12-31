using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.HeroEditor.Common.EditorScripts
{
	public class SosButton : MonoBehaviour
	{
		public void Navigate(Object scene)
		{
			 #if UNITY_EDITOR

            if (UnityEditor.EditorBuildSettings.scenes.All(i => !i.path.Contains(scene.name)))
            {
	            UnityEditor.EditorUtility.DisplayDialog("Hero Editor", string.Format("Please add '{0}.scene' to Build Settings!", scene.name), "OK");
				return;
            }

            #endif

			SceneManager.LoadScene(scene.name);
		}
	}
}