using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Double;
using Discretization;
using MongoDB.Driver;
using MongoDB.Bson;

namespace PCA
{
    class Program
    {
        public static MongoDBService A = new MongoDBService("mongodb://localhost:27017", "mylib", "Bilanco");

        static void Main(string[] args)
        {
            A.InitiateService("mongodb://localhost:27017", "mylib", "Bilanco");
            var filter = new BsonDocument { };
            var Data = A.GetService().GetCollection().Find(filter).ToCursor();
            var DataList = A.GetService().GetCollection().Find(filter).ToList();
            PCA(5, BsonToFeature(DataList));
        }

        public static void PCA(int n, List<List<double>> feature)
        {
            List<List<double>> covMatrix = new List<List<double>>();
            covMatrix = Covmatrix(feature);
            eigen(covMatrix);
            NewFeatures(n, eigen(covMatrix), feature);
        }

        public static double Cov(List<double> X, List<double> Y)
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

        public static List<List<double>> Covmatrix(List<List<double>> feature)
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

            Console.WriteLine(covarianceMatrix[0][0] + " " + covarianceMatrix[0][1] + "\n" + covarianceMatrix[1][0] + " " + covarianceMatrix[1][1] + " ");
            return covarianceMatrix;
        }

        public static Evd<double> eigen(List<List<double>> covarianceMatrix)
        {
            Matrix<double> E = DenseMatrix.OfColumns(covarianceMatrix);
            Evd<double> eigen = E.Evd();
            Matrix<double> eigenvectors = eigen.EigenVectors;
            Vector<Complex> eigenvalues = eigen.EigenValues;

            //            Console.WriteLine(eigenvectors + "" + eigenvalues + " ");
            return eigen;
        }

        public static void NewFeatures(int n, Evd<double> eigen, List<List<double>> feature)
        {
            List<List<double>> secilenoz = new List<List<double>>();
            List<double> eigvalue = new List<double>();
            List<double> eigvalue2 = new List<double>();
            List<int> eigvalueindex = new List<int>();
            Matrix<double> E = DenseMatrix.OfColumns(feature);
            Matrix<double> eigenvectors = eigen.EigenVectors;

            Vector<Complex> eigenvalues = eigen.EigenValues;
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
            MultiplyMatrix(feature, secilenoz);
        }

        public static void MultiplyMatrix(List<List<double>> A, List<List<double>> B)
        {
            Matrix<double> a = DenseMatrix.OfColumns(A);
            Matrix<double> b = DenseMatrix.OfColumns(B);
            var c = a.Multiply(b);
            var d = c.ToColumnArrays();
            var e = c.ToArray();
            Console.WriteLine(d[0].Count());
            Console.WriteLine(d[0][330]);
        }

        public static void CreateNewFeatures(double[][] d)
        {
            List<double> tmp = new List<double>();
            for (int i = 0; i < d.Count(); i++)
            {
                tmp = new List<double>();
                for (int j = 0; j < d[0].Count(); j++)
                {
                    tmp.Add(d[i][j]);
                }
                
            }

        }

        public static List<List<double>> BsonToFeature(List<BsonDocument> data)
        {
            List<List<double>> feature = new List<List<double>>();
            List<double> tmp = new List<double>();
            //            data.ElementAt(0).Count() - 4
            for (int i = 3; i < data.ElementAt(0).Count() - 3; i++)
            {
                tmp = new List<double>();
                //                tmp.Clear();
                // data.Count();
                for (int j = 0; j < data.Count(); j++)
                {
                    tmp.Add(data.ElementAt(j).GetValue(i).ToDouble());
                    //                    Console.Write(data.ElementAt(j).GetValue(i).ToDouble() + "   " );
                }
                //                Console.Write("Sum = " + tmp.Sum());
                //                ListToNormalList(tmp);
                feature.Add(ListToNormalList(tmp));
            }

            return feature;

        }

        public static List<double> ListToNormalList(List<double> data)
        {
            List<double> normalData = new List<double>();

            double std = 0;
            double mean = data.Sum() / data.Count();
            //            Console.Write("mean = "+mean);
            double sum = 0;
            foreach (var i in data)
            {
                std += (i - mean) * (i - mean);
            }
            std /= (data.Count());
            std = Math.Sqrt(std);
            //            Console.WriteLine(" sdt = " + std);
            double e = 0;
            foreach (var i in data)
            {
                if (std != 0)
                {
                    normalData.Add((i - mean) / std);
                    e = (i - mean) / std;
                }

                else
                    normalData.Add(0);



                //                Console.WriteLine(e);
            }
            return normalData;
        }

    }
}
