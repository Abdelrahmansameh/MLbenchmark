﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = new Vector2(speed, 0);

        Initialize();

    }

    public void Initialize()
    {
        Vector2 spawnpoint = new Vector2(GameObject.FindGameObjectWithTag("Respawn").transform.position.x - 20, gameObject.transform.position.y);
        //print(spawnpoint);
        gameObject.transform.position = spawnpoint;

        //Color newColor = new Color(Random.value, Random.value, Random.value, 1.0f);

//        gameObject.GetComponent<SpriteRenderer>().color = newColor;

    }
    // Update is called once per frame
    void Update()
    {

    }
}
