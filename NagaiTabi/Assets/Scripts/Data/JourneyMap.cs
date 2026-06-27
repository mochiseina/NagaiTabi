using System;
using System.Collections.Generic;
using UnityEngine;

namespace NagaiTabi.Journey
{
    /// <summary>Una estación del viaje: nombre + horas acumuladas necesarias para llegar.</summary>
    [Serializable]
    public class Station
    {
        public string name;
        public float hoursToReach; // horas totales acumuladas para estar EN esta estación

        public Station(string name, float hoursToReach)
        {
            this.name = name;
            this.hoursToReach = hoursToReach;
        }
    }

    /// <summary>
    /// El "cerebro" del mapa. A partir de las horas totales de inmersión, calcula
    /// en qué estación estás, cuál es la siguiente, cuántas horas faltan, y el progreso.
    /// No dibuja nada: solo da los datos para que la UI los muestre.
    /// </summary>
    public static class JourneyMap
    {
        // Las 14 estaciones, de Okinawa (0 h) a Wakkanai (2000 h).
        // Reparto orientativo a lo largo de las 2000 h; ajusta los números a tu gusto.
        public static readonly List<Station> Stations = new()
        {
            new Station("Okinawa",       5f),
            new Station("Kumamoto",     15f),
            new Station("Fukuoka",      30f),
            new Station("Yamaguchi",    50f),
            new Station("Hiroshima",    75f),
            new Station("Takamatsu",   120f),
            new Station("Kyoto/Osaka", 200f),
            new Station("Tokyo",       300f),
            new Station("Oarai",       450f),
            new Station("Fukushima",   600f),
            new Station("Sendai",      800f),
            new Station("Tsugaru",    1000f),
            new Station("Hakodate",   1250f),
            new Station("Sapporo",    1500f),
            new Station("Wakkanai",   2000f),
        };

        /// <summary>Índice de la estación actual (la última cuyo umbral ya has superado).</summary>
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

        /// <summary>La siguiente estación, o null si ya estás en Wakkanai (final).</summary>
        public static Station GetNextStation(float totalHours)
        {
            int current = GetCurrentStationIndex(totalHours);
            if (current >= Stations.Count - 1) return null; // ya en la última
            return Stations[current + 1];
        }

        /// <summary>Horas que faltan para la siguiente estación (0 si ya llegaste al final).</summary>
        public static float GetHoursToNextStation(float totalHours)
        {
            var next = GetNextStation(totalHours);
            if (next == null) return 0f;
            return Mathf.Max(0f, next.hoursToReach - totalHours);
        }

        /// <summary>
        /// Progreso (0..1) entre la estación actual y la siguiente.
        /// Útil para colocar el tren ENTRE dos puntos del mapa, no solo en la estación.
        /// </summary>
        public static float GetProgressToNextStation(float totalHours)
        {
            int current = GetCurrentStationIndex(totalHours);
            if (current >= Stations.Count - 1) return 1f;

            float from = Stations[current].hoursToReach;
            float to = Stations[current + 1].hoursToReach;
            if (to <= from) return 1f;

            return Mathf.Clamp01((totalHours - from) / (to - from));
        }

        /// <summary>Progreso global del viaje completo (0..1), de Okinawa a Wakkanai.</summary>
        public static float GetTotalProgress(float totalHours)
        {
            float max = Stations[Stations.Count - 1].hoursToReach;
            return Mathf.Clamp01(totalHours / max);
        }
    }
}
