using System;
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
		if (trackerManager == null)
		{
			Debug.LogWarning("[PlayerProfileView] Falta trackerManager.");
			return;
		}
		string name = trackerManager.Data != null ? trackerManager.Data.playerName : "";

		if (string.IsNullOrWhiteSpace(name) && Engine.Initialized)
		{
			string fromNani = TryReadNaninovelName();
			if (!string.IsNullOrWhiteSpace(fromNani))
			{
				name = fromNani;
				trackerManager.SetPlayerName(name); // persiste al JSON
				Debug.Log($"[PlayerProfileView] Nombre capturado de la intro y guardado: '{name}'");
			}
		}

		if (string.IsNullOrWhiteSpace(name))
			name = "Sin nombre";

		if (playerNameText != null)
			playerNameText.text = name;
	}

	/// <summary>Lee PlayerName de Naninovel de forma segura; "" si no existe.</summary>
	private string TryReadNaninovelName()
	{
		try
		{
			var vars = Engine.GetService<ICustomVariableManager>();
			return vars.GetVariableValue("PlayerName").String;
		}
		catch (Exception)
		{
			return "";
		}
	}

	public void ResetPlayerNameForTesting()
	{
		try
		{
			if (Engine.Initialized)
			{
				var vars = Engine.GetService<ICustomVariableManager>();
				vars.SetVariableValue("PlayerName", new(""));
			}
		}
		catch (Exception) { /* si no existe, nada que limpiar */ }

		if (trackerManager != null)
			trackerManager.SetPlayerName("");

		if (playerNameText != null)
			playerNameText.text = "Sin nombre";
	}
}