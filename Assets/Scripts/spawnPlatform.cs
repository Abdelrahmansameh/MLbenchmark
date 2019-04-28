using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnPlatform : MonoBehaviour
{
    public GameObject player;
    public GameObject[] platforms;
    public int spawnDistance;
    public int gap;

    private Vector3 spawnPlatformPosition;


    // Start is called before the first frame update
    void Start()
    {
        spawnPlatformPosition = player.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToSpawn = Vector3.Distance(player.gameObject.transform.position, spawnPlatformPosition);
        if (distanceToSpawn < spawnDistance)
        {
            SpawnPlatforms();
        }
    }

    void SpawnPlatforms()
    {
        spawnPlatformPosition = new Vector3(spawnPlatformPosition.x + gap, 0, 0);
        Instantiate(platforms[0], spawnPlatformPosition, Quaternion.identity);
    }
}
