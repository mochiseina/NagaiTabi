using System;
using Naninovel;
using Naninovel.Commands;
using UnityEngine;
using NagaiTabi.Runtime.TimeOfDay;
using NagaiTabi.TimeOfDay;

namespace NagaiTabi.Commands
{
	[Serializable, Alias("settimeofday")]
	public class SetTimeOfDay : Command
	{
		public StringParameter Period;

		public override Awaitable Execute(ExecutionContext ctx)
		{
			if (TimeOfDayRegistry.Instance == null)
			{
				Debug.LogWarning("TimeOfDayRegistry.Instance no encontrado.");
				return Async.Completed;
			}

			var value = Assigned(Period) ? Period.Value?.ToLowerInvariant() : "day";

			var period = value switch
			{
				"day" => TimeOfDayPeriod.Day,
				"sunset" => TimeOfDayPeriod.Sunset,
				"night" => TimeOfDayPeriod.Night,
				_ => TimeOfDayPeriod.Day
			};

			_ = TimeOfDayRegistry.Instance.ForcePeriodAsync(period);
			return Async.Completed;
		}
	}
}