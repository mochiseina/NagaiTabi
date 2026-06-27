using System;
using System.Collections;
using UnityEngine;
using Naninovel;

namespace NagaiTabi.Runtime.TimeOfDay
{
    /// <summary>
    /// Vigila la hora local EN VIVO y, cuando cruzas a un periodo nuevo
    /// (día / atardecer / noche), reproduce el script .nani correspondiente,
    /// que cambia el fondo y tiñe a Yuina con la luz de ese momento.
    ///
    /// La parte visual y de personalidad vive en los .nani (TOD_Day, TOD_Sunset,
    /// TOD_Night), así que puedes editarla sin tocar este código.
    /// </summary>
    public class TimeOfDayWatcher : MonoBehaviour
    {
        private enum Period { Day, Sunset, Night }

        [Header("Scripts .nani por periodo")]
        [SerializeField] private string dayScript = "TOD_Day";
        [SerializeField] private string sunsetScript = "TOD_Sunset";
        [SerializeField] private string nightScript = "TOD_Night";

        [Header("Cada cuántos segundos se comprueba la hora")]
        [SerializeField] private float checkIntervalSeconds = 30f;

        [Header("¿Empieza activo? Déjalo DESMARCADO para que no actúe en el título.")]
        [SerializeField] private bool startActive = false;

        private Period currentPeriod;
        private bool hasPeriod;
        private bool active;

        /// <summary>
        /// Enciende el vigilante. Llámalo al entrar al tracker. Aplica el periodo actual al instante.
        /// </summary>
        public void Activate()
        {
            active = true;
            if (Engine.Initialized)
                ApplyPeriod(ResolvePeriod(DateTime.Now.Hour), force: true);
        }

        /// <summary>Apaga el vigilante (p. ej. si vuelves al título).</summary>
        public void Deactivate() => active = false;

        private IEnumerator Start()
        {
            // Espera a que Naninovel esté listo.
            while (!Engine.Initialized)
                yield return null;

            if (startActive)
                Activate();

            var wait = new WaitForSeconds(checkIntervalSeconds);
            while (true)
            {
                yield return wait;
                if (active)
                    ApplyPeriod(ResolvePeriod(DateTime.Now.Hour), force: false);
            }
        }

        private Period ResolvePeriod(int hour)
        {
            if (hour >= 6 && hour < 16) return Period.Day;
            if (hour >= 16 && hour < 20) return Period.Sunset;
            return Period.Night;
        }

        private string ScriptFor(Period period) => period switch
        {
            Period.Day => dayScript,
            Period.Sunset => sunsetScript,
            Period.Night => nightScript,
            _ => dayScript
        };

        private void ApplyPeriod(Period period, bool force)
        {
            // Solo actúa si cambió el periodo (o si forzamos en el arranque).
            if (!force && hasPeriod && period == currentPeriod)
                return;

            currentPeriod = period;
            hasPeriod = true;

            if (!Engine.Initialized) return;

            var script = ScriptFor(period);
            var player = Engine.GetService<IScriptPlayer>();
            player.MainTrack.LoadAndPlay(script).Forget();

            // AQUÍ, más adelante, dispararemos también la frase de Yuina
            // del tipo "ya es de noche..." cuando cruce un periodo.
        }
    }
}
