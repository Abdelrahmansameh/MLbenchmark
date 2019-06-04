using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public int target;
    public GameObject origin;
    public int timer;
    public int max_time;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;

        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (p.GetComponent<FighterControl>().id != target)
            {
                Physics2D.IgnoreCollision(gameObject.GetComponent<CircleCollider2D>(), p.transform.Find("Standing").gameObject.GetComponent<BoxCollider2D>());
                Physics2D.IgnoreCollision(gameObject.GetComponent<CircleCollider2D>(), p.transform.Find("Crouching").gameObject.GetComponent<BoxCollider2D>());
                print(p.GetComponent<FighterControl>().id);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer++;
        if (timer > max_time)
        {
            origin.GetComponent<FighterControl>().bullet_counter--;
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == ("Player"))
        {
            collision.gameObject.GetComponent<FighterControl>().shot = true;
            origin.GetComponent<FighterControl>().bullet_counter--;
            Destroy(gameObject);
        }
    }

}
