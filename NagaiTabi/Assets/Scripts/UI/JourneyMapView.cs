using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NagaiTabi.Journey;

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
