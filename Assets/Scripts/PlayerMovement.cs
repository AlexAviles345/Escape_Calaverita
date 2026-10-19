using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float fallSpeedLimit = -9f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField, Range(0.5f, 1f)] private float hitboxScale = 0.78f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private Camera mainCamera;
    private bool isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ConfigureHitbox();
        playerCollider = GetComponent<Collider2D>();
        mainCamera = Camera.main;
        rb.simulated = false;
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
        if (gameManager == null || gameManager.IsGameOver) return;

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
        if (other.CompareTag("Obstacle"))
            Die();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Obstacle"))
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

        CreateDeathExplosion();

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (playerCollider != null)
            playerCollider.enabled = false;

        rb.linearVelocity = Vector2.zero;
        GameManager.Instance?.GameOver();
    }

    private void CreateDeathExplosion()
    {
        GameObject explosion = new GameObject("PlayerDeathExplosion");
        explosion.transform.position = transform.position;

        ParticleSystem particles = explosion.AddComponent<ParticleSystem>();
        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        ParticleSystem.MainModule main = particles.main;
        main.duration = 0.45f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.55f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 6f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.3f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(1f, 0.85f, 0.15f),
            new Color(0.9f, 0.2f, 0.55f));
        main.gravityModifier = 0.6f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.useUnscaledTime = true;
        main.maxParticles = 24;
        main.stopAction = ParticleSystemStopAction.Destroy;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, 20)
        });

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.18f;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient fade = new Gradient();
        fade.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(1f, 0.9f, 0.2f), 0f),
                new GradientColorKey(new Color(0.65f, 0.12f, 0.75f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            });
        colorOverLifetime.color = fade;

        ParticleSystemRenderer particleRenderer = explosion.GetComponent<ParticleSystemRenderer>();
        particleRenderer.sortingOrder = 10;

        particles.Play();
    }
}
