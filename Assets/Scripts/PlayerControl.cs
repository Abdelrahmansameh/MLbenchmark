using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using TMPro;

public class controls
{
    public float direction;
    public int jump;
}

public class Info
{
    public int id;
    public Info(int i)
    {
        id = i;
    }
}

public class PlayerControl : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public int id;

    private Rigidbody2D rigidbody;

    public bool Grounded;
    public bool aiMode;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask Ground;
    public LayerMask laser;

    public bool Dead;
    public float deadBarrier;

    public bool stuck;

    private bool facingRight = true;

    public int queueSize; 
    public Queue queue = new Queue();

    public bool check_again;

    public Sprite running;
    public Sprite standing;
    public Sprite champion_running;
    public Sprite champion_standing;
    public Sprite jumping;
    public Sprite champion_jumping;

    public bool champion = false;
    float best_score = 0;

    public TextMeshProUGUI scoreUI;
    public TextMeshProUGUI epUI;
    int death_counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        check_again = true;
        for (float i = 0; i < 10; i++ ){
            queue.Enqueue(0f + (i /1000) );
        }
        rigidbody = GetComponent<Rigidbody2D>();
        Dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!aiMode) { 
        scoreUI.text = "Best score: " + best_score.ToString("0");
        epUI.text = "Death Counter: " + death_counter.ToString("0");
        }
        controls mv = new controls();
	if(aiMode)
    {
            if (!Dead)
            {
                mv = GetControls();
            }
            else
            {
                mv = new controls();
                mv.direction = 0;
                mv.jump = 0;
            }
    }
	else
    {
        
	    mv = new controls();
	    mv.direction = Input.GetAxis("Horizontal");
	    if (Input.GetKeyDown(KeyCode.UpArrow))
        {
		    mv.jump = 1;
	    }
	    else
        {
		    mv.jump = 0;
	    }

	}
        Grounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, Ground);


        Move(mv.direction);


        Jump(mv.jump);

        AmIDead();

        if (Dead)
        {
            death_counter++;
            if (!aiMode)
            {
                float score = gameObject.transform.position.x;
                if (score >= best_score)
                {
                    best_score = score;
                }

                gameObject.transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position;
                Dead = false;
                foreach (GameObject plat in GameObject.FindGameObjectsWithTag("platform"))
                {
                    Destroy(plat);
                }

                foreach (GameObject gap in GameObject.FindGameObjectsWithTag("gap"))
                {
                    Destroy(gap);
                }
                GameObject.FindGameObjectWithTag("_Scene").GetComponent<spawnPlatform>().Initialize();
                GameObject.FindGameObjectWithTag("laser").GetComponent<Laser>().Initialize();
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                check_again = true;
            }
            else
            {
                rigidbody.velocity = Vector2.zero;
            }
        }
        queue.Dequeue();
        queue.Enqueue(gameObject.transform.position.x);
    }

    void Move(float dir)
    {
        float xinput = dir;
        if (dir != 0)
        {
            if (!Grounded)
            {
                if (champion)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = champion_jumping;

                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = jumping;
                }

            }
            else
            {
                if (champion)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = champion_running;

                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = running;
                }
            }
        }
        else
        {
            if (!Grounded)
            {
                if (champion)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = champion_jumping;

                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = jumping;
                }
            }
            else
            {
                if (champion)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = champion_standing;

                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = standing;
                }
            }
        }
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
        controls ret = new controls();
        //Vector<double> foo = gameObject.GetComponent<geneticAgent>().output;
        Vector<double> foo = gameObject.GetComponent<hillAgent>().output;

        if (foo[0]> 0.5)
        {
            ret.jump = 1;
        }
        else
        {
            ret.jump = 0;
        }
        if (foo[1] > 0.5)
        {
            ret.direction = 1;
        }
        else
        {
            ret.direction = 0;
        }
        return ret;
    }

    void AmIDead()
    {
        if (check_again)
        {
            if (gameObject.transform.position.y <= deadBarrier || gameObject.transform.position.x >= 1000)
            {
                Dead = true;
            }
            else
            {
                Dead = false;
            }

            if (Physics2D.OverlapCircle(gameObject.transform.position, 0.2f, laser))
            {
                Dead = true;
                check_again = false;
            }
            if (Dead)
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }


}
