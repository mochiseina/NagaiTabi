using System;

[Serializable]
public class ImmersionEntry
{
	public string entryId;
	public string dateIso;
	public int minutes;

	public string mode;      // derivado automáticamente
	public string mediaType; // Anime, Manga, VN, etc.
	public string title;
}