using System;
using System.Collections.Generic;
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
			{
				stats.totalReadingMinutes += entry.minutes;
				stats.totalChars += entry.chars;
			}
			else if (entry.mode == "Listening")
			{
				stats.totalListeningMinutes += entry.minutes;
			}
		}

		stats.totalHours = stats.totalMinutes / 60f;
		stats.totalReadingHours = stats.totalReadingMinutes / 60f;
		stats.totalListeningHours = stats.totalListeningMinutes / 60f;

		stats.readingListeningRatioText = BuildRatioBase10(
			stats.totalReadingMinutes, stats.totalListeningMinutes);

		// Velocidad de lectura (chars/hora) sobre horas de lectura.
		stats.avgReadingSpeed = stats.totalReadingHours > 0f
			? stats.totalChars / stats.totalReadingHours
			: 0f;

		CalculateStreaks(data, stats);
		CalculateDailyReadingAverage(data, stats);

		return stats;
	}

	/// <summary>Ratio reading:listening en base 10 (siempre suman 10). 7:3, 5:5...</summary>
	private static string BuildRatioBase10(int readingMinutes, int listeningMinutes)
	{
		int total = readingMinutes + listeningMinutes;
		if (total <= 0) return "0 : 0";

		float readingShare = (float)readingMinutes / total;
		int readingPart = Mathf.Clamp(Mathf.RoundToInt(readingShare * 10f), 0, 10);
		int listeningPart = 10 - readingPart;
		return $"{readingPart} : {listeningPart}";
	}

	/// <summary>
	/// Racha = días consecutivos (fecha local) con al menos 1 log.
	/// - currentStreak: cuenta hacia atrás desde HOY; si hoy no hay log pero ayer sí,
	///   la racha sigue viva (cuenta hasta ayer).
	/// - longestStreak: la secuencia consecutiva más larga del historial.
	/// - loggedToday: si hay al menos un log con fecha de hoy.
	/// </summary>
	private static void CalculateStreaks(TrackerData data, TrackerStats stats)
	{
		// Conjunto de días únicos (a medianoche local) que tienen algún log.
		var days = new HashSet<DateTime>();
		foreach (var entry in data.entries)
		{
			if (DateTime.TryParse(entry.dateIso, out var dt))
				days.Add(dt.ToLocalTime().Date);
		}

		if (days.Count == 0)
		{
			stats.currentStreak = 0;
			stats.longestStreak = 0;
			stats.loggedToday = false;
			return;
		}

		// Ordena los días ascendentes para medir la racha más larga.
		var sorted = new List<DateTime>(days);
		sorted.Sort();

		// Racha más larga: recorre buscando secuencias de días consecutivos.
		int longest = 1;
		int run = 1;
		for (int i = 1; i < sorted.Count; i++)
		{
			if ((sorted[i] - sorted[i - 1]).Days == 1)
				run++;
			else
				run = 1;

			if (run > longest) longest = run;
		}
		stats.longestStreak = longest;

		// Racha actual: parte de hoy; si hoy no hay log, prueba desde ayer.
		DateTime today = DateTime.Now.Date;
		stats.loggedToday = days.Contains(today);

		DateTime cursor;
		if (days.Contains(today))
			cursor = today;
		else if (days.Contains(today.AddDays(-1)))
			cursor = today.AddDays(-1); // hoy aún no logueado, racha viva hasta ayer
		else
		{
			stats.currentStreak = 0; // ni hoy ni ayer -> racha rota
			return;
		}

		int current = 0;
		while (days.Contains(cursor))
		{
			current++;
			cursor = cursor.AddDays(-1);
		}
		stats.currentStreak = current;
	}

	/// <summary>Media de caracteres por día que tuvo lectura (chars totales / días con lectura).</summary>
	private static void CalculateDailyReadingAverage(TrackerData data, TrackerStats stats)
	{
		var readingDays = new HashSet<DateTime>();
		foreach (var entry in data.entries)
		{
			if (entry.mode == "Reading" && entry.chars > 0 &&
				DateTime.TryParse(entry.dateIso, out var dt))
				readingDays.Add(dt.ToLocalTime().Date);
		}

		stats.dailyAverageChars = readingDays.Count > 0
			? Mathf.RoundToInt((float)stats.totalChars / readingDays.Count)
			: 0;
	}
}
