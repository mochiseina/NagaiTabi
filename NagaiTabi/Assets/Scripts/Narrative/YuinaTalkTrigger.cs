using Naninovel;
using UnityEngine;

public class YuinaTalkTrigger : MonoBehaviour
{
	[ScriptAssetRef]
	public string ScriptRef;

	public string Label;

	public void TriggerTalk()
	{
		if (!Engine.Initialized) return;

		var player = Engine.GetService<IScriptPlayer>();
		var path = ScriptAssets.GetPath(ScriptRef);

		if (string.IsNullOrWhiteSpace(Label))
			player.MainTrack.LoadAndPlay(path).Forget();
		else
			player.MainTrack.LoadAndPlayAtLabel(path, Label).Forget();
	}
}