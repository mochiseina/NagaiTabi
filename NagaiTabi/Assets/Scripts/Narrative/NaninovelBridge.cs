using Naninovel;
using UnityEngine;

public class NaninovelBridge : MonoBehaviour
{
	public void PlayScript(string scriptName)
	{
		if (!Engine.Initialized)
		{
			Debug.LogWarning("[NaninovelBridge] Naninovel no está inicializado todavía.");
			return;
		}
		Debug.Log($"[NaninovelBridge] Intentando reproducir script: {scriptName}");
		var player = Engine.GetService<IScriptPlayer>();
		player.MainTrack.LoadAndPlay(scriptName).Forget();
	}
}