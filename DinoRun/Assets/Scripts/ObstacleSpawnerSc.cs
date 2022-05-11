using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnerSc : MonoBehaviour
{
    bool isStart = true;
    float instantDuration;
    public float instantDurationMin;
    public float instantDurationMax;
    public GameObject container;
    Container containerSc;
    LevelGenerator levelGenerator;
    [Header("Obstacle Object")]
    public GameObject[] desertObstacles;
    public GameObject[] snowObstacles;
    public GameObject[] forestObstacles;
    [Header("Obstacle Instant Center")]
    public GameObject[] desertObstacleCenter;
    public GameObject[] snowdObstacleCenter;
    public GameObject[] forestObstacleCenter;
    [Header("Laser")]
    public GameObject laser;
    public GameObject[] laserInstantCenter;
    int[] laserInstantPercent = new int[100];
    public int laserInstantPercentStart;
    LaserForDestroySc laserForDestroySc;


    void Start()
    {
        laserForDestroySc = GameObject.Find("LaserForDestroy").GetComponent<LaserForDestroySc>();
        levelGenerator = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>();
        for(int i = 0; i < laserInstantPercentStart; i++)
        {
            laserInstantPercent[i] = 1;
        }
        

    }
    public void laserInstantMethod()
    {
        int random = Random.Range(0, laserInstantPercent.Length);
        if (laserInstantPercent[random] == 1)
        {
            laserForDestroySc.laserForDestroyMethod();
            int randomC = Random.Range(0, 2);
            Instantiate(laser, laserInstantCenter[randomC].transform.position, Quaternion.Euler(0,180,0));
        }
    }
    public void laserPercentMethod()
    {
       
        if (laserInstantPercentStart < 100)
        {
            laserInstantPercentStart++;
            
        }
        for (int i = 0; i < laserInstantPercentStart; i++)
        {
            laserInstantPercent[i] = 1;
        }
    }
    public void desertObstacleSpawn()
    {
        if (isStart)
        {
            containerSc = container.GetComponent<Container>();
            isStart = false;
        }
        
        if (containerSc.start)
        {
            int random = Random.Range(0, desertObstacles.Length);
            Instantiate(desertObstacles[random], desertObstacleCenter[random].transform.position, Quaternion.identity).transform.SetParent(container.transform);
        }
        
    }
    public void snowObstacleSpawn()
    {
        if (isStart)
        {
            containerSc = container.GetComponent<Container>();
            isStart = false;
        }
        if (containerSc.start)
        {
            int random = Random.Range(0, snowObstacles.Length);
            Instantiate(snowObstacles[random], snowdObstacleCenter[random].transform.position, Quaternion.identity).transform.SetParent(container.transform);
        }
            
    }
    public void forestObstacleSpawn()
    {
        if (isStart)
        {
            containerSc = container.GetComponent<Container>();
            isStart = false;
        }
        if (containerSc.start)
        {
            int random = Random.Range(0, forestObstacles.Length);
            Instantiate(forestObstacles[random], forestObstacleCenter[random].transform.position, Quaternion.identity).transform.SetParent(container.transform);
        }
        
    }
    public float instantDurationMethod()
    {
        instantDuration=  Random.Range(instantDurationMin, instantDurationMax);
        return instantDuration;
    }
}
