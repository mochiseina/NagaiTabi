using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NagaiTabi.Runtime.TimeOfDay
{
    /// <summary>
    /// Cambia el sprite de una Image de UI (p. ej. el fondo del LeftPanel del tracker)
    /// segun la hora local del usuario, con un cross-fade suave opcional.
    /// NO depende de Naninovel: solo intercambia sprites.
    ///
    /// USO: arrastra este componente al GameObject LeftPanel (el que tiene la Image del fondo).
    /// Si lo pones en el mismo objeto que la Image, se auto-asigna. Luego arrastra los 3 sprites.
    /// </summary>
    public class TimeOfDayBackgroundUI : MonoBehaviour
    {
        public enum Period { Day, Sunset, Night }

        [Header("Imagen de fondo a cambiar (LeftPanel)")]
        [Tooltip("Si se deja vacio, intenta usar la Image de este mismo GameObject.")]
        [SerializeField] private Image targetImage;

        [Header("Sprites por periodo")]
        [SerializeField] private Sprite daySprite;     // PH_NT_TrackerBG (cielo azul)
        [SerializeField] private Sprite sunsetSprite;  // PH_NT_TardeBG  (atardecer)
        [SerializeField] private Sprite nightSprite;   // PH_NT_NocheBG  (luna)

        [Header("Transicion")]
        [Tooltip("Duracion del cross-fade en segundos. Pon 0 para un cambio instantaneo.")]
        [SerializeField] private float crossfadeDuration = 1f;

        [Header("Comprobacion periodica")]
        [SerializeField] private bool checkPeriodically = true;
        [SerializeField] private float checkIntervalSeconds = 60f;

        private Period currentPeriod;
        private bool initialized;

        // Se llama al anadir el componente en el editor: intenta auto-asignar la Image.
        private void Reset()
        {
            targetImage = GetComponent<Image>();
        }

        private void Start()
        {
            if (targetImage == null)
                targetImage = GetComponent<Image>();

            if (targetImage == null)
            {
                Debug.LogWarning("[TimeOfDayBackgroundUI] No hay Image asignada ni en este GameObject.");
                return;
            }

            // Primera aplicacion: instantanea (sin fade), segun la hora actual.
            ApplyPeriod(ResolvePeriod(DateTime.Now.Hour), instant: true);
            initialized = true;

            if (checkPeriodically)
                StartCoroutine(CheckRoutine());
        }

        private IEnumerator CheckRoutine()
        {
            var wait = new WaitForSeconds(checkIntervalSeconds);
            while (true)
            {
                yield return wait;
                ApplyPeriod(ResolvePeriod(DateTime.Now.Hour), instant: false);
            }
        }

        /// <summary>Forzar un periodo manualmente (util para pruebas o desde otro script).</summary>
        public void ForcePeriod(Period period) => ApplyPeriod(period, instant: false);

        private Period ResolvePeriod(int hour)
        {
            if (hour >= 6 && hour < 17) return Period.Day;
            if (hour >= 17 && hour < 20) return Period.Sunset;
            return Period.Night;
        }

        private Sprite SpriteFor(Period period) => period switch
        {
            Period.Day => daySprite,
            Period.Sunset => sunsetSprite,
            Period.Night => nightSprite,
            _ => daySprite
        };

        private void ApplyPeriod(Period period, bool instant)
        {
            if (targetImage == null) return;

            // Si ya estamos en ese periodo (y no es la primera vez), no hacemos nada.
            if (initialized && period == currentPeriod) return;
            currentPeriod = period;

            var newSprite = SpriteFor(period);
            if (newSprite == null)
            {
                Debug.LogWarning($"[TimeOfDayBackgroundUI] Falta el sprite para el periodo {period}.");
                return;
            }

            if (instant || crossfadeDuration <= 0f || targetImage.sprite == null)
            {
                targetImage.sprite = newSprite;
                return;
            }

            StartCoroutine(Crossfade(newSprite));
        }

        private IEnumerator Crossfade(Sprite newSprite)
        {
            // Capa temporal ENCIMA con el sprite viejo; la desvanecemos mientras el target
            // ya muestra el nuevo debajo. Asi no hay parpadeo a negro.
            var overlayGO = new GameObject("TODCrossfadeOverlay", typeof(Image));
            var overlay = overlayGO.GetComponent<Image>();
            overlay.sprite = targetImage.sprite;
            overlay.type = targetImage.type;
            overlay.preserveAspect = targetImage.preserveAspect;
            overlay.raycastTarget = false;

            var rt = overlay.rectTransform;
            rt.SetParent(targetImage.rectTransform, false);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.SetAsLastSibling();

            // El target pasa a mostrar el nuevo sprite, oculto bajo la capa.
            targetImage.sprite = newSprite;

            float t = 0f;
            var c = overlay.color;
            while (t < crossfadeDuration)
            {
                t += Time.deltaTime;
                float a = 1f - Mathf.Clamp01(t / crossfadeDuration);
                overlay.color = new Color(c.r, c.g, c.b, a);
                yield return null;
            }

            Destroy(overlayGO);
        }
    }
}
