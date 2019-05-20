using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;
using System;

public class geneticAgent : MonoBehaviour
{
    public controls move;

    public int n_x;
    public int n_h;
    public int n_h2;
    public int n_y;

    public double[] individual;

    private Tuple<int, int> W1shape;
    private Tuple<int, int> W2shape;
    private Tuple<int, int> W3shape;

    private Matrix<double> W1;
    private Matrix<double> W2;
    private Matrix<double> W3;

    public Vector<double> output; 

    public void Initialize_Random(int x, int h, int h2, int y)
    {
        n_x = x;
        print(n_x);

        n_h = h;
        n_h2 = h2;
        n_y = y;

        W1shape = new Tuple<int, int>(n_h, n_x);
        W2shape = new Tuple<int, int>(n_h2, n_h);
        W3shape = new Tuple<int, int>(n_y, n_h2);

        W1 = Matrix<double>.Build.Random(W1shape.Item1, W1shape.Item2);
        W2 = Matrix<double>.Build.Random(W2shape.Item1, W2shape.Item2);
        W3 = Matrix<double>.Build.Random(W3shape.Item1, W3shape.Item2);
    }

    public void Initialize_unfold( double[] individual, int x, int h, int h2, int y)
    {
        int _=0;
        foreach (var thing in individual) { _++; };
        //print(_);

        n_x = x;
        print(n_x);
        n_h = h;
        n_h2 = h2;
        n_y = y;
        W1shape = new Tuple<int, int>(n_h, n_x);
        W2shape = new Tuple<int, int>(n_h2, n_h);
        W3shape = new Tuple<int, int>(n_y, n_h2);

        //print(W1shape);
        //print(W2shape);
        //print(W3shape);

        W1 = Matrix<double>.Build.Dense(W1shape.Item1, W1shape.Item2);
        W2 = Matrix<double>.Build.Dense(W2shape.Item1, W2shape.Item2);
        W3 = Matrix<double>.Build.Dense(W3shape.Item1, W3shape.Item2);

        int offset = 0;
        for (int i = 0; i < W1shape.Item1; i++)
        {
            for (int j = 0; j < W1shape.Item2; j++)
            {
                W1[i, j] = individual[i * W1shape.Item1 + j]; 
            }
        }
        offset = 1 + offset + (W1shape.Item1 - 1) * (W1shape.Item2 - 1);

        for (int i = 0; i < W2shape.Item1; i++)
        {
            for (int j = 0; j < W2shape.Item2; j++)
            {

                W2[i, j] = individual[offset + i * W2shape.Item1 + j];
            }
        }

        offset = 1 + offset + (W2shape.Item1 - 1) * (W2shape.Item2 - 1);

        for (int i = 0; i < W3shape.Item1; i++)
        {
            for (int j = 0; j < W3shape.Item2; j++)
            {
                W2[i, j] = individual[offset + i * W3shape.Item1 + j];
            }
        }
    }
        // Update is called once per frame
        void Update()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("platform");
        Array.Sort<GameObject>(platforms,
           delegate (GameObject m, GameObject n)
           {
               GameObject ULn;
               GameObject ULm;
               ULm = m.transform.Find("UL").gameObject;
               ULn = n.transform.Find("UL").gameObject;

               if (Math.Abs(ULm.transform.position.x - gameObject.transform.position.x) <= Math.Abs(ULn.transform.position.x - gameObject.transform.position.x))
               {
                   return -1;
               }
               else if (m.transform.position.x == n.transform.position.x)
               {
                   return 0;
               }
               return 1;
           }
           );
        Vector<double> input = Vector<double>.Build.Dense(n_x);
        input[0] = gameObject.transform.position.x;
        input[1] = gameObject.transform.position.y;
        input[2] = Math.Abs(platforms[0].transform.position.x - gameObject.transform.position.x);
        output = propagation(input);
        //print(output);
    }

    void updateMove()
    {

    }

    Vector<double> Sigmoid(Vector<double> W)  
    {
        return 1 / (1 + (1 / W.PointwiseExp()));
    }


    Vector<double> propagation(Vector<double> X)
    {
        Vector<double> Z1 = W1 * X;
        Vector<double> A1 = Z1.PointwiseTanh();
        Vector<double> Z2 = W2 * A1;
        Vector<double> A2 = Z2.PointwiseTanh();
        Vector<double> Z3 = W3 * A2;
        Vector<double> A3 = Sigmoid(Z3);
        return A3;
    }
}
