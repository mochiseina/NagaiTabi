using TMPro;
using UnityEngine;

public class TrackerHUD : MonoBehaviour
{
	[SerializeField] private TrackerManager trackerManager;
	[SerializeField] private TextMeshProUGUI totalHoursText;

	public void Refresh()
	{
		if (trackerManager == null || totalHoursText == null)
			return;

		int totalMinutes = trackerManager.GetTotalMinutes();
		float totalHours = totalMinutes / 60f;

		totalHoursText.text = $"Total: {totalHours:0.0} h";
	}
	private void Start()
	{
		Refresh();
	}
}