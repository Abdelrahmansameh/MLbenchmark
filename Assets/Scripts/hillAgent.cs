using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hillAgent : MonoBehaviour
{// Algorithm Parameters 

    public double alpha_w = 0.01;
    public double alpha_b = 0.01;
    public double prob_reset = 0.1;
    public double scaling = 0.0;
    public int input_size;
    public int output_size;

    public Matrix<double> W;
    public Vector<double> b;

    public hillAgent(int inputs, int outputs)
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
        var a = this.W.Transpose() * x + this.b;
        return Sigmoid(a);
    }

    public hillAgent copy(bool modify)
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
