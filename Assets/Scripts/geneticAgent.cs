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

    public float blockRadius;
    
    public void Initialize_Random(int x, int h, int h2, int y)

    {
        n_x = x;
        n_h = h;
        n_h2 = h2;
        n_y = y;

        W1shape = new Tuple<int, int>(n_h, n_x);
        W2shape = new Tuple<int, int>(n_h2, n_h);
        W3shape = new Tuple<int, int>(n_y, n_h2);
         
        W1 = Matrix<double>.Build.Random(W1shape.Item1, W1shape.Item2);
        W2 = Matrix<double>.Build.Random(W2shape.Item1, W2shape.Item2);
        W3 = Matrix<double>.Build.Random(W3shape.Item1, W3shape.Item2);

        int num_weights = n_x * n_h + n_h * n_h2 + n_h2 * n_y;

        individual = new double[num_weights];
        int c = 0;
        for (int i = 0; i < W1shape.Item1; i++)
        {
            for (int j = 0; j < W1shape.Item2; j++)
            {
                individual[c] = W1[i, j];
                c++;
            }
        }
        for (int i = 0; i < W2shape.Item1; i++)
        {
            for (int j = 0; j < W2shape.Item2; j++)
            {
                individual[c] = W2[i, j];
                c++;
            }
        }
        for (int i = 0; i < W3shape.Item1; i++)
        {
            for (int j = 0; j < W3shape.Item2; j++)
            {
                individual[c] = W3[i, j];
                c++;
            }
        }
    }

    public void Initialize_unfold( double[] individual, int x, int h, int h2, int y)
    {
        int _=0;
        foreach (var thing in individual) { _++; };
        //print(_);

        n_x = x;
        //print(n_x);
        n_h = h;
        n_h2 = h2;
        n_y = y;

        int num_weights = n_x * n_h + n_h * n_h2 + n_h2 * n_y;

        W1shape = new Tuple<int, int>(n_h, n_x);
        W2shape = new Tuple<int, int>(n_h2, n_h);
        W3shape = new Tuple<int, int>(n_y, n_h2);

        this.individual = individual;
        
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
                W1[i, j] = individual[offset];
                offset++;
            }
        }
        //print(W1);
       // print(offset);
        for (int i = 0; i < W2shape.Item1; i++)
        {
            for (int j = 0; j < W2shape.Item2; j++)
            {
                W2[i, j] = individual[offset];
                offset++;
            }
        }
        //print(W2);
        // print(offset);

        for (int i = 0; i < W3shape.Item1; i++)
        {
            for (int j = 0; j < W3shape.Item2; j++)
            {
                W3[i, j] = individual[offset];
                offset++;
            }
        }
       // print(W3);
    }
        // Update is called once per frame
        void Update()
    {
        if (!gameObject.GetComponent<PlayerControl>().Dead)
        {

        GameObject[] platforms = GameObject.FindGameObjectsWithTag("platform");
        Array.Sort<GameObject>(platforms,
           delegate (GameObject m, GameObject n)
           {
               GameObject ULn;
               GameObject ULm;
               ULm = m.transform.Find("UL").gameObject;
               ULn = n.transform.Find("UL").gameObject;
               if (Math.Abs(ULm.transform.position.x - gameObject.transform.position.x) < Math.Abs(ULn.transform.position.x - gameObject.transform.position.x))
               {
                   return -1;
               }
               else if (Math.Abs(ULm.transform.position.x - gameObject.transform.position.x) == Math.Abs(ULn.transform.position.x - gameObject.transform.position.x))
               {
                   return 0;
               }
               return 1;
           }
           );
        int foo = 0;
        foreach(GameObject block in GameObject.FindGameObjectsWithTag("block"))
        {
            float tmp = Math.Abs(block.transform.position.x  - gameObject.transform.position.x);
            print(tmp);
            if (tmp < blockRadius && block.transform.position.x >= gameObject.transform.position.x){
                foo = 1;
            }
        }
        Vector<double> input = Vector<double>.Build.Dense(n_x);
        input[0] = Math.Abs(platforms[0].transform.position.x - gameObject.transform.position.x);
        input[1] = foo;

        print(input);
        //input[1] = gameObject.transform.position.y;
        //input[2] = Math.Abs(platforms[0].transform.position.x - gameObject.transform.position.x);
            output = propagation(input);
            //print(output);
        }

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
