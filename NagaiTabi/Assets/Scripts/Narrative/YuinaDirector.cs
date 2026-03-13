using UnityEngine;

public class YuinaDirector : MonoBehaviour
{
	[SerializeField] private TrackerManager trackerManager;
	[SerializeField] private NaninovelBridge naninovelBridge;

	private void OnEnable()
	{
		if (trackerManager != null)
		{
			trackerManager.OnEntryLogged += HandleEntryLogged;
			Debug.Log("[YuinaDirector] Suscrito a OnEntryLogged.");
		}
		else
		{
			Debug.LogWarning("[YuinaDirector] trackerManager no está asignado.");
		}
	}
	private void OnDisable()
	{
		if (trackerManager != null)
			trackerManager.OnEntryLogged -= HandleEntryLogged;
	}
	private void HandleEntryLogged(ImmersionEntry entry)
	{
		Debug.Log($"[YuinaDirector] Recibida entrada: {entry.minutes} min");

		if (naninovelBridge == null)
		{
			Debug.LogWarning("[YuinaDirector] naninovelBridge no está asignado.");
			return;
		}

		if (entry.minutes >= 120)
			naninovelBridge.PlayScript("YuinaAfterLog_Big");
		else if (entry.minutes >= 30)
			naninovelBridge.PlayScript("YuinaAfterLog_Solid");
		else if (entry.minutes >= 15)
			naninovelBridge.PlayScript("YuinaAfterLog_Good");
		else
			naninovelBridge.PlayScript("YuinaAfterLog_Tiny");
	}
}