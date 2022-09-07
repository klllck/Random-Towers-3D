using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    [Header("Parameters")]
    [SerializeField] private float waveCountdown;
    [SerializeField] private float spawnCountdown;
    [SerializeField] private int waveEnemyCount;
    [SerializeField, Range(1, 2)] private int maxTypes;
    [SerializeField] private Difficulties startDifficulty = Difficulties.easy;

    [Header("Setup")]
    [SerializeField] private GameObject waypointPrefab;
    [SerializeField] private List<GameObject> enemyEasyPrefabs;
    [SerializeField] private List<GameObject> enemyNormalPrefabs;
    [SerializeField] private List<GameObject> enemyHardPrefabs;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI waveCountdownText;

    [HideInInspector] public int wavesPassed;
    private List<Coroutine> coroutines;

    private GameObject[] pathObj;
    [HideInInspector] public GameObject paths;

    [HideInInspector] public float currentWaveCountdown;
    private Vector3 offset;

    private void Update()
    {
        if (currentWaveCountdown > 0)
        {
            currentWaveCountdown -= Time.deltaTime;
        }
    }

    public void CreatePaths(GameObject[] pathObj)
    {
        this.pathObj = pathObj;
        paths = new GameObject("Paths");
        var yOffset = pathObj[0].GetComponent<MeshFilter>().mesh.bounds.extents.y / 2f;
        offset = new Vector3(0, yOffset, 0);

        foreach (var waypointObj in pathObj)
        {
            var waypoint = Instantiate(waypointPrefab, waypointObj.transform.position + offset, waypointObj.transform.rotation);
            waypoint.transform.parent = paths.transform;
        }
        coroutines = new List<Coroutine>();
        coroutines.Add(StartCoroutine(SpawnWave()));
    }

    public IEnumerator SpawnWave()
    {
        currentWaveCountdown = waveCountdown;
        yield return new WaitForSeconds(waveCountdown);

        List<GameObject> enemies = new List<GameObject>();

        switch (startDifficulty)
        {
            case Difficulties.easy:
                enemies = enemyEasyPrefabs;
                break;
            case Difficulties.normal:
                enemies = enemyNormalPrefabs;
                break;
            case Difficulties.hard:
                enemies = enemyHardPrefabs;
                break;
            case Difficulties.special:
                break;
            case Difficulties.infinite:
                break;
            default:
                break;
        }

        for (int i = 0; i < waveEnemyCount; i++)
        {
            int rnd = Random.Range(0, 2);
            var enemy = PoolManager.Instance.Get(enemies[rnd].name);
            enemy.transform.position = paths.transform.GetChild(0).position;
            enemy.transform.rotation = paths.transform.GetChild(0).rotation;
            enemy.GetComponent<Enemy>().SetStats(0);
            yield return new WaitForSeconds(spawnCountdown);
        }

        ReleaseWave();
    }

    public void ReleaseWave()
    {
        wavesPassed++;

        if (wavesPassed >= 50)
        {
            startDifficulty = Difficulties.hard;
        }
        else if (wavesPassed >= 25)
        {
            startDifficulty = Difficulties.normal;
        }

        coroutines.Add(StartCoroutine(SpawnWave()));
    }

    public void Reset()
    {
        foreach (var coroutine in coroutines)
        {
            StopCoroutine(coroutine);
        }
        wavesPassed = 0;
        startDifficulty = Difficulties.easy;
        Destroy(paths);
    }
}
