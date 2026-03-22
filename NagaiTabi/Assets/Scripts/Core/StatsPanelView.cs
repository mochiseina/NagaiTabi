using TMPro;
using UnityEngine;

public class StatsPanelView : MonoBehaviour
{
	[SerializeField] private TrackerManager trackerManager;

	[Header("Texts")]
	[SerializeField] private TextMeshProUGUI ratioText;
	[SerializeField] private TextMeshProUGUI totalReadText;
	[SerializeField] private TextMeshProUGUI totalLogsText;
	[SerializeField] private TextMeshProUGUI totalHoursText;
	[SerializeField] private TextMeshProUGUI totalListenedText;

	public void Refresh()
	{
		if (trackerManager == null)
		{
			Debug.LogWarning("[StatsPanelView] trackerManager no está asignado.");
			return;
		}

		var stats = StatsCalculator.Calculate(trackerManager.Data);

		if (ratioText != null)
			ratioText.text = $"Ratio: {stats.readingListeningRatioText}";

		if (totalReadText != null)
			totalReadText.text = $"Total Read: {stats.totalReadingHours:0.0} h";

		if (totalLogsText != null)
			totalLogsText.text = $"Total Logs: {stats.totalLogs}";

		if (totalHoursText != null)
			totalHoursText.text = $"Total Hours: {stats.totalHours:0.0} h";

		if (totalListenedText != null)
			totalListenedText.text = $"Total Listened: {stats.totalListeningHours:0.0} h";
	}

	private void Start()
	{
		Refresh();
	}
}