﻿using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using UnityEngine;

// A Single-Layer Perceptron with copy and random modification
public class Policy
{
    // Algorithm Parameters 
    public double alpha_w = 0.01;
    public double alpha_b = 0.01;
    public double prob_reset = 0.1;
    public double scaling = Math.Pow(10, -4);
    public int input_size;
    public int output_size;

    public Matrix<double> W;

    public Policy(int inputs, int outputs)
    {
        input_size = inputs;
        output_size = outputs;
        reset();
    }

    public void reset()
    {
        // Resets the weights and biases to normal distribution with scaling
        this.W = scaling * Matrix<double>.Build.Random(this.input_size, this.output_size);

    }

    public Vector<double> predict(Vector<double> x)
    {
        // Feedforward prediction of Policy.
        var a = x * this.W;
        return Sigmoid(a);
    }

    public int act(Vector<double> x)
    {
        var probability = predict(x);
        var action = probability.AbsoluteMaximumIndex();
        return action;
    }

    public Policy copy(bool modify)
    {
        // Creates a copy of this Policy, with possible modification.
        Policy b = new Policy(this.input_size, this.output_size);
        b.W = this.W.Clone();

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
            this.reset();
        }

        // Make a random adjustment to the weight matrix.
        this.W = this.W + alpha_w * Matrix<double>.Build.Random(this.input_size, this.output_size);
    }

    Vector<double> Sigmoid(Vector<double> W)
    {
        return 1 / (1 + (1 / W.PointwiseExp()));
    }
}


public class hillAgent : MonoBehaviour
{
    //Simple Hill Climbing Agent
    public int n_episodes = 1000; // maximum number of training episodes
    public int max_t = 1000; // maximum number of timesteps per episode 
    public double gamma = 1.0;  // discount rate 
    public double noise_scale = 0.001; // standard deviation for additive noise
    public int size_action_space;
    public int size_obs_space;
    public Vector<double> output;
    int episode_counter;
    int step_counter;

    public float blockRadius = 0.5f;
    // for keeping a memory of the scores
    List<double> scores_deque;
    List<double> score;

    double best_reward = double.NegativeInfinity;
    Policy policy;
    Matrix<double> best_policy_w;

    LineRenderer line;
    List<double> rewards;


    void Start()
    {
        policy = new Policy(size_obs_space, size_action_space);
        best_policy_w = policy.W;
        rewards = new List<double>();
        scores_deque = new List<double>();
        scores_deque.Capacity = 100;
        score = new List<double>();
        episode_counter = 0;

        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = 4;
        line.SetPosition(0, gameObject.transform.position);

        episode_counter++;
    }


    void Update()
    {

        if (episode_counter <= n_episodes)
        {

            if (!gameObject.GetComponent<PlayerControl>().Dead || step_counter <= max_t)
            {
                int action = policy.act(state());
                switch (action)
                {
                    case 0:
                        output = Vector<double>.Build.DenseOfArray(new double[] { 0, 0 });
                        break;
                    case 1:
                        output = Vector<double>.Build.DenseOfArray(new double[] { 1, 0 });
                        break;
                    case 2:
                        output = Vector<double>.Build.DenseOfArray(new double[] { 1, 1 });
                        break;

                    case 3:
                        output = Vector<double>.Build.DenseOfArray(new double[] { 0, 1 });
                        break;
                }
                step_counter++;
                rewards.Add(transform.position.x);
            }

            else
            {
                double R = 0.0;
                int count = 1;
                foreach (var r in rewards)
                {
                    R += r * Math.Pow(gamma, count);
                    count++;
                }

                if (R >= best_reward)
                {
                    best_reward = R;
                    best_policy_w = policy.W;
                    noise_scale = Math.Max(Math.Pow(10, -3), noise_scale / 2);
                    policy.W += noise_scale * Matrix<double>.Build.Random(policy.input_size, policy.output_size);
                }
                else
                {
                    noise_scale = Math.Min(2, noise_scale * 2);
                    policy.W = best_policy_w + noise_scale * Matrix<double>.Build.Random(policy.input_size, policy.output_size);
                }
                rewards = new List<double>();
                step_counter = 0;
                episode_counter++;
            }

        }
    }

    public Vector<double> state()
    {
        int foo = 0;
        foreach (GameObject block in GameObject.FindGameObjectsWithTag("block"))
        {
            GameObject UL = block.transform.Find("UUL").gameObject;

            float tmp = Math.Abs(UL.transform.position.x - gameObject.transform.position.x);
            //print(tmp);
            if (tmp < blockRadius && UL.transform.position.x >= gameObject.transform.position.x)
            {
                foo = 1;
                line.SetPosition(1, UL.transform.position);
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
                line.SetPosition(2, gap.transform.position);

            }
        }

        Vector<double> input = Vector<double>.Build.Dense(size_obs_space);
        input[0] = ba;
        input[1] = foo;
        input[2] = gameObject.transform.position.y + 1.58532;

        return input;
    }

}
