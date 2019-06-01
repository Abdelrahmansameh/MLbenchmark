using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;
using System.Globalization;
// Actor Policy Model
public class QNetwork
{          
	// h1, h2 are the number of neurons for 1st and 2nd hidden layer
	public int input_size, output_size, h1, h2;
	public double alpha = 0.01;
	public Matrix<double> W1;  
	public Matrix<double> error1, delta1, a1, z1; 
	public Matrix<double> W2;
	public Matrix<double> error2, delta2, a2, z2; 
	public Matrix<double> W3;
	public Matrix<double> error3, delta3, a3, z3; 
	public Matrix<double> b1, b2;
	double mse;
    public QNetwork(int input_size, int output_size, int h1, int h2)
		{
			this.input_size = input_size;
			this.output_size = output_size;
			this.h1 = h1;
			this.h2 = h2;

			// init the weights
			this.W1 = Matrix<double>.Build.Random(this.h1, this.input_size);
			this.W2 = Matrix<double>.Build.Random(this.h2, this.h1);
			this.W3 = Matrix<double>.Build.Random(this.output_size, this.h2);
			// init the biases with 0s
			this.b1 = Matrix<double>.Build.Dense(this.h1, 1);
			this.b2 = Matrix<double>.Build.Dense(this.h2, 1);
			// init the deltas for each layer
			this.delta1 = Matrix<double>.Build.Dense(this.h1, 1);
			this.delta2 = Matrix<double>.Build.Dense(this.h2, 1);
			this.delta3 = Matrix<double>.Build.Dense(this.output_size, 1);
			// init the errors for each 
			this.error1 = Matrix<double>.Build.Dense(this.h1, 1);
			this.error2 = Matrix<double>.Build.Dense(this.h2, 1);
			this.error3 = Matrix<double>.Build.Dense(this.output_size, 1);
			// init the input at neurons 
			this.z1 = Matrix<double>.Build.Dense(this.h1, 1);
			this.z2 = Matrix<double>.Build.Dense(this.h2, 1);
			this.z3 = Matrix<double>.Build.Dense(this.output_size, 1);
			// init the activation at neurons
			this.a1 = Matrix<double>.Build.Dense(this.h1, 1);
			this.a2 = Matrix<double>.Build.Dense(this.h2, 1);
			this.a3 = Matrix<double>.Build.Dense(this.output_size, 1);
		
		}
	public Matrix<double> Sigmoid(Matrix<double> W)  
	{
		return 1 / (1 + (1 / W.PointwiseExp()));
	}
	public Matrix<double> tensor_prod(Matrix<double> x, Matrix<double> y)  
	{
		Matrix<double> res = Matrix<double>.Build.Dense(x.RowCount, y.ColumnCount); 
		for(int i = 0;i<x.RowCount; i++)
			{
				for(int j = 0; j<y.ColumnCount; j++)
					{
						res[i,j] = x[i,j] * y[i,j];
					}
			}
		return res;
	}

	public Matrix<double> Sigmoid_Der(Matrix<double> W)
	{
		Matrix<double> t1 = Sigmoid(W);
		Matrix<double> t2 = 1-Sigmoid(W);
		return tensor_prod(t1, t2);
	}

	public static Matrix<double> ReLU(Matrix<double> a)
	{
		return a.Map(x => Math.Max(x, 0));
	}
	 public Matrix<double> feed(Matrix<double> input)
	 	{
			z1 = W1*input + b1;
			a1 = Sigmoid(z1);
			z2 = W2*a1 + b2;
			a2 = Sigmoid(z2);
			z3 = W3*a2;
			a3 = Sigmoid(z3); 
			return a3; 
		}
	public double error(Matrix<double> x, Matrix<double>y)
		{
			double res = 0.0; 

			for(int i = 0;i<x.RowCount;i++)
				{
					res = Math.Pow(x[i,0]-y[i,0], 2);
				}

			return res/(x.RowCount);
		}

	public void back_prop(Matrix<double> input, Matrix<double> expected)
		{
			Matrix<double> predicted = feed(input);
			Matrix<double> e = expected - predicted;
			mse = error(expected, predicted);

			// First update the deltas for the last layer L = 3
			delta3 = tensor_prod(e, Sigmoid_Der(z3));
			delta2 = tensor_prod(W3.Transpose()*delta3, Sigmoid_Der(z2));
			delta1 = tensor_prod(W2.Transpose()*delta2, Sigmoid_Der(z1));
			
			for(int j=0;j<W1.RowCount;j++)
				{
					for(int k=0;k<W1.ColumnCount; k++)
						{
							W1[j,k] = W1[j,k] - alpha * input[k,0]*delta1[j,0]; 
						}
				}

			for(int j=0;j<W2.RowCount;j++)
				{
					for(int k=0;k<W2.ColumnCount; k++)
						{
							W2[j,k] = W2[j,k] - alpha * a1[k,0]*delta2[j,0]; 
						}
				}

			for(int j=0;j<W3.RowCount;j++)
				{
					for(int k=0;k<W3.ColumnCount; k++)
						{
							W3[j,k] = W3[j,k] - alpha * a2[k,0]*delta3[j,0]; 
						}
				}
				
		}

	public void train(Matrix<double> input, Matrix<double> output, int max_epochs)
		{
			for(int i = 0;i<max_epochs;i++)
				{
					for(int j = 0; j< output.RowCount; j++)
					{
						// train the neural net for each input matrix-vector
						var input_mat = Matrix<double>.Build.DenseOfRowMajor(input.Row(j).Count, 1, input.Row(j));
						var expected = Matrix<double>.Build.DenseOfRowMajor(output.Row(j).Count, 1, input.Row(j));
						back_prop(input_mat, expected);
						
					}
					if((i+1)%10 == 0)
						Console.WriteLine(String.Format("Epoch = {0} and error = {1}", i+1, mse));
				}
		}
	static void Main(String[] args)
		{

			var formatProvider = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            formatProvider.TextInfo.ListSeparator = " ";

			Console.WriteLine("Training on XOR");
			QNetwork obj = new QNetwork(2, 1, 3, 3);
			
			var input = Matrix<double>.Build.DenseOfArray(new double[,] {{ 0.0,0.0}, {0.0,1.0}, {1.0,0.0}, {1.0,1.0}});
			var output = Matrix<double>.Build.DenseOfArray(new double[,] {{0.0} ,{1.0}, {1.0},{1.0}});

			obj.train(input,output, 1000);
		}
}



