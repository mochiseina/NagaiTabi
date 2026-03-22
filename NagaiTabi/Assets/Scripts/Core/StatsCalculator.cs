using UnityEngine;

public static class StatsCalculator
{
	public static TrackerStats Calculate(TrackerData data)
	{
		var stats = new TrackerStats();

		if (data == null || data.entries == null)
			return stats;

		stats.totalLogs = data.entries.Count;

		foreach (var entry in data.entries)
		{
			stats.totalMinutes += entry.minutes;

			if (entry.mode == "Reading")
				stats.totalReadingMinutes += entry.minutes;
			else if (entry.mode == "Listening")
				stats.totalListeningMinutes += entry.minutes;
		}

		stats.totalHours = stats.totalMinutes / 60f;
		stats.totalReadingHours = stats.totalReadingMinutes / 60f;
		stats.totalListeningHours = stats.totalListeningMinutes / 60f;

		int readHoursRounded = Mathf.RoundToInt(stats.totalReadingHours);
		int listenHoursRounded = Mathf.RoundToInt(stats.totalListeningHours);

		if (readHoursRounded == 0 && listenHoursRounded == 0)
			stats.readingListeningRatioText = "0 : 0";
		else
			stats.readingListeningRatioText = $"{readHoursRounded} : {listenHoursRounded}";

		return stats;
	}
}