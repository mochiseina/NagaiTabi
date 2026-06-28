using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NagaiTabi.Journey;

/// <summary>
/// Controla los orbes de las estaciones del mapa.
/// Cada orbe muestra uno de tres estados según tus horas totales:
///   - PASADA   (ya superaste esa estación)  -> sprite azul encendido
///   - ACTUAL   (estás en ella ahora)        -> sprite rojo encendido
///   - PENDIENTE (aún no llegaste)           -> sprite marrón/rojo apagado
///
/// SETUP:
/// 1) Coloca tus 15 orbes (Image) sobre el mapa, en el orden de las estaciones
///    (Okinawa primero ... Wakkanai último).
/// 2) Pon este componente en el panel del mapa.
/// 3) Arrastra los 15 orbes a la lista 'Orbs' EN ORDEN (índice 0 = Okinawa).
/// 4) Asigna los tres sprites de estado y el TrackerManager.
/// 5) Llama a Refresh() al entrar al tracker y al loguear (igual que el HUD).
/// </summary>
public class JourneyMapView : MonoBehaviour
{
    [SerializeField] private TrackerManager trackerManager;

    [Header("Los 15 orbes, EN ORDEN (0 = Okinawa ... 14 = Wakkanai)")]
    [SerializeField] private List<Image> orbs = new();

    [Header("Sprites por estado")]
    [Tooltip("Estación ya pasada (azul encendido).")]
    [SerializeField] private Sprite passedSprite;
    [Tooltip("Estación actual (rojo encendido).")]
    [SerializeField] private Sprite currentSprite;
    [Tooltip("Estación aún no alcanzada (marrón/rojo apagado).")]
    [SerializeField] private Sprite pendingSprite;

    public void Refresh()
    {
        if (trackerManager == null)
        {
            Debug.LogWarning("[JourneyMapView] Falta trackerManager.");
            return;
        }

        float totalHours = trackerManager.GetTotalMinutes() / 60f;
        int currentIndex = JourneyMap.GetCurrentStationIndex(totalHours);

        for (int i = 0; i < orbs.Count; i++)
        {
            if (orbs[i] == null) continue;

            Sprite sprite;
            if (i < currentIndex)
                sprite = passedSprite;     // ya pasada
            else if (i == currentIndex)
                sprite = currentSprite;    // estás aquí
            else
                sprite = pendingSprite;    // pendiente

            if (sprite != null)
                orbs[i].sprite = sprite;
        }
    }

    private void Start()
    {
        Refresh();
    }
}
