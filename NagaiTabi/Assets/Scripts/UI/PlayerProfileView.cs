using System.Collections;
using Naninovel;
using TMPro;
using UnityEngine;

public class PlayerProfileView : MonoBehaviour
{
	[SerializeField] private TrackerManager trackerManager;
	[SerializeField] private TextMeshProUGUI playerNameText;

	private IEnumerator Start()
	{
		while (!Engine.Initialized)
			yield return null;

		RefreshFromNaninovel();
	}

	public void RefreshFromNaninovel()
	{
		var vars = Engine.GetService<ICustomVariableManager>();
		var value = vars.GetVariableValue("PlayerName").String;

		if (string.IsNullOrWhiteSpace(value))
			value = "Sin nombre";

		if (trackerManager != null)
			trackerManager.Data.playerName = value;

		if (playerNameText != null)
			playerNameText.text = value;
	}

	public void ResetPlayerNameForTesting()
	{
		var vars = Engine.GetService<ICustomVariableManager>();
		vars.SetVariableValue("PlayerName", new(""));

		if (trackerManager != null)
			trackerManager.Data.playerName = "";

		if (playerNameText != null)
			playerNameText.text = "Sin nombre";
	}
}