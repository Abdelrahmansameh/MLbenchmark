using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Distributions;
using System;
using TMPro;
using System.Linq;

public class Population
{
    public int num_players;
    public GameObject[] pop;
    public double[] champion;
    public float champion_score;
    public int num_parents;
    public int num_offsprings;
    public int num_weights;
    public float mutation_rate;
    public float avg_score;

    public Population(int numplayer, GameObject[] population, int numparents, int numweights, int parents)
    {
        num_players = numplayer;
        pop = population;
        num_offsprings = numplayer;
        num_weights = numweights;
        num_parents = parents;
    }

    GameObject[] select_mating_pool()
    {
        GameObject[] ret = new GameObject[num_parents];

        Array.Sort<GameObject>(pop, 
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
        for (int i = 0; i < num_parents; i++)
        {
            ret[i] = pop[i];
        }
        return ret;
    }

    public double[,] SexAndMutations()
    {
        float avg = 0;
        foreach(GameObject player in pop)
        {
            avg += player.transform.position.x;
        }
        avg_score = avg / num_players;
        double[, ] offsprings = new double[num_offsprings, num_weights];
        System.Random random = new System.Random();
        GameObject[] parents = select_mating_pool();
        champion = parents[0].GetComponent<geneticAgent>().individual;
        champion_score = parents[0].transform.position.x;
        for (int counter = 0; counter < num_offsprings; counter++)
        {
            int i = random.Next(0, num_parents);
            int j = random.Next(0, num_parents);
            while(i == j)
            {
                j = random.Next(0, num_parents);
            }
            int crossover = random.Next(0, num_weights);
            var normal = new Normal(0, 1);
            for (int gene = 0; gene < crossover; gene++)
            {
                offsprings[counter, gene] = parents[i].GetComponent<geneticAgent>().individual[gene];
                double tmp = normal.Sample();
                if (UnityEngine.Random.Range(0f, 1f) < mutation_rate)
                {
                    offsprings[counter, gene] += tmp;
                }
                    
            }
            for (int gene = crossover; gene < num_weights; gene++)
            {
                offsprings[counter, gene] = parents[j].GetComponent<geneticAgent>().individual[gene];
                double tmp = normal.Sample();
                if (UnityEngine.Random.Range(0f, 1f) < 0.2)
                {
                    offsprings[counter, gene] += tmp;
                }

            }


        }
        return offsprings;
    }
}

public class God : MonoBehaviour
{
    public int n_x;
    public int n_h;
    public int n_h2;
    public int n_y;

    public GameObject player;
    public GameObject spawnPoint;
    public GameObject platform;

    public int num_weights;
    public int pop_size = 50;
    public int num_generations = 100;
    public int num_parents = 12;

    public double[] champions;
    public float[] champion_scores;
    
    int gen_counter = 0;

    public int num_pops;
    Population[] current_pops;

    public TextMeshProUGUI scoreUI;
    public TextMeshProUGUI genUI;

    public string bestscorepath;
    public string championpath;
    public string scorepath;
    public string avgscorepath;

    // Start is called before the first frame update
    void Start()
    {
        string DateTime = System.DateTime.Now.ToString("dd-MM-yyyy HH.mm.ss");

        bestscorepath = @"Gscores\" + DateTime + "-bestscore.txt" ;
        championpath = @"Gscores\" + DateTime + "-champion.txt";
        scorepath = @"Gscores\" + DateTime + "-score.txt";
        avgscorepath = @"Gscores\" + DateTime + "-avgscore.txt";

        champion_scores = new float[num_pops];
        current_pops = new Population[num_pops];
        num_weights = n_x * n_h + n_h * n_h2 + n_h2 * n_y;
        for (int j = 0; j < num_pops; j++) {
            GameObject[] pop = new GameObject[pop_size];
            for (int i = 0; i < pop_size; i++)
            {
                GameObject p = Instantiate(player, spawnPoint.transform.position, Quaternion.identity);
                //p.GetComponent<PlayerControl>().speed = 10-i;
                p.GetComponent<PlayerControl>().id = i;
                p.GetComponent<geneticAgent>().Initialize_Random(n_x, n_h, n_h2, n_y);
                pop[i] = p;
            }
            current_pops[j] = new Population(pop_size, pop, num_parents, num_weights, num_parents);
            current_pops[j].mutation_rate = 0.5f;
        }
        gen_counter++;
    }

