using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Distributions;
using System;



public class Population
{
    public int num_players;
    public GameObject[] pop;
    public double[] champion;
    public float champion_score;
    public int num_parents;
    public int num_offsprings;
    public int num_weights;
    public float mutation_rate1;
    public float mutation_rate2;

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
                if (UnityEngine.Random.Range(0f, 1f) < mutation_rate2)
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

    int num_weights;
    public int pop_size = 50;
    public int num_generations = 100;
    public int num_parents = 12;

    public double[] champion;
    public float champion_score;
    
    int gen_counter = 0;
    Population current_pop;
    // Start is called before the first frame update
    void Start()
    {
        num_weights = n_x * n_h + n_h * n_h2 + n_h2 * n_y;
        GameObject[] pop = new GameObject[pop_size];
        for (int i = 0; i < pop_size; i++)
        {
            GameObject p = Instantiate(player, spawnPoint.transform.position, Quaternion.identity);
            //p.GetComponent<PlayerControl>().speed = 10-i;
            p.GetComponent<PlayerControl>().id = i;
            p.GetComponent<geneticAgent>().Initialize_Random(n_x, n_h, n_h2, n_y);
            pop[i] = p;
        }
        current_pop = new Population(pop_size, pop, num_parents,num_weights, num_parents);
        current_pop.mutation_rate1 = 0.7f;
        current_pop.mutation_rate2 = 0.7f;
        gen_counter++;
    }

    // Update is called once per frame
    void Update()
    {
        if (gen_counter <= num_generations)
        {
            if (AllDead())
            {

                double[,] new_weights = current_pop.SexAndMutations();

                if (gen_counter == 0)
                {
                    champion = current_pop.champion;
                    champion_score = current_pop.champion_score;
                }

                else
                {
                    if (current_pop.champion_score >= champion_score)
                    {
                        champion = current_pop.champion;
                        champion_score = current_pop.champion_score;
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
                        tmp = champion;
                    }
                    else
                    {
                        for (int j = 0; j < num_weights; j++)
                        {
                            tmp[j] = new_weights[i, j];
                        }
                    }
                    p.GetComponent<geneticAgent>().Initialize_unfold(tmp, n_x, n_h, n_h2, n_y);
                    pop[i] = p;

                    }
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (GameObject p1 in players)
                    {
                    foreach (GameObject p2 in players)
                    {
                        Physics2D.IgnoreCollision(p1.GetComponent<BoxCollider2D>(), p2.GetComponent<BoxCollider2D>());
                    }
                    GameObject player = players[0];
                    /*Vector3 spawnPlatformPosition = new Vector3(player.transform.position.x + 3, 0, 0);
                    Instantiate(platform, spawnPlatformPosition, Quaternion.identity);*/
                }


                foreach (GameObject p in current_pop.pop)
                {
                        Destroy(p);
                }

                foreach (GameObject plat in GameObject.FindGameObjectsWithTag("platform")){
                    Destroy(plat);
                }

                foreach (GameObject gap in GameObject.FindGameObjectsWithTag("gap"))
                {
                    Destroy(gap);
                }

                gameObject.GetComponent<spawnPlatform>().Initialize();
                GameObject.FindGameObjectWithTag("laser").GetComponent<Laser>().Initialize();
                current_pop = new Population(pop_size, pop, num_parents, num_weights, num_parents);
                if (gen_counter < 10)
                {
                    current_pop.mutation_rate1 = 0.7f;
                    current_pop.mutation_rate2 = 0.5f;
                }
                else if (gen_counter < 20)
                {
                    current_pop.mutation_rate1 = 0.7f;
                    current_pop.mutation_rate2 = 0.3f;
                }
                else
                {
                    current_pop.mutation_rate1 = 0.7f;
                    current_pop.mutation_rate2 = 0.3f;
                }
                gen_counter++;
            }
        }
    }

   bool AllDead()
    {
        bool ret = true;
        foreach(GameObject p in current_pop.pop)
        {
            if (!p.GetComponent<PlayerControl>().Dead)
            {
                ret = false;
            }
        }
        return ret;
    }
}
