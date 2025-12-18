using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Note Prefab")]
    public GameObject notePrefab;

    [Header("Spawn Timing (sec)")]
    public float minInterval = 0.3f;
    public float maxInterval = 1.0f;

    [Header("Note Fall Speed")]
    public float minSpeed = 0.01f;
    public float maxSpeed = 0.1f;

    [Header("Note Height (Scale Y)")]
    public float minHeight = 0.01f;  // 원하는 최소 높이
    public float maxHeight = 0.10f;  // 원하는 최대 높이

    [Header("Targets (Keys)")]
    public Transform[] spawnPoints;  // C/D/E/F/G_SpawnPoint

    private float timer = 0f;
    private float currentInterval = 0f;

    void Start()
    {
        currentInterval = Random.Range(minInterval, maxInterval);
    }

    void Update()
    {
        if (notePrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= currentInterval)
        {
            SpawnNote();
            timer = 0f;
            currentInterval = Random.Range(minInterval, maxInterval);
        }
    }

    void SpawnNote()
    {
        // 1) 어느 키 위에서 떨어질지 랜덤 선택
        int index = Random.Range(0, spawnPoints.Length);
        Transform sp = spawnPoints[index];

        if (sp == null)
        {
            Debug.LogWarning($"NoteSpawner: spawnPoints[{index}] 가 null 입니다.");
            return;
        }

        Vector3 spawnPos = sp.position;

        Debug.Log($"Spawn from {sp.name} at world {spawnPos}");

        // 2) 노트 생성 (★ 꼭 sp.position 사용!)
        GameObject note = Instantiate(notePrefab, spawnPos, Quaternion.identity);

        // 3) 스케일 설정: x,z 고정, y만 랜덤
        float h = Random.Range(minHeight, maxHeight);
        note.transform.localScale = new Vector3(0.02f, h, 0.03f);

        // 4) 낙하 속도 설정
        var falling = note.GetComponent<FallingNote>();
        if (falling != null)
        {
            falling.fallSpeed = Random.Range(minSpeed, maxSpeed);
        }
    }
}
