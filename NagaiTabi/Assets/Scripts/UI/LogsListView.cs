using System.Text;
using TMPro;
using UnityEngine;

public class LogsListView : MonoBehaviour
{
	[SerializeField] private TrackerManager trackerManager;
	[SerializeField] private TextMeshProUGUI recentLogsText;
	[SerializeField] private int maxLogsToShow = 5;

	public void Refresh()
	{
		if (trackerManager == null || recentLogsText == null) return;

		var entries = trackerManager.Data.entries;
		var sb = new StringBuilder();

		int startIndex = Mathf.Max(0, entries.Count - maxLogsToShow);

		for (int i = entries.Count - 1; i >= startIndex; i--)
		{
			var e = entries[i];
			int h = e.minutes / 60;
			int m = e.minutes % 60;

			string duration = h > 0
				? $"{h}h {m}m"
				: $"{m}m";

			string safeTitle = string.IsNullOrWhiteSpace(e.title) ? "(sin título)" : e.title;

			sb.AppendLine($"{safeTitle}");
			sb.AppendLine($"{e.mediaType} | {duration}");
			sb.AppendLine("");
		}

		recentLogsText.text = sb.ToString();
	}

	private void Start()
	{
		Refresh();
	}
}