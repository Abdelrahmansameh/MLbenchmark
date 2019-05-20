using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.IO;

public class recieveInfo
{
    public int N;
}

public class spawnPlayer : MonoBehaviour
{
    public GameObject player;
    public GameObject spawnPoint;
    public GameObject platform;
    public GameObject topPlayer;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject p in players)
        {
            foreach(GameObject q in players)
            {
                    Physics2D.IgnoreCollision(p.GetComponent<BoxCollider2D>(), q.GetComponent<BoxCollider2D>());
            }
        }
        Vector3 spawnPlatformPosition = new Vector3(player.transform.position.x + 3,0 , 0);
        Instantiate(platform, spawnPlatformPosition, Quaternion.identity);
        topPlayer = findTopPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        topPlayer = findTopPlayer(); 
    }

    public GameObject findTopPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject ret = players[0];
        float max = -1000f;
        foreach(GameObject p in players)
        {
            if (!p.GetComponent<PlayerControl>().Dead)
            {
                if (p.gameObject.transform.position.x > max)
                {
                    max = p.gameObject.transform.position.x;
                    ret = p;
                }
            }
        }
        return ret;
    }
}
