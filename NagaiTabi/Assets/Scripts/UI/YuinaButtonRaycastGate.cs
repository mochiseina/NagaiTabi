using Naninovel;
using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]
public sealed class YuinaButtonRaycastGate : MonoBehaviour
{
	private CanvasGroup canvasGroup;
	private IScriptPlayer scriptPlayer;

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();

		SetButtonInputEnabled(true);
	}

	private void Update()
	{
		if (!Engine.Initialized)
		{
			SetButtonInputEnabled(true);
			return;
		}

		scriptPlayer ??= Engine.GetService<IScriptPlayer>();

		bool buttonShouldReceiveClicks =
			scriptPlayer == null || !scriptPlayer.Playing;

		SetButtonInputEnabled(buttonShouldReceiveClicks);
	}

	private void SetButtonInputEnabled(bool enabled)
	{
		if (canvasGroup == null)
			return;

		canvasGroup.blocksRaycasts = enabled;
		canvasGroup.interactable = enabled;
	}

	private void OnDisable()
	{
		SetButtonInputEnabled(true);
		scriptPlayer = null;
	}
}