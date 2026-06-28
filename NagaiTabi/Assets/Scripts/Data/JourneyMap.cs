using System;
using System.Collections.Generic;
using UnityEngine;

namespace NagaiTabi.Journey
{
	/// <summary>Una estación: nombre (romaji legible), nombre japonés, y horas para llegar.</summary>
	[Serializable]
	public class Station
	{
		public string name;        // romaji / nombre legible (Okinawa, Kumamoto...)
		public string nameJp;      // japonés (沖縄, 熊本...)
		public float hoursToReach; // horas totales acumuladas para estar EN esta estación

		public Station(string name, string nameJp, float hoursToReach)
		{
			this.name = name;
			this.nameJp = nameJp;
			this.hoursToReach = hoursToReach;
		}
	}

	/// <summary>
	/// El "cerebro" del mapa. A partir de las horas totales de inmersión, calcula
	/// en qué estación estás, cuál es la siguiente, cuántas horas faltan, y el progreso.
	/// </summary>
	public static class JourneyMap
	{
		// 15 estaciones, de Okinawa (5 h) a Wakkanai (2000 h). Progresión tipo JLPT.
		public static readonly List<Station> Stations = new()
		{
			new Station("Okinawa",     "沖縄",      5f),
			new Station("Kumamoto",    "熊本",     15f),
			new Station("Fukuoka",     "福岡",     30f),
			new Station("Yamaguchi",   "山口",     50f),
			new Station("Hiroshima",   "広島",     75f),
			new Station("Takamatsu",   "高松",    120f),
			new Station("Kyoto",       "京都",    200f),
			new Station("Tokyo",       "東京",    300f),
			new Station("Oarai",       "大洗",    450f),
			new Station("Fukushima",   "福島",    600f),
			new Station("Sendai",      "仙台",    800f),
			new Station("Tsugaru",     "津軽",   1000f),
			new Station("Hakodate",    "函館",   1250f),
			new Station("Sapporo",     "札幌",   1500f),
			new Station("Wakkanai",    "稚内",   2000f),
		};

		public static int GetCurrentStationIndex(float totalHours)
		{
			int index = 0;
			for (int i = 0; i < Stations.Count; i++)
			{
				if (totalHours >= Stations[i].hoursToReach)
					index = i;
				else
					break;
			}
			return index;
		}

		public static Station GetCurrentStation(float totalHours)
			=> Stations[GetCurrentStationIndex(totalHours)];

		public static Station GetNextStation(float totalHours)
		{
			int current = GetCurrentStationIndex(totalHours);
			if (current >= Stations.Count - 1) return null;
			return Stations[current + 1];
		}

		public static float GetHoursToNextStation(float totalHours)
		{
			var next = GetNextStation(totalHours);
			if (next == null) return 0f;
			return Mathf.Max(0f, next.hoursToReach - totalHours);
		}

		public static float GetProgressToNextStation(float totalHours)
		{
			int current = GetCurrentStationIndex(totalHours);
			if (current >= Stations.Count - 1) return 1f;

			float from = Stations[current].hoursToReach;
			float to = Stations[current + 1].hoursToReach;
			if (to <= from) return 1f;

			return Mathf.Clamp01((totalHours - from) / (to - from));
		}

		public static float GetTotalProgress(float totalHours)
		{
			float max = Stations[Stations.Count - 1].hoursToReach;
			return Mathf.Clamp01(totalHours / max);
		}
	}
}