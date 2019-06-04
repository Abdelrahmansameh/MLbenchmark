using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using System.Linq;

public class hillGod : MonoBehaviour
{
    public int num_agents;
    public GameObject player;
    private GameObject spawnPoint;
    public string bestscorepath;
    public string championpath;
    public string scorepath;
    public string avgscorepath;
    int counter = 0;
    float best_score = 0;
    float avg_score = 0;

    public TextMeshProUGUI scoreUI;
    public TextMeshProUGUI epUI;

    // Start is called before the first frame update
    void Start()
    {
        string DateTime = System.DateTime.Now.ToString("dd-MM-yyyy HH.mm.ss");

        bestscorepath = @"Hscores\" + DateTime + "-bestscore.txt";
        championpath = @"Hscores\" + DateTime + "-champion.txt";
        scorepath = @"Hscores\" + DateTime + "-score.txt";
        avgscorepath = @"Hscores\" + DateTime + "-avgscore.txt";

        spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        for (int j = 0; j < num_agents; j++)
        {
            GameObject p = Instantiate(player, spawnPoint.transform.position, Quaternion.identity);
            //p.GetComponent<PlayerControl>().speed = 10-i;
            p.GetComponent<PlayerControl>().id = j;
        }
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Resources.UnloadUnusedAssets();
        scoreUI.text = "Best score: " + best_score.ToString("0");
        epUI.text = "Episode: " + counter.ToString("0");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p1 in players)
            {
                p1.GetComponent<PlayerControl>().Dead = true;
            }
        }

        if (AllDead())
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            Array.Sort<GameObject>(players,
            delegate (GameObject m, GameObject n)
            {
                if (m.transform.position.x < n.transform.position.x)
                {
                    return 1;
                }
                else if (m.transform.position.x == n.transform.position.x)
                {
                    return 0;
                }
                return -1;
            }
            );

            float score = players[0].transform.position.x;
            if (score >= best_score)
            {
                best_score = score;
                GameObject p = players[0];
                hillAgent h = p.GetComponent<hillAgent>();
                string[] lines = new string[h.policy.input_size];
                for (int i = 0; i < h.policy.input_size; i++)
                {
                    string s = "";
                    for (int j = 0; j < h.policy.output_size; j++)
                    {
                        s += h.best_policy_w[i, j].ToString("0.0000");
                        if (j < h.policy.output_size - 1)
                        {
                            s += ',';
                        }
                    }
                    lines[i] = s;
                }
                System.IO.File.WriteAllLines(championpath, lines);
            }

            float avg = 0;
            foreach (GameObject player in players)
            {
                avg += player.transform.position.x;
            }
            avg_score = avg / num_agents;

            if (counter == 0)
            {

                using (StreamWriter sw = File.CreateText(bestscorepath))
                {
                    sw.WriteLine(best_score.ToString("0"));
                }
                using (StreamWriter sw = File.CreateText(scorepath))
                {
                    sw.WriteLine(score.ToString("0"));
                }
                using (StreamWriter sw = File.CreateText(avgscorepath))
                {
                    sw.WriteLine(avg_score.ToString("0"));
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(bestscorepath))
                {
                    sw.WriteLine(best_score.ToString("0"));
                }
                using (StreamWriter sw = File.AppendText(scorepath))
                {
                    sw.WriteLine(score.ToString("0"));
                }
                using (StreamWriter sw = File.AppendText(avgscorepath))
                {
                    sw.WriteLine(avg_score.ToString("0"));
                }
            }
            foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
            {
                p.transform.position = spawnPoint.transform.position;
                p.GetComponent<PlayerControl>().Dead = false;
                p.GetComponent<PlayerControl>().check_again = true;
                p.GetComponent<SpriteRenderer>().enabled = true;
                p.GetComponent<hillAgent>().wait = false;
                p.GetComponent<PlayerControl>().champion = false;

            }
            players[0].GetComponent<PlayerControl>().champion = true;
            foreach (GameObject plat in GameObject.FindGameObjectsWithTag("platform"))
            {
                Destroy(plat);
            }

            foreach (GameObject gap in GameObject.FindGameObjectsWithTag("gap"))
            {
                Destroy(gap);
            }
            gameObject.GetComponent<spawnPlatform>().Initialize();
            GameObject.FindGameObjectWithTag("laser").GetComponent<Laser>().Initialize();
            counter++;
        }
    }
    bool AllDead()
    {
        bool ret = true;
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (!p.GetComponent<PlayerControl>().Dead)
            {
                ret = false;
            }
        }
        return ret;
    }
}
