using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnPlayer : MonoBehaviour
{
    public GameObject player;
    public GameObject spawnPoint;
    public GameObject platform;
    public int numberOfPlayers;
    public GameObject topPlayer;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            
           GameObject p = Instantiate(player, spawnPoint.transform.position, Quaternion.identity);
            //p.GetComponent<PlayerControl>().speed = i;
            p.GetComponent<PlayerControl>().id = i;
        }
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < numberOfPlayers; i++)
        {
            for (int j = 0; j < numberOfPlayers; j++)
            {
                if (i != j)
                {
                    Physics2D.IgnoreCollision(Players[i].GetComponent<BoxCollider2D>(), Players[j].GetComponent<BoxCollider2D>());
                }
            }
        }
        Vector3 spawnPlatformPosition = new Vector3(player.transform.position.x + 3,0 , 0);
        Instantiate(platform, spawnPlatformPosition, Quaternion.identity);
        topPlayer = findTopPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        topPlayer = findTopPlayer(); 
    }

    public GameObject findTopPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject ret = players[0];
        float max = -1000f;
        for (int i = 0; i < numberOfPlayers; i++)
        {
            GameObject p = players[i];
            if (p.gameObject.transform.position.x > max)
            {
                max = p.gameObject.transform.position.x;
                ret = p;
            }
        }
        return ret;
    }
}
