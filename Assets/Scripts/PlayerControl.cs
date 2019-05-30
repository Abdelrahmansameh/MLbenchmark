using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

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

    private bool check_again;
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
            rigidbody.velocity = Vector2.zero;
        }
        queue.Dequeue();
        queue.Enqueue(gameObject.transform.position.x);
    }

    void Move(float dir)
    {
        float xinput = dir;
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
        Vector<double> foo = gameObject.GetComponent<geneticAgent>().output;
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
            if (gameObject.transform.position.y <= deadBarrier)
            {
                Dead = true;
            }
            else
            {
                Dead = false;
            }

            if (Physics2D.OverlapCircle(gameObject.transform.position, 1, laser))
            {
                Dead = true;
                check_again = false;
            }
        }
    }


}
