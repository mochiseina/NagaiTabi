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
	[Tooltip("Recuadro morado: racha actual.")]
	[SerializeField] private TextMeshProUGUI streakText;

	[Header("Métricas de lectura")]
	[SerializeField] private TextMeshProUGUI totalCharsText;
	[SerializeField] private TextMeshProUGUI readingSpeedText;
	[SerializeField] private TextMeshProUGUI dailyAvgCharsText;
	[SerializeField] private TextMeshProUGUI dailyAvgHoursText;

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

		if (streakText != null)
		{
			string unit = stats.currentStreak == 1 ? "day" : "days";
			streakText.text = $"Streak: {stats.currentStreak} {unit}";
		}

		// --- Métricas de lectura nuevas ---
		if (totalCharsText != null)
			totalCharsText.text = $"Total Chars: {stats.totalChars:N0}";

		if (readingSpeedText != null)
			readingSpeedText.text = $"Reading Speed: {stats.avgReadingSpeed:N0} cph";

		if (dailyAvgCharsText != null)
			dailyAvgCharsText.text = $"Daily Avg Chars: {stats.dailyAverageChars:N0}";

		if (dailyAvgHoursText != null)
			dailyAvgHoursText.text = $"Daily Avg Hours: {stats.dailyAverageHours:0.0} h";
	}
}