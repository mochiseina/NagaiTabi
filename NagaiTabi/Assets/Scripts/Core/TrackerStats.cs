using System;

[Serializable]
public class TrackerStats
{
	public int totalLogs;
	public int totalMinutes;
	public int totalReadingMinutes;
	public int totalListeningMinutes;

	public float totalHours;
	public float totalReadingHours;
	public float totalListeningHours;

	public string readingListeningRatioText;
}