using System;
using System.Collections.Generic;

[Serializable]
public class ImmersionEntry
{
	public string entryId;
	public string dateIso;
	public int minutes;

	public string mode;      // Reading/Listening
	public string mediaType; // Manga/Anime/VN/Moviess...
	public string title;

	public List<string> tags = new();
	public string notes;
}