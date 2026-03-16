using UnityEngine;

public class DebugButtonReset : MonoBehaviour
{
	[SerializeField] private TrackerManager trackerManager;
	[SerializeField] private TrackerHUD trackerHUD;
	[SerializeField] private LogsListView logsListView;

	public void ResetData()
	{
		if (trackerManager == null)
		{
			Debug.LogWarning("[DebugButtonReset] trackerManager no está asignado.");
			return;
		}

		trackerManager.ResetAllData();

		if (trackerHUD != null) trackerHUD.Refresh();
		if (logsListView != null) logsListView.Refresh();
	}
}