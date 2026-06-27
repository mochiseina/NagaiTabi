using TMPro;
using UnityEngine;
using NagaiTabi.Journey;

/// <summary>
/// Rellena el cartel de estación del tracker (estación actual, siguiente y horas
/// que faltan) usando las horas totales de inmersión y la lógica de JourneyMap.
///
/// No dibuja el mapa: solo el cartel de texto. Sirve para ver el sistema funcionando
/// con datos reales antes de tener el gráfico del mapa.
///
/// SETUP: pon este componente en el objeto del cartel, asigna el TrackerManager
/// y los textos (TMP) que quieras rellenar. Llama a Refresh() al entrar al tracker
/// y cada vez que se loguee algo (igual que haces con TrackerHUD).
/// </summary>
public class StationSignView : MonoBehaviour
{
    [SerializeField] private TrackerManager trackerManager;

    [Header("Textos del cartel (asigna los que uses)")]
    [Tooltip("Estación actual, p. ej. 'Yamaguchi'.")]
    [SerializeField] private TMP_Text currentStationText;
    [Tooltip("Próxima estación, p. ej. 'Hiroshima'.")]
    [SerializeField] private TMP_Text nextStationText;
    [Tooltip("Horas que faltan, p. ej. '15 hours'.")]
    [SerializeField] private TMP_Text hoursLeftText;

    public void Refresh()
    {
        if (trackerManager == null)
        {
            Debug.LogWarning("[StationSignView] Falta trackerManager.");
            return;
        }

        float totalHours = trackerManager.GetTotalMinutes() / 60f;

        var current = JourneyMap.GetCurrentStation(totalHours);
        var next = JourneyMap.GetNextStation(totalHours);
        float hoursLeft = JourneyMap.GetHoursToNextStation(totalHours);

        if (currentStationText != null)
            currentStationText.text = current.name;

        if (next == null)
        {
            // Ya en Wakkanai: fin del viaje.
            if (nextStationText != null) nextStationText.text = "終点 · Wakkanai";
            if (hoursLeftText != null) hoursLeftText.text = "¡Has llegado al final!";
        }
        else
        {
            if (nextStationText != null)
                nextStationText.text = next.name;

            if (hoursLeftText != null)
            {
                // Redondea a 1 decimal; si es menos de 1h, muestra los minutos.
                if (hoursLeft >= 1f)
                    hoursLeftText.text = $"{hoursLeft:0.#} hours";
                else
                    hoursLeftText.text = $"{Mathf.CeilToInt(hoursLeft * 60f)} min";
            }
        }
    }

    private void Start()
    {
        Refresh();
    }
}
