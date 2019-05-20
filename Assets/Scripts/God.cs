using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Linq;

public class CustomArray<T>
{
    public T[] GetColumn(T[,] matrix, int columnNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(0))
                .Select(x => matrix[x, columnNumber])
                .ToArray();
    }

    public T[] GetRow(T[,] matrix, int rowNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowNumber, x])
                .ToArray();
    }
}

public class Population
{
    public int num_players;
    public GameObject[] pop;
    public int num_parents;
    public int num_offsprings;
    public int num_weights;

    public Population(int numplayer, GameObject[] population, int numparents, int numweights)
    {
        num_players = numplayer;
        pop = population;
        num_offsprings = num_players;
        num_weights = numweights;
    }

    GameObject[] select_mating_pool()
    {
        GameObject[] ret = new GameObject[num_parents];

        Array.Sort<GameObject>(pop, 
            delegate (GameObject m, GameObject n)
            {
                if (m.transform.position.x <= n.transform.position.x)
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
        for (int counter = 0; counter < num_offsprings; counter++)
        {
            int i = random.Next(0, num_parents);
            int j = random.Next(0, num_parents);
            if (i != j)
            {
                int crossover = random.Next(0, num_weights);
                for (int gene = 0; gene < crossover; gene++)
                {
                    offsprings[counter, gene] = parents[i].GetComponent<geneticAgent>().individual[gene];
                   
                }
                for (int gene = crossover; gene < num_weights; gene++)
                {
                    offsprings[counter, gene] = parents[j].GetComponent<geneticAgent>().individual[gene];

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
    int gen_counter = 0;
    Population current_pop;
    // Start is called before the first frame update
    void Start()
    {
        print(n_x);
        num_weights = n_x * n_h + n_h * n_h2 + n_h2 * n_y;
        print(num_weights);
        GameObject[] pop = new GameObject[pop_size];
        for (int i = 0; i < pop_size; i++)
        {
            GameObject p = Instantiate(player, spawnPoint.transform.position, Quaternion.identity);
            //p.GetComponent<PlayerControl>().speed = 10-i;
            p.GetComponent<PlayerControl>().id = i;
            p.GetComponent<geneticAgent>().Initialize_Random(n_x, n_h, n_h2, n_y);
            pop[i] = p;
        }
        print(pop);
        current_pop = new Population(pop_size, pop, num_parents,num_weights);
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
                GameObject[] pop = new GameObject[pop_size];
                for (int i = 0; i < pop_size; i++)
                {
                    GameObject p = Instantiate(player, spawnPoint.transform.position, Quaternion.identity);
                    //p.GetComponent<PlayerControl>().speed = 10-i;
                    p.GetComponent<PlayerControl>().id = i;
                    double[] tmp = new double[num_weights];
                    for (int j = 0; j < num_weights; j++)
                    {
                        tmp[j] = new_weights[i, j];
                    }
                    p.GetComponent<geneticAgent>().Initialize_unfold(tmp, n_x, n_h, n_h2, n_y);
                    pop[i] = p;
                }

                foreach (GameObject p in current_pop.pop)
                {
                        print(p.GetComponent<PlayerControl>().Dead);
                        Destroy(p);
                }
                current_pop = new Population(pop_size, pop, num_parents, num_weights);
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
