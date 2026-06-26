using System;
using Naninovel;
using UnityEngine;
using NagaiTabi.Runtime.TimeOfDay;

namespace NagaiTabi.Commands
{
    /// <summary>
    /// Comando @activatetimeofday : enciende el TimeOfDayWatcher de la escena.
    /// Se llama al entrar al tracker (en MainTracker.nani) para que el ciclo
    /// día/noche NO actúe en el título.
    /// </summary>
    [Serializable, Alias("activatetimeofday")]
    public class ActivateTimeOfDay : Command
    {
        public override Awaitable Execute(ExecutionContext ctx)
        {
            var watcher = UnityEngine.Object.FindFirstObjectByType<TimeOfDayWatcher>();
            if (watcher != null)
                watcher.Activate();
            else
                Debug.LogWarning("[ActivateTimeOfDay] No se encontró TimeOfDayWatcher en la escena.");

            return Async.Completed;
        }
    }
}
