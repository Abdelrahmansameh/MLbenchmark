using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnPlatform : MonoBehaviour
{
    private GameObject player;
    public GameObject[] platforms;
    public int spawnDistance;
    public float gap;


    public Vector3 spawnPlatformPosition;

    public GameObject playerSpawner;
    private spawnPlayer topPlayerFinder;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        playerSpawner = GameObject.FindGameObjectWithTag("_Scene");
        topPlayerFinder = playerSpawner.GetComponent<spawnPlayer>();
        player = topPlayerFinder.topPlayer;
        spawnPlatformPosition = player.gameObject.transform.position;
        //SpawnPlatforms(0);
    }
    // Update is called once per frame
    void Update()
    {
        print(spawnPlatformPosition);
        player = topPlayerFinder.topPlayer;
        float distanceToSpawn = Vector3.Distance(player.gameObject.transform.position, spawnPlatformPosition);
        if (distanceToSpawn < spawnDistance)
        {
            SpawnPlatforms(gap);
        }
    }

    void SpawnPlatforms(float g)
    {
        spawnPlatformPosition = new Vector3(spawnPlatformPosition.x + g, 0, 0);
        print(spawnPlatformPosition);
        Instantiate(platforms[0], spawnPlatformPosition, Quaternion.identity);
        //spawnPlatformPosition = player.gameObject.transform.position;
    }


}
