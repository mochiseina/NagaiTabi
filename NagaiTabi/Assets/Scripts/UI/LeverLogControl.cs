using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Palanca de registro (versión UI). Va en un objeto de UI (con Image/Button).
/// Al accionarse: comprueba que el log es válido; si lo es, anima el BRAZO en arco
/// alrededor de su pivote, suena un efecto y dispara OnPulled (SubmitLog).
/// Si NO es válido, dispara OnInvalid (para un sonido de error o un aviso).
///
/// SETUP:
/// 1) La palanca debe ser UI (Image), no Sprite Renderer.
/// 2) Pon este componente en el objeto raíz de la palanca (con Button).
/// 3) Arrastra a 'Lever Arm' el RectTransform del BRAZO (la parte que gira).
///    Asegúrate de que el PIVOT del brazo (en su Rect Transform) esté en la BASE
///    del brazo (p. ej. Y=0), para que el giro sea un arco real y no desde el centro.
/// 4) En 'On Pulled ()' arrastra LogInputPanel.SubmitLog.
/// 5) (Opcional) AudioSource + clips de palanca y de error.
/// </summary>
[RequireComponent(typeof(Button))]
public class LeverLogControl : MonoBehaviour
{
    [Header("Brazo que gira (con el pivote en su base)")]
    [SerializeField] private RectTransform leverArm;

    [Header("Animación en arco")]
    [Tooltip("Grados que gira el brazo al accionarse (negativo = otro sentido).")]
    [SerializeField] private float pullAngle = -55f;
    [SerializeField] private float pullDuration = 0.12f;
    [SerializeField] private float returnDuration = 0.25f;

    [Header("Sonido")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pullSound;
    [SerializeField] private AudioClip errorSound;

    [Header("Validación: ¿se puede loguear?")]
    [Tooltip("Arrastra aquí el LogInputPanel para comprobar si los campos son válidos.")]
    [SerializeField] private LogInputPanel logInputPanel;

    [Header("Eventos")]
    [Tooltip("Se dispara si el log es válido (arrastra LogInputPanel.SubmitLog).")]
    public UnityEvent OnPulled;
    [Tooltip("Se dispara si faltan campos (para aviso/sonido de error).")]
    public UnityEvent OnInvalid;

    private Button button;
    private bool busy;
    private float baseZ;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (leverArm != null) baseZ = leverArm.localEulerAngles.z;
        button.onClick.AddListener(TryPull);
    }

    private void OnDestroy()
    {
        if (button != null) button.onClick.RemoveListener(TryPull);
    }

    public void TryPull()
    {
        if (busy) return;

        // Si hay panel asignado y NO es válido, no logueamos.
        if (logInputPanel != null && !logInputPanel.IsValid())
        {
            if (audioSource != null && errorSound != null)
                audioSource.PlayOneShot(errorSound);
            OnInvalid?.Invoke();
            return;
        }

        StartCoroutine(PullRoutine());
    }

    private IEnumerator PullRoutine()
    {
        busy = true;

        if (audioSource != null && pullSound != null)
            audioSource.PlayOneShot(pullSound);

        yield return Rotate(baseZ, baseZ + pullAngle, pullDuration);

        OnPulled?.Invoke(); // SubmitLog

        yield return Rotate(baseZ + pullAngle, baseZ, returnDuration);

        busy = false;
    }

    private IEnumerator Rotate(float fromZ, float toZ, float duration)
    {
        if (leverArm == null || duration <= 0f)
        {
            if (leverArm != null)
                leverArm.localEulerAngles = new Vector3(0, 0, toZ);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
            leverArm.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(fromZ, toZ, k));
            yield return null;
        }
        leverArm.localEulerAngles = new Vector3(0, 0, toZ);
    }
}
