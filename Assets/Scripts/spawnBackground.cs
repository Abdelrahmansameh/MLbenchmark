using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnBackground : MonoBehaviour
{
    private GameObject player;
    public GameObject background;
    public int spawnDistance;
    private GameObject playerSpawner;
    public spawnPlayer topPlayerFinder;
    private Vector3 spawnPlatformPosition;


    // Start is called before the first frame update
    void Start()
    {
        playerSpawner = GameObject.FindGameObjectWithTag("_Scene");
        topPlayerFinder = playerSpawner.GetComponent<spawnPlayer>();
        player = topPlayerFinder.topPlayer;
        spawnPlatformPosition = player.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        player = topPlayerFinder.topPlayer;
        float distanceToSpawn = Vector3.Distance(player.gameObject.transform.position, spawnPlatformPosition);
        if (distanceToSpawn < spawnDistance)
        {
            SpawnPlatforms();
        }
    }

    void SpawnPlatforms()
    {
        spawnPlatformPosition = new Vector3(spawnPlatformPosition.x + background.GetComponent<Collider2D>().bounds.size.x, 0, 0);
        Instantiate(background, spawnPlatformPosition, Quaternion.identity);
    }
}
