using UnityEngine;
using TMPro;
using System.Collections;

[DefaultExecutionOrder(1000)]
public class DialogueBubble : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text textField;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Following")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0.4f, 0f);

    [Header("Orientation")]
    [SerializeField] private bool keepUpright = true;

    [Header("Size")]
    [SerializeField] private bool constantScreenSize = true;
    [SerializeField] private float sizeFactor = 0.0015f;
    [SerializeField] private float baseScale = 0.01f;

    [Header("Text")]
    [SerializeField] private int charsPerSecond = 40;

    private Camera cam;
    private Coroutine typingCoroutine;

    public bool isTyping => typingCoroutine != null;
    public bool isVisible => canvasGroup != null && canvasGroup.alpha > 0f;

    private void EnsureReferences()
    {
        if (textField == null)
            textField = GetComponentInChildren<TMP_Text>(true);

        if (canvasGroup == null)
            canvasGroup = GetComponentInChildren<CanvasGroup>(true);

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (textField != null)
            textField.gameObject.SetActive(true);

        if (canvasGroup != null)
            canvasGroup.gameObject.SetActive(true);
    }

    private Camera ResolveCamera()
    {
        if (Camera.main != null)
            return Camera.main;

        var anyCamera = Object.FindFirstObjectByType<Camera>();
        if (anyCamera != null)
            return anyCamera;

        var cinemachineBrainType = System.Type.GetType("Cinemachine.CinemachineBrain, Cinemachine");
        if (cinemachineBrainType != null)
        {
            var brain = Object.FindFirstObjectByType(cinemachineBrainType);
            if (brain != null)
            {
                var outputCameraProperty = cinemachineBrainType.GetProperty("OutputCamera");
                if (outputCameraProperty != null)
                {
                    var outputCamera = outputCameraProperty.GetValue(brain) as Camera;
                    if (outputCamera != null)
                        return outputCamera;
                }
            }
        }

        return null;
    }

    private void Awake()
    {
        EnsureReferences();
        cam = ResolveCamera();
        Hide();
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        if (cam == null)
            cam = ResolveCamera();

        if (cam == null)
            return;

        transform.position = target.position + offset;

        if (keepUpright)
        {
            Vector3 forward = cam.transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            }
            else
            {
                transform.rotation = cam.transform.rotation;
            }
        }

        float scale = baseScale;
        if (constantScreenSize)
        {
            float distance = Vector3.Distance(cam.transform.position, transform.position);
            scale = distance * sizeFactor;
        }

        transform.localScale = Vector3.one * scale;
    }

    public void SetTarget(Transform newTarget) => target = newTarget;

    public void Show(string line)
    {
        EnsureReferences();

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        if (textField != null)
            textField.gameObject.SetActive(true);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(line));
    }

    public void Skip()
    {
        if (typingCoroutine == null)
            return;

        StopCoroutine(typingCoroutine);
        typingCoroutine = null;
        textField.maxVisibleCharacters = textField.textInfo.characterCount;
    }

    public void Hide()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (textField != null)
            textField.maxVisibleCharacters = 0;
    }

    private IEnumerator TypeText(string line)
    {
        textField.text = line;
        textField.maxVisibleCharacters = 0;
        textField.ForceMeshUpdate();

        int totalChars = textField.textInfo.characterCount;
        float elapsed = 0f;

        while (textField.maxVisibleCharacters < totalChars)
        {
            elapsed += Time.unscaledDeltaTime * charsPerSecond;
            textField.maxVisibleCharacters = Mathf.Min(Mathf.FloorToInt(elapsed), totalChars);
            yield return null;
        }

        typingCoroutine = null;
    }

    private void Start()
    {
    }

    private void Update()
    {
    }
}
