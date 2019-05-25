using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Destroyer : MonoBehaviour
{
    private GameObject player;
    public int destroyDistance;

    private GameObject playerSpawner;
    private spawnPlayer topPlayerFinder;

    // Start is called before the first frame update
    void Start()
    {
        playerSpawner = GameObject.FindGameObjectWithTag("_Scene");
        topPlayerFinder = playerSpawner.GetComponent<spawnPlayer>();
        player = topPlayerFinder.topPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        player = topPlayerFinder.topPlayer;
        if (Math.Abs(player.transform.position.x - gameObject.transform.position.x) > destroyDistance )
        {
            Destroy(gameObject);
        }
    }
}
