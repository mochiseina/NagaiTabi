using System;

[Serializable]
public class ImmersionEntry
{
	public string entryId;
	public string dateIso;
	public int minutes;

	public string mode;      // derivado automáticamente (Reading / Listening)
	public string mediaType; // Anime, Manga, VN, etc.
	public string title;

	public int chars;        // caracteres leídos (0 si es contenido de audio)
}