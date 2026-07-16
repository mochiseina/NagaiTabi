using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogItemView : MonoBehaviour
{
	[SerializeField] private Image topBorder;
	[SerializeField] private TextMeshProUGUI mediaTypeText;
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private TextMeshProUGUI metaText;

	[Header("Actions")]
	[SerializeField] private Button deleteButton;

	private string entryId;
	private Action<string> deleteCallback;

	public void Setup(
		ImmersionEntry entry,
		Action<string> onDeleteRequested
	)
	{
		entryId = entry.entryId;
		deleteCallback = onDeleteRequested;

		if (mediaTypeText != null)
			mediaTypeText.text = entry.mediaType;

		if (titleText != null)
		{
			titleText.text = string.IsNullOrWhiteSpace(entry.title)
				? "(sin título)"
				: entry.title;
		}

		if (metaText != null)
		{
			int hours = entry.minutes / 60;
			int minutes = entry.minutes % 60;

			string duration = hours > 0
				? $"{hours}h {minutes}m"
				: $"{minutes}m";

			string dateText = entry.dateIso;

			if (DateTime.TryParse(entry.dateIso, out DateTime date))
				dateText = date.ToString("dd/MM/yyyy HH:mm");

			string charsText = entry.chars > 0
				? $" · {entry.chars:N0} chars"
				: "";

			metaText.text =
				$"{duration}{charsText} · {dateText}";
		}

		if (topBorder != null)
			topBorder.color = GetColorForMediaType(entry.mediaType);

		if (deleteButton != null)
		{
			deleteButton.onClick.RemoveListener(
				HandleDeleteClicked
			);

			deleteButton.onClick.AddListener(
				HandleDeleteClicked
			);
		}
	}

	private void HandleDeleteClicked()
	{
		if (string.IsNullOrWhiteSpace(entryId))
		{
			Debug.LogWarning(
				"[LogItemView] El log no tiene entryId."
			);
			return;
		}

		deleteCallback?.Invoke(entryId);
	}

	private void OnDestroy()
	{
		if (deleteButton != null)
		{
			deleteButton.onClick.RemoveListener(
				HandleDeleteClicked
			);
		}
	}

	private Color GetColorForMediaType(string mediaType)
	{
		switch (mediaType)
		{
			case "Anime":
				return new Color(1f, 0.2f, 0.6f);

			case "Visual Novel":
				return new Color(0.1f, 0.9f, 0.9f);

			case "Manga":
				return new Color(0.7f, 0.3f, 1f);

			case "Light Novel":
				return new Color(0.4f, 0.7f, 1f);

			case "Audio":
			case "Audiobook":
			case "Podcast":
				return new Color(0.2f, 1f, 0.4f);

			case "Live Action":
			case "Movie":
			case "Video":
				return new Color(1f, 0.5f, 0.2f);

			case "Text":
				return new Color(0.9f, 0.9f, 0.3f);

			case "Game":
				return new Color(0.3f, 1f, 0.8f);

			default:
				return Color.white;
		}
	}
}