using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float fallSpeedLimit = -9f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField, Range(0.5f, 1f)] private float hitboxScale = 0.78f;

    [Header("Efectos")]
    [SerializeField] private ParticleSystem deathExplosion;
    [SerializeField] private AudioClip deathSound;
    [SerializeField, Range(0f, 1f)] private float deathSoundVolume = 0.75f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private SpriteRenderer playerVisual;
    private Camera mainCamera;
    private bool isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ConfigureHitbox();
        playerCollider = GetComponent<Collider2D>();
        playerVisual = GetComponentInChildren<SpriteRenderer>();
        mainCamera = Camera.main;
        rb.simulated = false;

        if (deathExplosion != null)
            deathExplosion.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void ConfigureHitbox()
    {
        PolygonCollider2D polygon = GetComponent<PolygonCollider2D>();
        if (polygon == null) return;

        for (int pathIndex = 0; pathIndex < polygon.pathCount; pathIndex++)
        {
            Vector2[] points = polygon.GetPath(pathIndex);
            for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
                points[pointIndex] *= hitboxScale;

            polygon.SetPath(pathIndex, points);
        }
    }

    private void Start()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameStarted) return;

        rb.simulated = true;
        Jump();
    }

    private void Update()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null || gameManager.IsGameOver || gameManager.IsPaused) return;

        if (GameManager.WasActionPressed())
        {
            if (!gameManager.IsGameStarted)
            {
                gameManager.StartGame();
                rb.simulated = true;
            }

            Jump();
        }

        if (!gameManager.IsGameStarted) return;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            Mathf.Max(rb.linearVelocity.y, fallSpeedLimit));

        float rotation = Mathf.Lerp(-75f, 30f,
            Mathf.InverseLerp(fallSpeedLimit, jumpForce, rb.linearVelocity.y));
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(0f, 0f, rotation),
            rotationSpeed * Time.deltaTime);

        CheckScreenLimits(gameManager);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance != null &&
            !GameManager.Instance.IsPaused &&
            other.CompareTag("Obstacle"))
            Die();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (GameManager.Instance != null &&
            !GameManager.Instance.IsPaused &&
            other.collider.CompareTag("Obstacle"))
            Die();
    }

    private void CheckScreenLimits(GameManager gameManager)
    {
        if (mainCamera == null || playerCollider == null) return;

        float halfHeight = playerCollider.bounds.extents.y;
        float bottom = mainCamera.ViewportToWorldPoint(Vector3.zero).y + halfHeight;
        float top = mainCamera.ViewportToWorldPoint(Vector3.one).y - halfHeight;

        if (transform.position.y < bottom || transform.position.y > top)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        PlayDeathEffects();

        if (playerVisual != null)
            playerVisual.enabled = false;

        if (playerCollider != null)
            playerCollider.enabled = false;

        rb.linearVelocity = Vector2.zero;
        GameManager.Instance?.GameOver();
    }

    private void PlayDeathEffects()
    {
        if (deathExplosion != null)
        {
            deathExplosion.gameObject.SetActive(true);
            deathExplosion.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            deathExplosion.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            deathExplosion.Play(true);
        }

        if (deathSound != null)
        {
            Vector3 soundPosition = mainCamera != null
                ? mainCamera.transform.position
                : transform.position;
            AudioSource.PlayClipAtPoint(deathSound, soundPosition, deathSoundVolume);
        }
    }

}
