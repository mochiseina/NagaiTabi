using Naninovel;
using UnityEngine;

public class NaninovelBridge : MonoBehaviour
{
	public void PlayScript(string scriptName)
	{
		if (!Engine.Initialized)
		{
			Debug.LogWarning("Naninovel no está inicializado todavía.");
			return;

			var player = Engine.GetService<IScriptPlayer>();
			player.MainTrack.LoadAndPlay(scriptName).Forget();
		}
	}
}