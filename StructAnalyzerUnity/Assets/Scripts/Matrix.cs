using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Matrix {

    double[][] matrix;
    double[] solutions;
    int rows;
    int columns;

    //-----------------Public methods---------------------------------------
	public Matrix(ref double[][] m)
    {
        matrix = m;
        rows = m.Length;
        columns = m[0].Length;
    }

    public double[] getSolutions()
    {
        return solutions;
    }

    public int solveByGaussJordan()
    {
        double det = 1;

        for (int i=0; i<rows; i++)
        {
            int nextRow = i + 1;
            while(matrix[i][i] == 0)
            {
                if(nextRow >= rows)
                {
                    det = 0;
                    return -1;
                }

                swapRows(i, nextRow);
                nextRow++;
            }

            det *= matrix[i][i];

            double pivotInverse = 1 / matrix[i][i];
            multiplyRowByConstant(i, pivotInverse);

            for(int j=0; j<rows; j++)
            {
                if (i == j)
                    continue;

                linearCombination(i, j, i);
            }

        }

        solutions = new double[rows];
        
        for(int i=0; i<rows; i++)
        {
            solutions[i] = matrix[i][rows];
        }

        return 0;
    }
    
    //------------------Private methods---------------------------------------
    void swapRows(int row1, int row2)
    {
        for(int i=0; i< columns; i++)
        {
            double tmp = matrix[row1][i];
            matrix[row1][i] = matrix[row2][i];
            matrix[row2][i] = tmp;
        }
    }

    void multiplyRowByConstant(int row, double c)
    {
        for(int i=0; i<columns; i++)
        {
            matrix[row][i] *= c;
        }
    }

    //We assume that the position pivot on the original row is 1, this is achieved by previously call multiplyRowByConstant
    void linearCombination(int originalRow, int otherRow, int pivot)
    {
        double c = -1 * matrix[otherRow][pivot];

        for(int i=0; i<columns; i++)
        {
            matrix[otherRow][i] = c * matrix[originalRow][i] + matrix[otherRow][i];
        }
    }

}
