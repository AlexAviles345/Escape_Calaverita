using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Lapida : MonoBehaviour
{
    // La modifica el spawner para que todas las lápidas compartan velocidad
    public static float Speed = 3f;
    public float GapCenterLocal => gapCenterLocal;

    [Header("Colision del hueco")]
    [SerializeField] private float gapCenterLocal = 4f;
    [SerializeField] private float gapHeightLocal = 3f;
    [SerializeField, Range(0.5f, 1f)] private float colliderWidth = 0.72f;
    [SerializeField, Range(0.25f, 0.8f)] private float openingColliderWidth = 0.44f;

    private Rigidbody2D rb;
    private Transform player;
    private float despawnX;
    private bool scored;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ConfigureGapColliders();
    }

    private void Start()
    {
        // Borde izquierdo de la cámara menos un margen
        Camera mainCamera = Camera.main;
        despawnX = mainCamera != null
            ? mainCamera.ViewportToWorldPoint(Vector3.zero).x - 3f
            : -12f;

        PlayerMovement playerMovement = FindAnyObjectByType<PlayerMovement>();
        if (playerMovement != null)
            player = playerMovement.transform;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsGameStarted ||
            GameManager.Instance.IsGameOver)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = Vector2.left * Speed;
    }

    private void Update()
    {
        if (!scored && player != null && transform.position.x < player.position.x)
        {
            scored = true;
            GameManager.Instance?.AddScore();
        }

        if (transform.position.x < despawnX)
            Destroy(gameObject);
    }

    private void ConfigureGapColliders()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;

        PolygonCollider2D polygon = GetComponent<PolygonCollider2D>();
        if (polygon == null)
            polygon = gameObject.AddComponent<PolygonCollider2D>();

        foreach (Collider2D otherCollider in GetComponents<Collider2D>())
        {
            if (otherCollider != polygon)
                otherCollider.enabled = false;
        }

        Bounds bounds = spriteRenderer.sprite.bounds;
        float gapBottom = gapCenterLocal - gapHeightLocal * 0.5f;
        float gapTop = gapCenterLocal + gapHeightLocal * 0.5f;
        float outerHalfWidth = bounds.extents.x * colliderWidth;
        float openingHalfWidth = bounds.extents.x * openingColliderWidth;
        float topShoulder = Mathf.Lerp(gapTop, bounds.max.y, 0.32f);
        float bottomShoulder = Mathf.Lerp(bounds.min.y, gapBottom, 0.68f);
        float centerX = bounds.center.x;

        Vector2[] topPath =
        {
            new Vector2(centerX - openingHalfWidth, gapTop),
            new Vector2(centerX + openingHalfWidth, gapTop),
            new Vector2(centerX + outerHalfWidth, topShoulder),
            new Vector2(centerX + outerHalfWidth, bounds.max.y),
            new Vector2(centerX - outerHalfWidth, bounds.max.y),
            new Vector2(centerX - outerHalfWidth, topShoulder)
        };

        Vector2[] bottomPath =
        {
            new Vector2(centerX - outerHalfWidth, bounds.min.y),
            new Vector2(centerX + outerHalfWidth, bounds.min.y),
            new Vector2(centerX + outerHalfWidth, bottomShoulder),
            new Vector2(centerX + openingHalfWidth, gapBottom),
            new Vector2(centerX - openingHalfWidth, gapBottom),
            new Vector2(centerX - outerHalfWidth, bottomShoulder)
        };

        polygon.enabled = true;
        polygon.isTrigger = true;
        polygon.offset = Vector2.zero;
        polygon.pathCount = 2;
        polygon.SetPath(0, topPath);
        polygon.SetPath(1, bottomPath);
    }
}
