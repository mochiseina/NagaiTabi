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
		if (!Engine.Initialized)
		{
			Debug.LogWarning("[PlayerProfileView] Engine aún no inicializado.");
			return;
		}

		var vars = Engine.GetService<ICustomVariableManager>();
		var value = vars.GetVariableValue("PlayerName").String;

		Debug.Log($"[PlayerProfileView] PlayerName leído desde Naninovel: '{value}'");

		if (string.IsNullOrWhiteSpace(value))
			value = "Sin nombre";

		if (trackerManager != null)
		{
			trackerManager.Data.playerName = value;

			// opcional pero útil: guardar el nombre también en tu JSON
			var saveService = trackerManager.GetComponent<SaveService>();
		}

		if (playerNameText != null)
			playerNameText.text = value;
	}

	public void ResetPlayerNameForTesting()
	{
		if (!Engine.Initialized) return;

		var vars = Engine.GetService<ICustomVariableManager>();
		vars.SetVariableValue("PlayerName", new(""));

		if (trackerManager != null)
			trackerManager.Data.playerName = "";

		if (playerNameText != null)
			playerNameText.text = "Sin nombre";
	}
}