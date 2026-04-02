using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Naninovel;
using NagaiTabi.TimeOfDay;

namespace NagaiTabi.Runtime.TimeOfDay
{
	public class LocalTimeOfDayController : MonoBehaviour
	{
		[Header("Background Names (Naninovel)")]
		[SerializeField] private string dayBackground = "BG_MT_Morning";
		[SerializeField] private string sunsetBackground = "BG_MT_Sunset";
		[SerializeField] private string nightBackground = "BG_MT_Night";

		[Header("Character / UI Tint")]
		[SerializeField] private Image characterImage;
		[SerializeField] private Image ambientOverlay;

		[Header("Update")]
		[SerializeField] private bool updateEveryMinute = true;
		[SerializeField] private float checkIntervalSeconds = 60f;

		private TimeOfDayPeriod currentPeriod;
		private bool initialized;

		private async void Start()
		{
			await RuntimeInitializer.Initialize();
			await ApplyLocalTimeAsync();
			initialized = true;

			if (updateEveryMinute)
				StartCoroutine(CheckTimeRoutine());
		}

		private IEnumerator CheckTimeRoutine()
		{
			while (true)
			{
				yield return new WaitForSeconds(checkIntervalSeconds);
				_ = ApplyLocalTimeAsync();
			}
		}

		public async Awaitable ApplyLocalTimeAsync()
		{
			var hour = DateTime.Now.Hour;
			var newPeriod = ResolvePeriod(hour);

			if (initialized && newPeriod == currentPeriod)
				return;

			currentPeriod = newPeriod;
			ApplyCharacterTint(newPeriod);
			await ApplyBackgroundAsync(newPeriod);
		}

		public async Awaitable ForcePeriodAsync(TimeOfDayPeriod period)
		{
			currentPeriod = period;
			ApplyCharacterTint(period);
			await ApplyBackgroundAsync(period);
		}

		private TimeOfDayPeriod ResolvePeriod(int hour)
		{
			if (hour >= 6 && hour < 17) return TimeOfDayPeriod.Day;
			if (hour >= 17 && hour < 20) return TimeOfDayPeriod.Sunset;
			return TimeOfDayPeriod.Night;
		}

		private void ApplyCharacterTint(TimeOfDayPeriod period)
		{
			if (characterImage != null)
			{
				switch (period)
				{
					case TimeOfDayPeriod.Day:
						characterImage.color = Color.white;
						break;
					case TimeOfDayPeriod.Sunset:
						characterImage.color = new Color(1f, 0.86f, 0.78f, 1f);
						break;
					case TimeOfDayPeriod.Night:
						characterImage.color = new Color(0.78f, 0.84f, 1f, 1f);
						break;
				}
			}

			if (ambientOverlay != null)
			{
				switch (period)
				{
					case TimeOfDayPeriod.Day:
						ambientOverlay.color = new Color(1f, 1f, 1f, 0f);
						break;
					case TimeOfDayPeriod.Sunset:
						ambientOverlay.color = new Color(1f, 0.55f, 0.3f, 0.12f);
						break;
					case TimeOfDayPeriod.Night:
						ambientOverlay.color = new Color(0.15f, 0.22f, 0.45f, 0.18f);
						break;
				}
			}
		}
		private async Awaitable ApplyBackgroundAsync(TimeOfDayPeriod period)
		{
			var player = Engine.GetService<IScriptPlayer>();

			string scriptName = period switch
			{
				TimeOfDayPeriod.Day => "TOD_Morning",
				TimeOfDayPeriod.Sunset => "TOD_Sunset",
				TimeOfDayPeriod.Night => "TOD_Night",
				_ => "TOD_Morning"
			};

			await player.MainTrack.LoadAndPlay(scriptName);
		}
	}
}