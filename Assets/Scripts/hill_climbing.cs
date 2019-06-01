using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
// A Single-Layer Perceptron with copy and random modification
public class Policy
{   
      // Algorithm Parameters 
      public double alpha_w = 0.01;
      public double alpha_b = 0.01;
      public double prob_reset = 0.1;
      public double scaling = Math.pow(10, -4);
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

	  public double act(Vector<double> x)
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

public class SimpleHillClimber
  {
    //Simple Hill Climbing Agent
        public int n_episodes = 1000; // maximum number of training episodes
        public int max_t = 1000; // maximum number of timesteps per episode 
        public double gamma = 1.0;  // discount rate 
        public double noise_scale = Math.pow(10, -2); // standard deviation for additive noise
        
        // for keeping a memory of the scores
        List<double> scores_deque; 
        List<double> score; 
        
        double best_reward = double.NegativeInfinity;
        Policy policy;
        Matrix<double> best_policy_w;

        public SimpleHillClimber(int size_action_space, int size_obs_space)
            {
                policy = new Policy(size_obs_space, size_action_space);
                best_policy_w = policy.W;

				scores_deque = new List<double>();
				scores_deque.Capacity = 100; 
				score = new List<double>();
            }

		public void run()
			{

				for(int i = 0; i<n_episodes; i++)
					{
						List<double> rewards = new List<double>();

						// TODO: reset the player here, let state be the initial state
						var state; 

						for(int t = 0; t < max_t; t++)
							{
								var action = policy.act(state);

								// TODO: Take this action and get the update the current state
								// state, reward, grounded, _ = env.step(action)

								rewards.Add(reward);
								if (dead)
									break; 
							}
						
						scores_deque.Add(rewards.Sum());
						scores.Add(rewards.Sum());

						// now we need to compute the expected return
						double R = 0.0;
						int count = 1; 
						foreach (var r in rewards)
						{		
							R += r*Math.pow(gamma, count);
							count++; 
						}

						if(R >= best_reward)
							{
								best_reward = R;
								best_policy_w = policy.W;
								noise_scale = max(Math.pow(10, -3), noise_scale/2);
								policy.w += noise_scale * Matrix<double>.Build.Random(policy.input_size, policy.output_size);
							}
						else
							{
								noise_scale = min(2, noise_scale*2);
								policy.W = best_policy_w + noise_scale * Matrix<double>.Build.Random(policy.input_size, policy.output_size);
							}
						


					}
				



			}
  }
