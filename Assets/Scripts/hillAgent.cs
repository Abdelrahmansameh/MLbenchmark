using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;

public class SLP
{
    // Algorithm Parameters 
    public double alpha_w = 0.01;
    public double alpha_b = 0.01;
    public double prob_reset = 0.1;
    public double scaling = 1     ;
    public int input_size;
    public int output_size;

    public Matrix<double> W;
    //public Vector<double> b;

    public SLP(int inputs, int outputs)
    {
        input_size = inputs;
        output_size = outputs;
        reset(this.scaling);
    }

    public void reset(double scaling)
    {
        // Resets the weights and biases to normal distribution with scaling
        this.W = scaling * Matrix<double>.Build.Random(this.input_size, this.output_size);
        // bias
        //this.b = scaling * Vector<double>.Build.Random(this.output_size);
    }

    public Vector<double> predict(Vector<double> x)
    {
        // Feedforward prediction of SLP.
        var a = x * this.W; // + this.b;
        return Sigmoid(a);
    }

    public SLP copy(bool modify)
    {
        // Creates a copy of this SLP, with possible modification.
        SLP b = new SLP(this.input_size, this.output_size);
        b.W = this.W.Clone();
        //b.b = this.b.Clone();

        if (modify)
        {
            b.modify(this.alpha_w, this.alpha_b, this.prob_reset);
        }
        return b;
    }
    public void modify(double alpha_w, double alpha_b, double prob_reset)
    {
        Normal normal = new Normal(0, 1);

        // Adds perturbation to weights and biases, scaled by alphas, with random reset probability.
        if (normal.Sample() < prob_reset)
        {
            this.reset(this.scaling);
        }

        // Make a random adjustment to the weight matrix.
        this.W = this.W + alpha_w * Matrix<double>.Build.Random(this.input_size, this.output_size);
        this.b = this.b + alpha_b * Vector<double>.Build.Random(this.output_size);
    }

    Vector<double> Sigmoid(Vector<double> W)
    {
        return 1 / (1 + (1 / W.PointwiseExp()));
    }
}


public class hillAgent : MonoBehaviour
{
    //Simple Hill Climbing Agent
    public int max_episode_length = 50;
    public int num_episodes_per_test = 100;
    public int episode_counter;
    public double R, avg_R;
    public int size_obs_space, size_action_space, step_counter, test_counter, test_success;
    private Vector<double> memory;
    public double alpha_init, alpha, alpha_decay;
    private SLP policy;
    private SLP policy_trial;
    public GameObject spawnPoint;
    public float blockRadius = 0.5f;
    public Vector<double> output;

    public hillAgent(int obs_space, int act_space, int max_episode_length, int num_episodes_per_test, double alpha)
    {
        // Size of observation and action spaces
        size_obs_space = obs_space;
        size_action_space = act_space;

        // Max length of an episode 
        this.max_episode_length = max_episode_length;

        // Number of episodes per test
        this.num_episodes_per_test = num_episodes_per_test;



        // Alpha (step size / learning rate)
        this.alpha_init = alpha;
        this.alpha = this.alpha_init;




    }

    public void update_policy(double reward)
    {
        // Update the return for the episode
        R = reward;

        // Counting (each step of the episode of max length T)
        step_counter += 1;

        // If end of epsiode ...
        if (step_counter >= max_episode_length ||  gameObject.GetComponent<PlayerControl>().Dead)
        {
            // End of the episode 
            memory.Add(R);
            // Reset
            step_counter = 0;
            gameObject.transform.position = spawnPoint.transform.position;
            gameObject.GetComponent<PlayerControl>().Dead = false;
            R = 0;
            alpha = alpha * alpha_decay;
            episode_counter++;
        }

        // If end of the set of tests 
        if (memory.Count >= num_episodes_per_test)
        {
            // Calculate the average return per episode
            double average_R = memory.Sum() / num_episodes_per_test;
            test_counter += 1;

            if (average_R > avg_R)
            {
                // Accept
                avg_R = average_R;
                policy = policy_trial.copy(false);
                test_success += 1;
            }

            else
            {
                // Reject (modify old policy)
                this.policy_trial = this.policy.copy(true);
            }

            this.policy_trial.modify(alpha * 0.1, alpha, 0.01);
            // Reset
            this.memory = Vector.Build.Dense(num_episodes_per_test);
        }
    }
    public Vector<double> act(Vector<double> obs, bool training)
    {
        // bool is initiall set to False
        // training is set to False
        // reward is initially set to None
        // obs is a state vector

        Vector<double> y = Vector<double>.Build.Dense(size_action_space);

        if (training)
        {
            y = policy_trial.predict(obs);
        }
        else
        {
            y = policy.predict(obs);
        }

        return y;
    }

    // Start is called before the first frame update
    void Start()
        {

        spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        // Step counter
        step_counter = 0;

        // Test counter
        test_counter = 0;

        // Test counter (successful ones; with an accept)
        test_success = 0;

        // Return for the current episode
        R = 0;

        // Mean return per episode (best so far)
        avg_R = -100000;

        alpha_decay = 1; // 0.99999

        // The final policy
        this.policy = new SLP(size_obs_space, size_action_space);
        // we will try and improve this policy and then finally make it our actual policy
        this.policy_trial = this.policy.copy(true);

        // zero double vector to hold memory of return gained over each episode
        this.memory = Vector.Build.Dense(num_episodes_per_test);

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), player.GetComponent<BoxCollider2D>());
        }
    }

        // Update is called once per frame
        void Update()
        {
            if (episode_counter <= num_episodes_per_test)
            {
                output = act(observation(), true);
                
                update_policy(gameObject.transform.position.x);
                
            }

        }

        Vector<double> observation()
         {
            int foo = 0;
            foreach (GameObject block in GameObject.FindGameObjectsWithTag("block"))
            {
                float tmp = Math.Abs(block.transform.position.x - gameObject.transform.position.x);
                //print(tmp);
                if (tmp < 2 * blockRadius && block.transform.position.x >= gameObject.transform.position.x)
                {
                    foo = 1;
                }
            }

            int ba = 0;
            foreach (GameObject gap in GameObject.FindGameObjectsWithTag("gap"))
            {
                float tmp = Math.Abs(gap.transform.position.x - gameObject.transform.position.x);
                //print(tmp);
                if (tmp < blockRadius && gap.transform.position.x >= gameObject.transform.position.x)
                {
                    ba = 1;
                }
            }
            Vector<double> input = Vector<double>.Build.Dense(size_obs_space);
    
            input[0] = ba;
            input[1] = foo;
            //input[2] = fooba;
            input[2] = gameObject.transform.position.y;
            return input;
        }

}
