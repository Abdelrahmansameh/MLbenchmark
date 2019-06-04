using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FighterControl : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public bool shot;
    public int team;
    public int max_bullets;
    public int bullet_counter;
    public int buller_timer;

    public float bulletSpeed;

    public int id;

    private Rigidbody2D rigidbody;

    public bool Grounded;
    public bool aiMode = false;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask Ground;
    public LayerMask laser;

    public bool Dead;
    private bool facingRight = true;

    public Sprite running;
    public Sprite standing;
    public Sprite champion_running;
    public Sprite champion_standing;
    public Sprite jumping;
    public Sprite champion_jumping;
    public Sprite ducking;

    public bool champion = false;
    float best_score = 0;

    int death_counter = 0;
    public int target;

    GameObject standCollider;
    GameObject crouchCollider;
    public bool crouch;

    public GameObject ennemy;
    public GameObject bullet;

    public bool alternate_controls = false;
    public TextMeshProUGUI scoreUI;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        Dead = false;
        standCollider = transform.Find("Standing").gameObject;
        crouchCollider = transform.Find("Crouching").gameObject;
        crouchCollider.SetActive(false);
        best_score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!aiMode)
        {
            if (team == 1)
            {
                scoreUI.text = "Player 1: " + best_score.ToString("0");
            }
            if (team == 2)
            {
                scoreUI.text = "Player 2: " + best_score.ToString("0");
            }
        }
        controls mv = new controls();

        if (aiMode)
        {
            if (!Dead)
            {
                //mv = GetControls();
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
            if (!alternate_controls) {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    mv.direction = 1;
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    mv.direction = -1;
                }
                else
                {
                    mv.direction = 0;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.D))
                {
                    mv.direction = 1;
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    mv.direction = -1;
                }
                else
                {
                    mv.direction = 0;
                }
            }
            if (!alternate_controls) { 
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                mv.jump = 1;
            }
            else
            {
                mv.jump = 0;
            }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    mv.jump = 1;
                }
                else
                {
                    mv.jump = 0;
                }
            }

        }
        Grounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, Ground);


        Move(mv.direction);


        Jump(mv.jump);

        AmIDead();

        Crouch();

        Shoot();

        if (Dead)
        {
            death_counter++;
            if (!aiMode)
            {
                if (team == 1)
                {
                    gameObject.transform.position = GameObject.FindGameObjectWithTag("Respawn1").transform.position;
                }
                if (team == 2)
                {
                    gameObject.transform.position = GameObject.FindGameObjectWithTag("Respawn2").transform.position;
                }
                shot = false;
                Dead = false;
            }
            else
            {
                rigidbody.velocity = Vector2.zero;
            }
        }
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
        if ((jmp == 1) && Grounded)
        {
            rigidbody.velocity = Vector2.up * jumpForce;

        }
    }


    void Crouch()
    {
        if (!aiMode)
        {
            if (!alternate_controls)
            {
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    standCollider.SetActive(false);
                    crouchCollider.SetActive(true);
                    gameObject.GetComponent<SpriteRenderer>().sprite = ducking;
                    crouch = true;

                }
                else
                {
                    crouchCollider.SetActive(false);
                    standCollider.SetActive(true);
                    crouch = false;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.S))
                {
                    standCollider.SetActive(false);
                    crouchCollider.SetActive(true);
                    gameObject.GetComponent<SpriteRenderer>().sprite = ducking;
                    crouch = true;

                }
                else
                {
                    crouchCollider.SetActive(false);
                    standCollider.SetActive(true);
                    crouch = false;
                }
            }
        }
    }

    void Shoot()
    {
        if (!alternate_controls) { 
            if (Input.GetKeyDown(KeyCode.RightControl) && bullet_counter <max_bullets)
            {
                GameObject bulletClone;
                if (crouch) { 
                    bulletClone = Instantiate(bullet, gameObject.transform.Find("shooter2").transform.position, transform.rotation);
                }
                else
                {
                    bulletClone = Instantiate(bullet, gameObject.transform.Find("shooter1").transform.position, transform.rotation);
                }
                bulletClone.GetComponent<bullet>().target = target;
                bulletClone.GetComponent<Rigidbody2D>().velocity = new Vector2(facingRight? 1 : -1, 0) * bulletSpeed;
                bulletClone.GetComponent<bullet>().max_time = buller_timer;
                bulletClone.GetComponent<bullet>().origin= gameObject;
                bullet_counter++;
                if (team == 1)
                {
                    bulletClone.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
                }
                if (team == 2)
                {
                    bulletClone.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && bullet_counter < max_bullets)
            {
                GameObject bulletClone ;
                if (crouch)
                {
                    bulletClone = Instantiate(bullet, gameObject.transform.Find("shooter2").transform.position, transform.rotation);
                }
                else
                {
                    bulletClone = Instantiate(bullet, gameObject.transform.Find("shooter1").transform.position, transform.rotation);
                }
                bulletClone.GetComponent<bullet>().target = target;
                bulletClone.GetComponent<Rigidbody2D>().velocity = new Vector2(facingRight ? 1 : -1, 0) * bulletSpeed;
                bulletClone.GetComponent<bullet>().max_time = buller_timer;
                bulletClone.GetComponent<bullet>().origin = gameObject;
                bullet_counter++;
                if (team == 1)
                {
                    bulletClone.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
                }
                if (team == 2)
                {
                    bulletClone.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                }
            }
        }
    }   
    void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    void AmIDead()
    {
        if (shot)
        {
            Dead = true;
            foreach(GameObject bullet in GameObject.FindGameObjectsWithTag("bullet")){
                bullet.GetComponent<bullet>().origin.GetComponent<FighterControl>().bullet_counter = 0;
                Destroy(bullet);
            }
            ennemy.GetComponent<FighterControl>().best_score ++;
            if (team == 1)
            {
                ennemy.transform.position = GameObject.FindGameObjectWithTag("Respawn2").transform.position;
                
            }
            if (team == 2)
            {
                ennemy.transform.transform.position = GameObject.FindGameObjectWithTag("Respawn1").transform.position;
            }

        }
    }

}
