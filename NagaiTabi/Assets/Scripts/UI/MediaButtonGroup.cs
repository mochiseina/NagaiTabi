using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestiona el grupo de botones de tipo de contenido (Anime, Manga, Reading,
/// Visual N., Video, Audio) para que el SELECCIONADO brille y los demás se atenúen,
/// con sonido de feedback al pulsar.
///
/// SETUP:
/// 1) Pon este componente en un GameObject (p. ej. ContentTypeContainer).
/// 2) Añade una entrada por botón en la lista 'Buttons': arrastra el Button y
///    escribe su mediaType EXACTO (igual que pasas a SetMediaType, p. ej. "Anime",
///    "Manga", "Reading", "Visual Novel", "Video", "Audio").
/// 3) Arrastra tu LogInputPanel.
/// 4) (Opcional) AudioSource + clip de clic.
/// 5) Quita los OnClick -> SetMediaType de cada botón en el Inspector:
///    este componente ya los llama.
/// </summary>
public class MediaButtonGroup : MonoBehaviour
{
	[System.Serializable]
	public class MediaButton
	{
		public Button button;
		public string mediaType;          // "Anime", "Manga", "Visual Novel"...
		[HideInInspector] public Image image;     // se cachea solo
		[HideInInspector] public Vector3 baseScale; // escala original
	}

	[Header("Botones de contenido (botón + su mediaType)")]
	[SerializeField] private List<MediaButton> buttons = new();

	[Header("Referencia")]
	[SerializeField] private LogInputPanel logInputPanel;

	[Header("Aspecto seleccionado / no seleccionado")]
	[Tooltip("Color del botón seleccionado (brillante).")]
	[SerializeField] private Color selectedColor = Color.white;
	[Tooltip("Color de los botones NO seleccionados (atenuado).")]
	[SerializeField] private Color deselectedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
	[Tooltip("Cuánto crece el botón seleccionado (1.1 = 10% más grande).")]
	[SerializeField] private float selectedScale = 1.1f;

	[Header("Sonido de feedback")]
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip clickSound;

	private string currentSelected = "";

	private void Awake()
	{
		// Cachea imagen y escala base, y engancha el click de cada botón.
		foreach (var mb in buttons)
		{
			if (mb.button == null) continue;
			mb.image = mb.button.GetComponent<Image>();
			mb.baseScale = mb.button.transform.localScale;

			string type = mb.mediaType; // captura local para el closure
			mb.button.onClick.AddListener(() => Select(type));
		}
		RefreshVisuals();
	}

	/// <summary>Selecciona un tipo: avisa al panel, suena y actualiza el brillo.</summary>
	public void Select(string mediaType)
	{
		currentSelected = mediaType;

		if (logInputPanel != null)
			logInputPanel.SetMediaType(mediaType);

		if (audioSource != null && clickSound != null)
			audioSource.PlayOneShot(clickSound);

		RefreshVisuals();
	}

	private void RefreshVisuals()
	{
		foreach (var mb in buttons)
		{
			if (mb.button == null) continue;

			bool isSelected = mb.mediaType == currentSelected;

			if (mb.image != null)
				mb.image.color = isSelected ? selectedColor : deselectedColor;

			mb.button.transform.localScale = isSelected
				? mb.baseScale * selectedScale
				: mb.baseScale;
		}
	}

	/// <summary>Limpia la selección (p. ej. tras enviar un log). Opcional.</summary>
	public void ClearSelection()
	{
		currentSelected = "";
		RefreshVisuals();
	}
}
