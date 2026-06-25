using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Naninovel;
using NagaiTabi.TimeOfDay;

namespace NagaiTabi.Runtime.TimeOfDay
{
	/// <summary>
	/// Cambia el fondo del MainBackground de Naninovel según la hora local,
	/// SIN reproducir scripts (no secuestra la VN). También tinta a Yuina y un overlay opcional.
	/// </summary>
	public class LocalTimeOfDayController : MonoBehaviour
	{
		// IMPORTANTE: estos son los NOMBRES DE APARIENCIA tal y como los registres en
		// Naninovel -> Resources -> Backgrounds dentro del actor MainBackground.
		// NO son nombres de script. Cámbialos para que coincidan EXACTO (mayúsculas incluidas).
		[Header("Apariencias del MainBackground (Naninovel -> Resources -> Backgrounds)")]
		[SerializeField] private string dayBackground = "BG_MT_Morning";
		[SerializeField] private string sunsetBackground = "BG_MT_Sunset";
		[SerializeField] private string nightBackground = "BG_MT_Night";

		[Header("Transición")]
		[SerializeField] private float transitionDuration = 1.5f;

		[Header("Tinte de personaje / UI (opcional)")]
		[SerializeField] private Image characterImage;
		[SerializeField] private Image ambientOverlay;

		[Header("Comprobación periódica")]
		[SerializeField] private bool checkPeriodically = true;
		[SerializeField] private float checkIntervalSeconds = 60f;

		private TimeOfDayPeriod currentPeriod;
		private bool initialized;

		private void Start()
		{
			StartCoroutine(Boot());
		}

		private IEnumerator Boot()
		{
			// Esperamos a Naninovel sin usar async void (que oculta las excepciones).
			while (!Engine.Initialized)
				yield return null;

			_ = ApplyLocalTimeAsync(); // primera aplicación (la sincroniza el try/catch interno)
			initialized = true;

			if (checkPeriodically)
				StartCoroutine(CheckTimeRoutine());
		}

		private IEnumerator CheckTimeRoutine()
		{
			var wait = new WaitForSeconds(checkIntervalSeconds);
			while (true)
			{
				yield return wait;
				_ = ApplyLocalTimeAsync();
			}
		}

		public async Awaitable ApplyLocalTimeAsync()
		{
			try
			{
				var newPeriod = ResolvePeriod(DateTime.Now.Hour);

				// Si ya estamos en ese periodo, no hacemos nada (evita transición a la misma apariencia).
				if (initialized && newPeriod == currentPeriod)
					return;

				currentPeriod = newPeriod;
				ApplyCharacterTint(newPeriod);
				await ApplyBackgroundAsync(newPeriod);
			}
			catch (Exception e)
			{
				Debug.LogError($"[TimeOfDay] Error aplicando la hora local: {e}");
			}
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

		private string AppearanceFor(TimeOfDayPeriod period) => period switch
		{
			TimeOfDayPeriod.Day => dayBackground,
			TimeOfDayPeriod.Sunset => sunsetBackground,
			TimeOfDayPeriod.Night => nightBackground,
			_ => dayBackground
		};

		private async Awaitable ApplyBackgroundAsync(TimeOfDayPeriod period)
		{
			if (!Engine.Initialized) return;

			var backManager = Engine.GetService<IBackgroundManager>();
			// "MainBackground" es el ID por defecto del fondo principal.
			var main = backManager.GetActor("MainBackground");
			if (main == null)
			{
				Debug.LogWarning("[TimeOfDay] No se encontró el actor MainBackground.");
				return;
			}

			// ── FIRMA CORRECTA EN 1.21 ──────────────────────────────────────────────────
			// ChangeAppearance ya no acepta un float: la duración va dentro de un Tween.
			// new Tween(duración) usa el cross-fade y easing por defecto. Si quisieras un
			// easing concreto: new Tween(transitionDuration, EasingType.EaseOutSine).
			await main.ChangeAppearance(AppearanceFor(period), new Tween(transitionDuration));
			// ────────────────────────────────────────────────────────────────────────────
		}

		private void ApplyCharacterTint(TimeOfDayPeriod period)
		{
			if (characterImage != null)
			{
				characterImage.color = period switch
				{
					TimeOfDayPeriod.Day => Color.white,
					TimeOfDayPeriod.Sunset => new Color(1f, 0.86f, 0.78f, 1f),
					TimeOfDayPeriod.Night => new Color(0.78f, 0.84f, 1f, 1f),
					_ => Color.white
				};
			}

			if (ambientOverlay != null)
			{
				ambientOverlay.color = period switch
				{
					TimeOfDayPeriod.Day => new Color(1f, 1f, 1f, 0f),
					TimeOfDayPeriod.Sunset => new Color(1f, 0.55f, 0.3f, 0.12f),
					TimeOfDayPeriod.Night => new Color(0.15f, 0.22f, 0.45f, 0.18f),
					_ => new Color(1f, 1f, 1f, 0f)
				};
			}
		}
	}
}