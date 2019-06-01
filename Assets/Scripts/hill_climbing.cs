using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
// A Single-Layer Perceptron with copy and random modification
/*public class SLP
{   
      // Algorithm Parameters 
      public double alpha_w = 0.01;
      public double alpha_b = 0.01;
      public double prob_reset = 0.1;
      public double scaling = 0.0;
      public int input_size;
      public int output_size;

      public Matrix<double> W;
      public Vector<double> b;

      public SLP (int inputs, int outputs)
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
        this.b = scaling * Vector<double>.Build.Random(this.output_size);
      }
          
      public Vector<double> predict(Vector<double> x)
      {
          // Feedforward prediction of SLP.
          var a = x * this.W + this.b; 
          return Sigmoid(a);
      }

      public SLP copy(bool modify)
      {
          // Creates a copy of this SLP, with possible modification.
          SLP b = new SLP(this.input_size, this.output_size);
          b.W = this.W.Clone();
          b.b = this.b.Clone();

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

public class SimpleHillClimber
  {
    //Simple Hill Climbing Agent
        int max_episode_length = 50;
        int num_episodes_per_test = 100; 
        double R, avg_R;
        int size_obs_space, size_action_space, step_counter, test_counter,test_success;
        Vector<double> memory;
        double alpha_init, alpha, alpha_decay;
        SLP policy;
        SLP policy_trial; 

    public SimpleHillClimber(int obs_space, int act_space, int max_episode_length, int num_episodes_per_test, double alpha)
      {  
        // Size of observation and action spaces
        size_obs_space = obs_space;
        size_action_space= act_space; 

        // Max length of an episode 
        this.max_episode_length = max_episode_length; 

        // Number of episodes per test
        this.num_episodes_per_test = num_episodes_per_test;

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
        
        // Alpha (step size / learning rate)
        this.alpha_init = alpha;
        this.alpha = this.alpha_init;
        this.alpha_decay = 1; // 0.99999

        // The final policy
        this.policy = new SLP(size_obs_space, size_action_space);
        // we will try and improve this policy and then finally make it our actual policy
        this.policy_trial = this.policy.copy(true);

        // zero double vector to hold memory of return gained over each episode
        this.memory = Vector.Build.Dense(num_episodes_per_test);
      }

    public void update_policy(double reward, bool done)
      {
        // Update the return for the episode
        R = R + reward; 

        // Counting (each step of the episode of max length T)
        step_counter += 1;  

        // If end of epsiode ...
        if (step_counter >= max_episode_length || done)
          {
            // End of the episode 
            memory.Add(R);
            // Reset
            step_counter= 0;
            R = 0;
            alpha = alpha * alpha_decay;
          }

        // If end of the set of tests 
        if (memory.Count >= num_episodes_per_test)
          {
            // Calculate the average return per episode
            double average_R = memory.Sum()/num_episodes_per_test;
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
            
            this.policy_trial.modify(alpha*0.1, alpha, 0.01); 
            // Reset
            this.memory = Vector.Build.Dense(num_episodes_per_test);
          }
      }    
    public double act(Vector<double> obs,double reward,bool done,bool training)
      {
        // bool is initiall set to False
        // training is set to False
        // reward is initially set to None
        // obs is a state vector

        Vector<double> y =  Vector<double>.Build.Dense(size_action_space); 

        if (training)
          {
            y = policy_trial.predict(obs); 
          }            
        else
          {
            y = policy.predict(obs); 
          }  

        return y.AbsoluteMaximum();
      }
  }

/*
class Driver
{

    public double run_agent(SimpleHillClimber agent, Environment env, int num_episodes, int max_episode_length, bool train)
      {
        double R = 0.0;

        // For each episode 
        for(int i = 0; i<num_episodes; i++)
          {
            

            // initial state of the environment (obs or observation at t = 0)
            s_t = env.reset();

            // For each time step
            for(int t = 0; t<max_episode_length; t++)
              {
                // Act
                a_t = agent.act(s_t, train);

                // Step
                s_t, r_t, done = env.step(a_t); 

                //Update
                agent.update_policy(s_t, r_t, done);
  
                R += r_t;
                if(done) break;
              }
          }
          env.close();
          return (1/num_episodes)*R;

      }

}
*/