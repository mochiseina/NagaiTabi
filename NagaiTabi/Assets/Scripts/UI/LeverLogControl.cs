using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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

        OnPulled?.Invoke();

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