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

	//racha
	public int currentStreak;   //racha actual (viva)
	public int longestStreak;   //racha más larga alcanzada
	public bool loggedToday;    //ya hay al menos un log hoy?

	//lectura
	public int totalChars;              //caracteres leídos acumulados
	public float avgReadingSpeed;       //chars por hora de lectura
	public int dailyAverageChars;       //media de chars por día activo de lectura
	public float dailyAverageHours;     //media de horas por día activo (cualquier tipo)
}