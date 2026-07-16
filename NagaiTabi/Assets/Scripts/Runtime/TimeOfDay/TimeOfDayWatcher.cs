using System;
using System.Collections;
using UnityEngine;
using Naninovel;

namespace NagaiTabi.Runtime.TimeOfDay
{
	public class TimeOfDayWatcher : MonoBehaviour
	{
		private enum Period
		{
			Day,
			Sunset,
			Night
		}

		[Header("Ciclos de iluminación")]
		[SerializeField] private string dayScript = "TOD_Day";
		[SerializeField] private string sunsetScript = "TOD_Sunset";
		[SerializeField] private string nightScript = "TOD_Night";

		[Header("Actualización")]
		[SerializeField] private float checkIntervalSeconds = 30f;
		[SerializeField] private bool startActive = false;

		[Header("Consejo nocturno de Yuina")]
		[SerializeField] private string lateNightScript = "YuinaLateNight";
		[SerializeField] private int lateNightStartHour = 20;
		[SerializeField] private int lateNightStartMinute = 0;
		[SerializeField] private int lateNightEndHour = 4;
		[SerializeField] private int lateNightEndMinute = 59;

		private const string LateKey = "NT_LateAdvice";

		private Period currentPeriod;
		private bool hasPeriod;
		private bool active;

		private Coroutine activationRoutine;

		public void Activate()
		{
			active = true;

			if (activationRoutine != null)
				StopCoroutine(activationRoutine);

			activationRoutine = StartCoroutine(ActivateRoutine());
		}

		public void Deactivate()
		{
			active = false;

			if (activationRoutine != null)
			{
				StopCoroutine(activationRoutine);
				activationRoutine = null;
			}
		}

		private IEnumerator Start()
		{
			while (!Engine.Initialized)
				yield return null;

			if (startActive)
				Activate();

			var wait = new WaitForSecondsRealtime(
				Mathf.Max(1f, checkIntervalSeconds)
			);

			while (true)
			{
				yield return wait;

				if (!active || !Engine.Initialized)
					continue;

				yield return RunChecks(forcePeriod: false);
			}
		}

		private IEnumerator ActivateRoutine()
		{
			while (!Engine.Initialized)
				yield return null;

			yield return RunChecks(forcePeriod: true);

			activationRoutine = null;
		}

		private IEnumerator RunChecks(bool forcePeriod)
		{
			if (!active || !Engine.Initialized)
				yield break;

			var player = Engine.GetService<IScriptPlayer>();

			while (active && player.Playing)
				yield return null;

			if (!active)
				yield break;

			Period resolvedPeriod = ResolvePeriod(DateTime.Now.Hour);

			bool periodChanged =
				forcePeriod ||
				!hasPeriod ||
				resolvedPeriod != currentPeriod;

			if (periodChanged)
			{
				ApplyPeriod(resolvedPeriod, force: true);

				yield return null;

				while (active && player.Playing)
					yield return null;

				if (!active)
					yield break;
			}

			TryLateNightAdvice();
		}

		private void TryLateNightAdvice()
		{
			if (!active || !Engine.Initialized)
				return;

			DateTime now = DateTime.Now;

			var start = new TimeSpan(
				lateNightStartHour,
				lateNightStartMinute,
				0
			);

			var end = new TimeSpan(
				lateNightEndHour,
				lateNightEndMinute,
				59
			);

			if (!IsNowInRange(now.TimeOfDay, start, end))
				return;

			DateTime nightDate = now.Date;

			if (start > end && now.TimeOfDay <= end)
				nightDate = nightDate.AddDays(-1);

			string nightId = nightDate.ToString("yyyyMMdd");

			if (PlayerPrefs.GetString(LateKey, "") == nightId)
				return;

			var player = Engine.GetService<IScriptPlayer>();

			if (player.Playing)
				return;

			Debug.Log(
				$"[TimeOfDayWatcher] Consejo nocturno a las " +
				$"{now:HH:mm}: {lateNightScript}"
			);

			PlayerPrefs.SetString(LateKey, nightId);
			PlayerPrefs.Save();

			player.MainTrack
				.LoadAndPlay(lateNightScript)
				.Forget();
		}

		private static bool IsNowInRange(
			TimeSpan now,
			TimeSpan start,
			TimeSpan end
		)
		{
			if (start <= end)
				return now >= start && now <= end;

			return now >= start || now <= end;
		}

		private Period ResolvePeriod(int hour)
		{
			if (hour >= 6 && hour < 16)
				return Period.Day;

			if (hour >= 16 && hour < 20)
				return Period.Sunset;

			return Period.Night;
		}

		private string ScriptFor(Period period)
		{
			return period switch
			{
				Period.Day => dayScript,
				Period.Sunset => sunsetScript,
				Period.Night => nightScript,
				_ => dayScript
			};
		}

		private void ApplyPeriod(Period period, bool force)
		{
			if (!force && hasPeriod && period == currentPeriod)
				return;

			currentPeriod = period;
			hasPeriod = true;

			if (!Engine.Initialized)
				return;

			string script = ScriptFor(period);
			var player = Engine.GetService<IScriptPlayer>();

			Debug.Log(
				$"[TimeOfDayWatcher] Aplicando periodo: {period} " +
				$"mediante {script}."
			);

			player.MainTrack
				.LoadAndPlay(script)
				.Forget();
		}

		[ContextMenu("Reset Late Night Advice For Testing")]
		private void ResetLateNightAdviceForTesting()
		{
			PlayerPrefs.DeleteKey(LateKey);
			PlayerPrefs.Save();

			Debug.Log(
				"[TimeOfDayWatcher] Clave nocturna eliminada. " +
				"El consejo aparecerá en la próxima entrada al tracker."
			);
		}
	}
}