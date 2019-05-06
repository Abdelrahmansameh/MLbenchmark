using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private GameObject player;
    public int offset;

    public GameObject playerSpawner;
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
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(player.gameObject.transform.position.x + offset, -2, -10), Time.deltaTime * 100);
    }
}
