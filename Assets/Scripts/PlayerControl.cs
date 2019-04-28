using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.IO;

public class controls
{
    public int direction;
    public int jump;
}

public class PlayerControl : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float xinput;

    private Rigidbody2D rigidbody;

    public bool Grounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask Ground;

    private bool facingRight = true;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();    
    }

    // Update is called once per frame
    void Update()
    {
        controls mv = GetControls();

        Grounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, Ground);

        Move(mv.direction);

        Jump(mv.jump);
    }

    void Move(int dir)
    {
        int xinput = dir;
        rigidbody.velocity = new Vector2(xinput * speed, rigidbody.velocity.y);
        if ((facingRight == false && xinput > 0) || (facingRight == true && xinput < 0))
        {
            Flip();
        }
    }

    void Jump(int jmp)
    {
        if ( (jmp == 1) && Grounded)
        {
            rigidbody.velocity = Vector2.up * jumpForce;
        }
    }
    void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    controls GetControls()
    {
        Uri serverUri = new Uri("http://127.0.0.1:5000/getmove");
        var request = (HttpWebRequest)WebRequest.Create(serverUri);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        controls info = JsonUtility.FromJson<controls>(jsonResponse);
        return info;
    }
}
