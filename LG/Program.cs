using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LG
{
    class Program
    {
        static void Main(string[] args)
        {
            double[][] inputs = new double[4][] { new double[] { 1, 2 }, new double[] { 1, 3 }, new double[] { 1, 4 }, new double[] { 1, 5 } };
            //double[][] inputs = LogisticRegression.ReadInputFromFile("exe1data1.txt");
            double[] outputs = new double[] { 1, 1, 1, 0 };
            //double[] outputs = LogisticRegression.ReadDataFromFile("
            double alpha = 0.01;
            double[] theta = new double[] { 1, 1 };
            LogisticRegression lr = new LogisticRegression();
            double[] newtheta = lr.Learn(inputs, outputs, theta, alpha);
            Console.WriteLine(newtheta[0] + " " + newtheta[1] + " " + newtheta[2] + " " + newtheta[3]);
            lr.Normalise(3.95);
        }
    }
}
