using UnityEngine;

public class YuinaDirector : MonoBehaviour
{
	[SerializeField] private TrackerManager trackerManager;
	[SerializeField] private NaninovelBridge naninovelBridge;

	private void OnEnable()
	{
		if (trackerManager != null)
			trackerManager.OnEntryLogged += HandleEntryLogged;
	}
	private void OnDisable()
	{
		if (trackerManager != null)
			trackerManager.OnEntryLogged -= HandleEntryLogged;
	}
	private void HandleEntryLogged(ImmersionEntry entry)
	{
		if (naninovelBridge == null || entry == null) return;

		if (entry.minutes >= 120)
		{
			naninovelBridge.PlayScript("YuinaAfterLog_Big");
			return;
		}
		if (entry.mode == "Reading" && entry.minutes >= 30)
		{
			naninovelBridge.PlayScript("YuinaAfterLog_ReadingSolid");
			return;
		}
		if (entry.mode == "Listening" && entry.minutes >= 30)
		{
			naninovelBridge.PlayScript("YuinaAfterLog_ListeningSolid");
			return;
		}
		if (entry.minutes >= 15)
		{
			naninovelBridge.PlayScript("YuinaAfterLog_Good");
			return;
		}
		naninovelBridge.PlayScript("YuinaAfterLog_Tiny");
	}
}