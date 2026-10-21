using UnityEngine;

public class LapidaSpawner : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject[] lapidaPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float minGapY = -1.25f;
    [SerializeField] private float maxGapY = 1.35f;

    [Header("Tamaño")]
    [SerializeField] private float minScale = 0.94f;
    [SerializeField] private float maxScale = 1.02f;

    [Header("Separación (unidades de mundo)")]
    [SerializeField] private float minSpacing = 5f;
    [SerializeField] private float maxSpacing = 6.5f;

    [Header("Velocidad")]
    [SerializeField] private float startSpeed = 3f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float speedIncreasePerSecond = 0.035f;

    private float travelled;
    private float nextSpacing;

    private void Start()
    {
        Lapida.Speed = startSpeed;
        nextSpacing = Random.Range(minSpacing, maxSpacing);
        Spawn(); // primera lápida inmediata
    }

    private void Update()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsGameStarted ||
            GameManager.Instance.IsGameOver ||
            GameManager.Instance.IsPaused)
            return;

        Lapida.Speed = Mathf.MoveTowards(
            Lapida.Speed, maxSpeed, speedIncreasePerSecond * Time.deltaTime);

        travelled += Lapida.Speed * Time.deltaTime;

        if (travelled >= nextSpacing)
        {
            Spawn();
            travelled = 0f;
            nextSpacing = Random.Range(minSpacing, maxSpacing);
        }
    }

    private void Spawn()
    {
        if (lapidaPrefabs == null || lapidaPrefabs.Length == 0) return;

        GameObject prefab = lapidaPrefabs[Random.Range(0, lapidaPrefabs.Length)];
        Camera mainCamera = Camera.main;
        float cameraRight = mainCamera != null
            ? mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f)).x
            : 8f;
        float configuredX = spawnPoint != null ? spawnPoint.position.x : cameraRight + 1.5f;
        float spawnX = Mathf.Min(configuredX, cameraRight + 1.5f);

        GameObject lapida = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        lapida.name = "Obstaculo_Arriba_Abajo";

        float s = Random.Range(minScale, maxScale);
        lapida.transform.localScale = prefab.transform.localScale * s;

        Lapida movement = lapida.GetComponent<Lapida>();
        float localGapCenter = movement != null ? movement.GapCenterLocal : -1f;
        float desiredGapY = Random.Range(minGapY, maxGapY);
        float scaledGapCenter = localGapCenter * lapida.transform.lossyScale.y;

        lapida.transform.position = new Vector3(
            spawnX,
            desiredGapY - scaledGapCenter,
            0f);
    }
}
