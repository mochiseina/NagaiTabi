using Naninovel;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NewGameButton : MonoBehaviour
{
	[Tooltip("Script de la intro a arrancar (arrastra Entry.nani).")]
	[ScriptAssetRef] public string entryScript;

	private Button button;

	private void Awake()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(OnNewGame);
	}

	private void OnDestroy()
	{
		if (button != null) button.onClick.RemoveListener(OnNewGame);
	}

	public void OnNewGame()
	{
		if (!Engine.Initialized)
		{
			Debug.LogWarning("[NewGameButton] Naninovel no inicializado.");
			return;
		}

		// Busca el TrackerManager en la escena (no hace falta asignarlo en el Inspector).
		var trackerManager = UnityEngine.Object.FindFirstObjectByType<TrackerManager>();
		if (trackerManager != null)
		{
			trackerManager.ResetAllData();
			Debug.Log("[NewGameButton] Datos reseteados para nueva partida.");
		}
		else
		{
			Debug.LogWarning("[NewGameButton] No se encontró TrackerManager en la escena.");
		}

		if (string.IsNullOrWhiteSpace(entryScript))
		{
			Debug.LogWarning("[NewGameButton] Falta asignar el script Entry.");
			return;
		}

		var player = Engine.GetService<IScriptPlayer>();
		var path = ScriptAssets.GetPath(entryScript);
		Debug.Log($"[NewGameButton] Nueva partida -> {path}");
		player.MainTrack.LoadAndPlay(path).Forget();
	}
}