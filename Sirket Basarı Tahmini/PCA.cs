using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Double;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sirket_Basarı_Tahmini
{
    class PCA
    {
        int featureCount;
        List<List<double>> Features;
        public List<List<double>> ReducedFeatures { get; set; }
        public PCA(int featureCount, List<List<double>> features)
        {
            this.featureCount = featureCount;
            Features = features ?? throw new ArgumentNullException(nameof(features));
            ReducedFeatures = new List<List<double>>(); 
        }

        public List<List<double>> getNewFeatures()
        {
            List<List<double>> covMatrix = new List<List<double>>();
            covMatrix = Covmatrix(Features);
            //Console.WriteLine(covMatrix[0][2]);
            return NewFeatures(featureCount, eigen(covMatrix), Features);
        }

        public double Cov(List<double> X, List<double> Y)
        {
            if (X.Count != Y.Count)
                return -1;

            double meanX = X.Average();
            double meanY = Y.Average();
            double cov = 0;
            for (int i = 0; i < X.Count; i++)
            {
                cov += (X[i] - meanX) * (Y[i] - meanY);
            }

            //            Console.WriteLine(cov / (X.Count - 1));

            return cov / (X.Count - 1);
        }

        public List<List<double>> Covmatrix(List<List<double>> feature)
        {
            List<List<double>> covarianceMatrix = new List<List<double>>();
            List<double> tmp = new List<double>();

            foreach (var x in feature)
            {
                tmp = new List<double>();
                tmp.Clear();
                foreach (var y in feature)
                {
                    tmp.Add(Cov(x, y));
                }
                covarianceMatrix.Add(tmp);
            }

            //Console.WriteLine(covarianceMatrix[0][0] + " " + covarianceMatrix[0][1] + "\n" + covarianceMatrix[1][0] + " " + covarianceMatrix[1][1] + " ");
            return covarianceMatrix;
        }

        public Evd<double> eigen(List<List<double>> covarianceMatrix)
        {
            Matrix<double> E = DenseMatrix.OfColumns(covarianceMatrix);
            Evd<double> eigen = E.Evd();
            Matrix<double> eigenvectors = eigen.EigenVectors;
            Vector<Complex> eigenvalues = eigen.EigenValues;

            //Console.WriteLine(eigenvectors + "" + eigenvalues + " ");
            return eigen;
        }

        public List<List<double>> NewFeatures(int n, Evd<double> eigen, List<List<double>> feature)
        {
            List<List<double>> secilenoz = new List<List<double>>();
            List<double> eigvalue = new List<double>();
            List<double> eigvalue2 = new List<double>();
            List<int> eigvalueindex = new List<int>();
            Matrix<double> E = DenseMatrix.OfColumns(feature);
            Matrix<double> eigenvectors = eigen.EigenVectors;
            //Console.WriteLine(eigenvectors);
            

            Vector<Complex> eigenvalues = eigen.EigenValues;
            //Console.WriteLine(eigenvalues);
            double b = eigenvalues[0].Real;
            foreach (var i in eigenvalues)
            {
                eigvalue.Add(i.Real);
                eigvalue2.Add(i.Real);
            }
            eigvalue.Sort();
            eigvalue.Reverse();

            for (int i = 0; i < n; i++)
            {
                eigvalueindex.Add(eigvalue2.IndexOf(eigvalue[i]));
            }

            int k = 0;
            List<double> tmp = new List<double>();
            for (int i = 0; i < n; i++)
            {
                tmp = new List<double>();
                for (int j = 0; j < eigenvectors.RowCount; j++)
                {
                    //                    Console.WriteLine(eigenvectors.At(j, eigvalueindex[k]));
                    tmp.Add(-eigenvectors.At(j, eigvalueindex[k]));
                }
                secilenoz.Add(tmp);
                k++;
            }

            var c = E.Multiply(eigenvectors);
            //            Console.WriteLine("           "+secilenoz[1][0]);
            //            Console.WriteLine(eigvalueindex[1]);
            return MultiplyMatrix(feature, secilenoz);
        }

        public List<List<double>> MultiplyMatrix(List<List<double>> A, List<List<double>> B)
        {
            Matrix<double> a = DenseMatrix.OfColumns(A);
            Matrix<double> b = DenseMatrix.OfColumns(B);
            var c = a.Multiply(b);
            var d = c.ToColumnArrays();
            var e = c.ToArray();
            //Console.WriteLine(d[300][1]);
            return CreateNewFeatures(d);
        }

        public List<List<double>> CreateNewFeatures(double[][] d)
        {
            List<double> tmp = new List<double>();
            List<List<double>> Ft = new List<List<double>>();
            for (int i = 0; i < d.Count(); i++)
            {
                tmp = new List<double>();
                for (int j = 0; j < d[0].Count(); j++)
                {
                    tmp.Add(d[i][j]);
                }
                ReducedFeatures.Add(tmp);
            }
            return ReducedFeatures;
        }

        



    }
}
