using UnityEngine;

public class DebugButtonReset : MonoBehaviour
{
	[SerializeField] private TrackerManager trackerManager;

	[Header("Vistas a refrescar tras el reset")]
	[SerializeField] private TrackerHUD trackerHUD;
	[SerializeField] private LogsListView logsListView;
	[SerializeField] private StatsPanelView statsPanelView;
	[SerializeField] private StationSignView stationSignView;
	[SerializeField] private JourneyMapView journeyMapView;

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
		if (statsPanelView != null) statsPanelView.Refresh();
		if (stationSignView != null) stationSignView.Refresh();
		if (journeyMapView != null) journeyMapView.Refresh();
	}
}