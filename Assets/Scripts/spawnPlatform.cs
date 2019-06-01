using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class spawnPlatform : MonoBehaviour
{
    private GameObject player;
    public GameObject[] platforms;
    public int spawnDistance;
    public float gap;


    public Vector3 spawnPlatformPosition;

    public GameObject playerSpawner;
    private spawnPlayer topPlayerFinder;

    public GameObject Gap;

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
        spawnPlatformPosition = new Vector3(0, 0,0);
        //SpawnPlatforms(0);
    }
    // Update is called once per frame
    void Update()
    {
        gap = UnityEngine.Random.Range(6f, 8f);
        player = topPlayerFinder.topPlayer;
        float distanceToSpawn = Vector3.Distance(player.gameObject.transform.position, spawnPlatformPosition);
        if (distanceToSpawn < spawnDistance)
        {
            SpawnPlatforms(gap);
        }
    }

    void SpawnPlatforms(float g)
    {
        System.Random random = new System.Random();
        int foo = random.Next(0, 2);
        if (spawnPlatformPosition == new Vector3(0, 0, 0))
        {
            foo = 0;
        }
        spawnPlatformPosition = new Vector3(spawnPlatformPosition.x + g, 0, 0);
        //print(spawnPlatformPosition);
        if (foo == 0)
        {
            GameObject plat = Instantiate(platforms[0], spawnPlatformPosition, Quaternion.identity);
            Instantiate(Gap, new Vector2(plat.transform.Find("UR").gameObject.transform.position.x + 0.2f, plat.transform.Find("UR").gameObject.transform.position.y), Quaternion.identity);
        }
        if (foo == 1){
            GameObject plat = Instantiate(platforms[1], spawnPlatformPosition, Quaternion.identity);
            GameObject block = plat.transform.Find("UpperBlock").gameObject;
            //print(block.transform.localPosition);
            float ba = UnityEngine.Random.Range(-1f, 0.8f);
            block.transform.localPosition =(new Vector3(ba, 0, 0)) + block.transform.localPosition;
            Instantiate(Gap, new Vector2(plat.transform.Find("UR").gameObject.transform.position.x + 0.2f, plat.transform.Find("UR").gameObject.transform.position.y), Quaternion.identity);

        }
        //spawnPlatformPosition = player.gameObject.transform.position;
    }


}
