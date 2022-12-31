using HeroEditor.Common.Tools;
using UnityEngine;

namespace Assets.HeroEditor.Common.EditorScripts
{
	public class PhotoMaker : MonoBehaviour
	{
		public ScreenshotTransparent ScreenshotTransparent;
	
		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.P))
			{
				MakePhoto();
			}
		}

		public void MakePhoto()
		{
			#if UNITY_EDITOR

			ScreenshotTransparent.Capture(ScreenshotTransparent.GetPath());

			#elif UNITY_ANDROID || UNITY_IOS

			Share();

			#endif
		}

		public void Share() // 3rd party asset required: NativeShare.
		{
			//var path = Path.Combine(Application.temporaryCachePath, "Photo.png");
			//const string shareText = "Created in Fantasy Heroes: Character Editor!";

			//File.Delete(path);
			//File.WriteAllBytes(path, ScreenshotTransparent.Capture().EncodeToPNG());
			//new NativeShare().SetSubject("Sharing my pixel art").SetTitle("Sharing").SetText(shareText).AddFile(path, "image/*").Share();
		}
	}
}