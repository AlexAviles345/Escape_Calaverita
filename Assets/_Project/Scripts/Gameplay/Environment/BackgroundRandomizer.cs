using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundRandomizer : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private Camera targetCamera;

    [Header("Fondos disponibles")]
    [SerializeField] private Sprite[] backgrounds;

    [Header("Configuración")]
    [SerializeField] private bool avoidImmediateRepeat = true;

    private static int previousBackgroundIndex = -1;

    private int previousScreenWidth;
    private int previousScreenHeight;

    private void Awake()
    {
        if (backgroundRenderer == null)
            backgroundRenderer = GetComponent<SpriteRenderer>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        SelectRandomBackground();
        FitBackgroundToCamera();
        RememberScreenSize();
    }

    private void LateUpdate()
    {
        if (Screen.width == previousScreenWidth &&
            Screen.height == previousScreenHeight)
        {
            return;
        }

        FitBackgroundToCamera();
        RememberScreenSize();
    }

    private void SelectRandomBackground()
    {
        if (backgrounds == null || backgrounds.Length == 0)
        {
            Debug.LogWarning(
                "BackgroundRandomizer no tiene fondos asignados.",
                this
            );
            return;
        }

        int selectedIndex = Random.Range(0, backgrounds.Length);

        if (avoidImmediateRepeat &&
            backgrounds.Length > 1 &&
            selectedIndex == previousBackgroundIndex)
        {
            int offset = Random.Range(1, backgrounds.Length);
            selectedIndex = (selectedIndex + offset) % backgrounds.Length;
        }

        Sprite selectedBackground = backgrounds[selectedIndex];

        if (selectedBackground == null)
        {
            Debug.LogWarning(
                $"El fondo de la posición {selectedIndex} está vacío.",
                this
            );
            return;
        }

        backgroundRenderer.sprite = selectedBackground;
        previousBackgroundIndex = selectedIndex;
    }

    private void FitBackgroundToCamera()
    {
        if (backgroundRenderer == null ||
            backgroundRenderer.sprite == null ||
            targetCamera == null ||
            !targetCamera.orthographic)
        {
            return;
        }

        Vector2 spriteSize = backgroundRenderer.sprite.bounds.size;

        float cameraHeight = targetCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * targetCamera.aspect;

        float horizontalScale = cameraWidth / spriteSize.x;
        float verticalScale = cameraHeight / spriteSize.y;
        float requiredScale = Mathf.Max(horizontalScale, verticalScale);

        transform.localScale = new Vector3(
            requiredScale,
            requiredScale,
            1f
        );
    }

    private void RememberScreenSize()
    {
        previousScreenWidth = Screen.width;
        previousScreenHeight = Screen.height;
    }
}