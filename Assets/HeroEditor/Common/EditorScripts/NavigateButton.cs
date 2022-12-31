using UnityEngine;

namespace Assets.HeroEditor.Common.EditorScripts
{
	public class NavigateButton : MonoBehaviour
	{
		public void Navigate(string url)
		{
			Application.OpenURL(url);
		}
	}
}