using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LogItemView : MonoBehaviour
{
	[SerializeField] private Image topBorder;
	[SerializeField] private TextMeshProUGUI mediaTypeText;
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private TextMeshProUGUI metaText;

	public void Setup(ImmersionEntry entry)
	{
		if (mediaTypeText != null)
			mediaTypeText.text = entry.mediaType;

		if (titleText != null)
			titleText.text = string.IsNullOrWhiteSpace(entry.title) ? "(sin título)" : entry.title;

		if (metaText != null)
		{
			int h = entry.minutes / 60;
			int m = entry.minutes % 60;
			string duration = h > 0 ? $"{h}h {m}m" : $"{m}m";

			string dateText = entry.dateIso;
			if (DateTime.TryParse(entry.dateIso, out var dt))
				dateText = dt.ToString("dd/MM/yyyy HH:mm");

			metaText.text = $"{duration} · {dateText}";
		}

		if (topBorder != null)
			topBorder.color = GetColorForMediaType(entry.mediaType);
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