    // Update is called once per frame
    void Update()
    {
        Resources.UnloadUnusedAssets();
        scoreUI.text = "Best score: " + champion_scores.Max().ToString("0");
        genUI.text = "Generation: " + gen_counter.ToString("0");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p1 in players)
            {
                p1.GetComponent<PlayerControl>().Dead = true;
            }
        }


        if (gen_counter <= num_generations)
        {
            if (AllDead())
            {
                for (int j = 0; j < num_pops; j++)
                {
                    double[,] new_weights = current_pops[j].SexAndMutations();

                    if (gen_counter == 0)
                    {
                        champions = current_pops[j].champion;
                        champion_scores[j] = current_pops[j].champion_score;
                        string[] lines = new string[num_weights];
                        for (int i = 0; i < num_weights; i++)
                        {
                            lines[i] = champions[i].ToString("0.00");
                        }
                        System.IO.File.WriteAllLines(championpath, lines);
                        using (StreamWriter sw = File.CreateText(bestscorepath))
                        {
                            sw.WriteLine(champion_scores[j].ToString("0"));
                        }
                        using (StreamWriter sw = File.CreateText(scorepath))
                        {
                            sw.WriteLine(champion_scores[j].ToString("0"));
                        }
                        using (StreamWriter sw = File.CreateText(avgscorepath))
                        {
                            sw.WriteLine(current_pops[j].avg_score.ToString("0"));
                        }
                    }

                    else
                    {

                        if (current_pops[j].champion_score >= champion_scores[j])
                        {
                            champions = current_pops[j].champion;
                            champion_scores[j] = current_pops[j].champion_score;
                            string[] lines = new string[num_weights];
                            for (int i = 0; i < num_weights; i++)
                            {
                                lines[i] = champions[i].ToString("0.00");
                            }
                            System.IO.File.WriteAllLines(championpath, lines);
                        }

                        using (StreamWriter sw = File.AppendText(scorepath))
                        {
                            sw.WriteLine(current_pops[j].champion_score.ToString("0"));
                        }

                        using (StreamWriter sw = File.AppendText(bestscorepath))
                        {
                            sw.WriteLine(champion_scores[j].ToString("0"));
                        }

                        using (StreamWriter sw = File.AppendText(avgscorepath))
                        {
                            sw.WriteLine(current_pops[j].avg_score.ToString("0"));
                        }
                    }
                    GameObject[] pop = new GameObject[pop_size];
                    for (int i = 0; i < pop_size; i++)
                    {
                        GameObject p = Instantiate(player, spawnPoint.transform.position, Quaternion.identity);
                        //p.GetComponent<PlayerControl>().speed = 10-i;
                        p.GetComponent<PlayerControl>().id = i;
                        double[] tmp = new double[num_weights];
                        if (i == 0)
                        {
                            SpriteRenderer sprite = p.GetComponent<SpriteRenderer>();

                            sprite.color = Color.blue;

                            tmp = champions;
                        }
                        else
                        {
                            for (int j2 = 0; j2 < num_weights; j2++)
                            {
                                tmp[j2] = new_weights[i, j2];
                            }
                        }
                        p.GetComponent<geneticAgent>().Initialize_unfold(tmp, n_x, n_h, n_h2, n_y);
                        pop[i] = p;

                    }



                    foreach (GameObject p in current_pops[j].pop)
                    {
                        Destroy(p);
                    }


                    current_pops[j] = new Population(pop_size, pop, num_parents, num_weights, num_parents);
                    if (gen_counter < 10)
                    {
                        current_pops[j].mutation_rate = 0.5f;
                    }
                    else
                    {
                        current_pops[j].mutation_rate = 0.3f;
                    }
                }

                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject p1 in players)
                {
                    foreach (GameObject p2 in players)
                    {
                        Physics2D.IgnoreCollision(p1.GetComponent<BoxCollider2D>(), p2.GetComponent<BoxCollider2D>());
                    }
                    //GameObject player = players[0];
                    /*Vector3 spawnPlatformPosition = new Vector3(player.transform.position.x + 3, 0, 0);
                    Instantiate(platform, spawnPlatformPosition, Quaternion.identity);*/
                }

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

                gen_counter++;
            }
        }
    }

   bool AllDead()
    {
        bool ret = true;
        foreach (Population current_pop in current_pops)
        {
            foreach (GameObject p in current_pop.pop)
            {
                if (!p.GetComponent<PlayerControl>().Dead)
                {
                    ret = false;
                }
            }
        }
        return ret;
    }
}
