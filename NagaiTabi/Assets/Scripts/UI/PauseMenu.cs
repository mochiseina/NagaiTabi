using Naninovel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Menú de pausa del tracker. Usa el nuevo Input System (Unity 6) directamente.
/// ESC abre/cierra; Return to Title reproduce ReturnToTitle.nani; Resume cierra.
///
/// Debe estar en un GameObject SIEMPRE ACTIVO (p. ej. Systems), NUNCA dentro del panel.
///
/// SETUP:
/// - Pause Panel -> el panel de pausa (empieza desactivado).
/// - Return To Title Button -> ReturnButton.
/// - Resume Button -> ResumeButton.
/// - Return To Title Script -> ReturnToTitle.nani
/// </summary>
public class PauseMenu : MonoBehaviour
{
	[Header("Panel")]
	[SerializeField] private GameObject pausePanel;

	[Header("Botones")]
	[SerializeField] private Button returnToTitleButton;
	[SerializeField] private Button resumeButton;

	[Header("Script de vuelta al título (arrastra ReturnToTitle.nani)")]
	[ScriptAssetRef] public string returnToTitleScript;

	private bool isPaused = false;

	private void Awake()
	{
		if (pausePanel != null) pausePanel.SetActive(false);
		if (returnToTitleButton != null) returnToTitleButton.onClick.AddListener(ReturnToTitle);
		if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
	}

	private void Update()
	{
		// Nuevo Input System: leer ESC del teclado actual.
		if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
		{
			if (isPaused) Resume();
			else Pause();
		}
	}

	public void Pause()
	{
		isPaused = true;
		if (pausePanel != null) pausePanel.SetActive(true);
	}

	public void Resume()
	{
		isPaused = false;
		if (pausePanel != null) pausePanel.SetActive(false);
	}

	public void ReturnToTitle()
	{
		Resume();

		if (!Engine.Initialized)
		{
			Debug.LogWarning("[PauseMenu] Naninovel no inicializado.");
			return;
		}
		if (string.IsNullOrWhiteSpace(returnToTitleScript))
		{
			Debug.LogWarning("[PauseMenu] Falta asignar ReturnToTitle.nani.");
			return;
		}

		var player = Engine.GetService<IScriptPlayer>();
		var path = ScriptAssets.GetPath(returnToTitleScript);
		player.MainTrack.LoadAndPlay(path).Forget();
	}
}