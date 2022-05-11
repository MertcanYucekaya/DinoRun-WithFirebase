using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    ObstacleSpawnerSc obstacleSpawnerSc;

    private static LevelGenerator instance;

    private const float PLAYER_DISTANCE_SPAWN_LEVEL_PART = 70f;

    [SerializeField] private Transform levelPart_Start;

    [SerializeField] private List<Transform> levelPartDesert;
    [SerializeField] private List<Transform> desertStraightPartList;
    [SerializeField] private GameObject desertBg;

    [SerializeField] private List<Transform> levelPartSnow;
    [SerializeField] private List<Transform> snowStraightPartList;
    [SerializeField] private GameObject snowBg;

    [SerializeField] private List<Transform> levelPartForest;
    [SerializeField] private List<Transform> forestStraightPartList;
    [SerializeField] private GameObject forestBg;

    [SerializeField] private Transform backgroundContainer;

    [SerializeField] private Transform levelTransition;

    [SerializeField] private Transform player;
    [SerializeField] private Transform grid;

    [SerializeField] public List<Map> mapList = new List<Map>();

    public Map currentMap;

    public bool stopSpawning = false;
    private bool spawnTransition = false;
    public enum Map
    {
        Desert,
        Snow,
        Forest,
    }

    private Transform lastEndPosition;
    private int levelPartsSpawned;
    private void Start()
    {
        obstacleSpawnerSc = GameObject.Find("ObastacleSpawner").GetComponent<ObstacleSpawnerSc>();
        RandomMap();
    }
    private void Awake()
    {
        instance = this;
        lastEndPosition = levelPart_Start.Find("EndPosition");
        




        //int startingSpawnLevelParts = 1;

        //for (int i = 0; i < startingSpawnLevelParts; i++)
        //{
        //    SpawnLevelPart();
        //}
    }
    private void RandomMap()
    {
        List<Map> tmpList = new List<Map>(mapList);
        currentMap = tmpList[Random.Range(0, tmpList.Count)];

        List<Transform> difficultyLevelPartList;
        Transform chosenStraightLevelPart = null;

        difficultyLevelPartList = GetMapPartStraight();
        StartCoroutine(obstacleSpawnMethod());
        chosenStraightLevelPart = difficultyLevelPartList[Random.Range(0, difficultyLevelPartList.Count)];

        Transform lastStraightLevelPartTransform = SpawnLevelPart(chosenStraightLevelPart, lastEndPosition.position);
        lastEndPosition = lastStraightLevelPartTransform.Find("EndPosition");
        levelPartsSpawned++;
    }

    private void Update()
    {
        if (Vector3.Distance(player.transform.position, lastEndPosition.position) < PLAYER_DISTANCE_SPAWN_LEVEL_PART && !stopSpawning)
        {
            // Spawn another level part
            SpawnLevelPart();
        }
    }
    public void ChangeMap()
    {
        List<Map> tmpList = new List<Map>(mapList);
        tmpList.Remove(currentMap);

        currentMap = tmpList[Random.Range(0, tmpList.Count)];

        spawnTransition = true;
        stopSpawning = true;

        SpawnLevelPart();
        //obstacleSpawnerSc.currentMapMethod();
    }
    public void SetBackground()
    {
        GameObject background = GetBackground();

        foreach (Transform item in backgroundContainer)
        {
            if(item.gameObject == background)
            {
                item.gameObject.SetActive(true);
                continue;
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }
    }
    public GameObject GetBackground()
    {
        switch (currentMap)
        {
            case Map.Desert:
                return desertBg;
            case Map.Snow:
                return snowBg;
            case Map.Forest:
                return forestBg;
            default:
                return desertBg;
        }
    }
    public void SpawnLevelPart()
    {
        List<Transform> difficultyLevelPartList;
        Transform chosenLevelPart = null;
        Transform chosenStraightLevelPart = null;
        if (spawnTransition)
        {
            chosenLevelPart = levelTransition;

            Transform lastLevelPartTransform = SpawnLevelPart(chosenLevelPart, lastEndPosition.position);
            lastEndPosition = lastLevelPartTransform.Find("EndPosition");
            levelPartsSpawned++;


            difficultyLevelPartList = GetMapPartStraight();
            chosenStraightLevelPart = difficultyLevelPartList[Random.Range(0, difficultyLevelPartList.Count)];

            Transform lastStraightLevelPartTransform = SpawnLevelPart(chosenStraightLevelPart, lastEndPosition.position);
            lastEndPosition = lastStraightLevelPartTransform.Find("EndPosition");
            levelPartsSpawned++;

            spawnTransition = false;
            stopSpawning = false;
        }
        else
        {
            if (Random.value > 0.05f)
            {
                difficultyLevelPartList = GetMapPartStraight();
            }
            else
            {
                difficultyLevelPartList = GetMapPart();
            }

            chosenLevelPart = difficultyLevelPartList[Random.Range(0, difficultyLevelPartList.Count)];

            Transform lastLevelPartTransform = SpawnLevelPart(chosenLevelPart, lastEndPosition.position);
            lastEndPosition = lastLevelPartTransform.Find("EndPosition");
            levelPartsSpawned++;
        }
    }

    private Transform SpawnLevelPart(Transform levelPart, Vector2 spawnPosition)
    {
        Transform levelPartTransform = Instantiate(levelPart, grid);

        levelPartTransform.transform.position = spawnPosition;

        return levelPartTransform;
    }

    public static int GetLevelPartsSpawned()
    {
        return instance.levelPartsSpawned;
    }
    public List<Transform> GetMapPart()
    {
        switch (currentMap)
        {
            case Map.Desert:
                return levelPartDesert;
            case Map.Snow:
                return levelPartSnow;
            case Map.Forest:
                return levelPartForest;
            default:
                
                return levelPartDesert;
        }
    }
    public List<Transform> GetMapPartStraight()
    {
        switch (currentMap)
        {
            case Map.Desert:
                return desertStraightPartList;
            case Map.Snow:
                return snowStraightPartList;
            case Map.Forest:
                return forestStraightPartList;
            default:
                return desertStraightPartList;
        }
    }
    IEnumerator obstacleSpawnMethod()
    {
        while (true)
        {
            if (currentMap == Map.Desert)
            {
                obstacleSpawnerSc.desertObstacleSpawn();
            }
            else if (currentMap == Map.Snow)
            {
                obstacleSpawnerSc.snowObstacleSpawn();
            }
            else if (currentMap == Map.Forest)
            {
                obstacleSpawnerSc.forestObstacleSpawn();
            }
            else
            {
                obstacleSpawnerSc.desertObstacleSpawn();
            }
            yield return new WaitForSeconds(obstacleSpawnerSc.instantDurationMethod());
        }
        
    }
}
