using TMPro;
using UnityEngine;

public class TrackerHUD : MonoBehaviour
{
	[SerializeField] private TrackerManager trackerManager;
	[SerializeField] private TextMeshProUGUI totalHoursText;

	private void OnEnable()
	{
		if (trackerManager != null)
			trackerManager.OnDataChanged += Refresh;

		Refresh();
	}

	private void OnDisable()
	{
		if (trackerManager != null)
			trackerManager.OnDataChanged -= Refresh;
	}

	public void Refresh()
	{
		if (trackerManager == null || totalHoursText == null)
		{
			Debug.LogWarning(
				"[TrackerHUD] Falta trackerManager o totalHoursText."
			);
			return;
		}

		int totalMinutes = trackerManager.GetTotalMinutes();
		float totalHours = totalMinutes / 60f;

		totalHoursText.text = $"Time:\n{totalHours:0.0} h";

		Debug.Log(
			$"[TrackerHUD] Total actualizado: {totalHours:0.0} h"
		);
	}
}