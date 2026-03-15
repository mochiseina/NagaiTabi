public static class MediaTypeMapper
{
	public static string GetModeFromMediaType(string mediaType)
	{
		switch (mediaType)
		{
			case "Manga":
			case "Visual Novel":
			case "Light Novel":
			case "Text":
				return "Reading";

			case "Anime":
			case "Audiobook":
			case "Audio":
			case "Podcast":
			case "Live Action":
			case "Video":
				return "Listening";

			case "Game":
				return "Listening"; // por ahora; luego puedes refinarlo
			default:
				return "Listening";
		}
	}
}