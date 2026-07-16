using System;
using Naninovel;
using Naninovel.Commands;
using UnityEngine;
using NagaiTabi.Runtime.TimeOfDay;

namespace NagaiTabi.Commands
{
	[Serializable, Alias("activateTimeOfDay")]
	public sealed class ActivateTimeOfDay : Command
	{
		public override Awaitable Execute(ExecutionContext ctx)
		{
			var watcher =
				UnityEngine.Object.FindFirstObjectByType<TimeOfDayWatcher>(
					FindObjectsInactive.Include
				);

			if (watcher == null)
			{
				Debug.LogWarning(
					"[ActivateTimeOfDay] No se encontró TimeOfDayWatcher."
				);

				return Async.Completed;
			}

			watcher.Activate();

			Debug.Log(
				"[ActivateTimeOfDay] Sistema horario activado."
			);

			return Async.Completed;
		}
	}

	[Serializable, Alias("deactivateTimeOfDay")]
	public sealed class DeactivateTimeOfDay : Command
	{
		public override Awaitable Execute(ExecutionContext ctx)
		{
			var watcher =
				UnityEngine.Object.FindFirstObjectByType<TimeOfDayWatcher>(
					FindObjectsInactive.Include
				);

			if (watcher == null)
			{
				Debug.LogWarning(
					"[DeactivateTimeOfDay] No se encontró TimeOfDayWatcher."
				);

				return Async.Completed;
			}

			watcher.Deactivate();

			Debug.Log(
				"[DeactivateTimeOfDay] Sistema horario desactivado."
			);

			return Async.Completed;
		}
	}
}