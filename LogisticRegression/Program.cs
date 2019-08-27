using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discretization;
using MongoDB.Bson;
using MongoDB.Driver;
using PCA;

namespace LogisticRegression
{
    class Program
    {
        public static MongoDBService database = new MongoDBService("mongodb://localhost:27017", "mylib", "Bilanco");
        static void Main(string[] args)
        {
            database.InitiateService("mongodb://localhost:27017", "mylib", "Bilanco");
            var filter = new BsonDocument { };
            var DataList = database.GetService().GetCollection().Find(filter).ToList();
            List<double> weigths = new List<double>();

            weigths.Add(0);

            List<List<double>> features = new List<List<double>>();
            List<double> f1 = new List<double>();

            for (int i = 0; i < 354; i++)
            {
                f1.Add(DataList.ElementAt(i).GetValue(482).ToDouble());
            }
            //            f1 = ListToNormalList(f1);
            List<double> f2 = new List<double>();
            f2.Add(80);
            f2.Add(55);
            f2.Add(55);
            f2.Add(90);
            f2.Add(80);
            f2.Add(45);
            f2.Add(65);
            f2.Add(80);
            f2.Add(80);
            f2.Add(70);
            f2.Add(55);
            f2.Add(65);
            //            f2 = ListToNormalList(f2);
            features.Add(f1);
            // features.Add(f2);
            List<double> labels = new List<double>();

            for (int i = 0; i < 354; i++)
            {
                if (DataList.ElementAt(i).GetValue(482).ToDouble() > 0.5)
                    labels.Add(1);
                else
                    labels.Add(0);
            }



            List<double> newW = new List<double>();
            List<double> Label2 = new List<double>();
            //            for(int i = 0; i < features.Count(); i++)
            //            {
            //                features[i] = NormalizeData(features[i],0,100);
            //            }

                        for (int i = 0; i < features.Count(); i++)
                        {
                            features[i] = ListToGaussNormalList(features[i]);
                        }

            newW = Train(features, weigths, labels);
            Label2 = Classify(Predict(features, newW));
            yazdir(Label2);
            Compare(labels, Label2);

        }

        public static List<double> ListToGaussNormalList(List<double> data)
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
                    normalData.Add(((i - mean) / std)*100);
                    Console.WriteLine((((i - mean) / std)));
                    e = (i - mean) / std;
                }

                else
                    normalData.Add(0);



                //                Console.WriteLine(e);
            }
            return normalData;
        }

        public static List<double> listToNormalList2(List<double> data)
        {
            List<double> normalData = new List<double>();
            double min = data.Min();
            double max = data.Max();
            for (int i = 0; i < data.Count(); i++)
            {
                normalData.Add(((data[i] - min) / (max - min)));
            }
            //            Console.WriteLine("Girdi");
            return normalData;
        }

        private static List<double> NormalizeData(IEnumerable<double> data, int min, int max)
        {
            double dataMax = data.Max();
            double dataMin = data.Min();
            double range = dataMax - dataMin;

            return data
                .Select(d => (d - dataMin) / range)
                .Select(n => (double)((1 - n) * min + n * max))
                .ToList();
        }

        public static List<double> getLabels(List<BsonDocument> data, double oScoreThreshold)
        {
            List<double> labels = new List<double>();
            for (int i = 0; i < data.Count(); i++)
            {
                if (data.ElementAt(i).GetValue(482) > oScoreThreshold)
                    labels.Add(1);
                else
                    labels.Add(0);
            }
            return labels;
        }

        public static List<double> InitializeWeights(int featureCount)
        {
            List<double> weigths = new List<double>();
            for (int i = 0; i < featureCount; i++)
                weigths.Add(0);
            return weigths;
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
                //                feature.Add(ListToNormalList(tmp));
                feature.Add(tmp);
                //               feature.Add(listToNormalList2(tmp));
            }

            return feature;

        }

        public static void yazdir(List<double> data)
        {
            for (int i = 0; i < data.Count(); i++)
                Console.WriteLine(data[i]);
        }

        public static void Compare(List<double> labels, List<double> predLabel)
        {
            double birkenbir = 0;
            double birken0 = 0;
            double sıfırkensıfır = 0;
            double sıfırken1 = 0;
            for (int i = 0; i < labels.Count(); i++)
            {
                if (labels[i] == predLabel[i] && labels[i] == 1)
                    birkenbir++;
                if (labels[i] == predLabel[i] && labels[i] == 0)
                    sıfırkensıfır++;
                if (labels[i] != predLabel[i] && labels[i] == 0)
                    sıfırken1++;
                if (labels[i] != predLabel[i] && labels[i] == 1)
                    birken0++;
            }
            //            double rate = (double)(right / labels.Count());

            Console.WriteLine("Birkenbir " + birkenbir + " sıfırkensıfır " + sıfırkensıfır + " sıfırkenbir " + sıfırken1 + " birkensıfır " + birken0);
            var a = (birkenbir + sıfırkensıfır) / (birkenbir + sıfırkensıfır + sıfırken1 + birken0);
            Console.WriteLine("Dogruluk Oranı = " + a);
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
            }
            return normalData;
        }


        public static double Sigmoid(double t)
        {
            return (1.0 / (1 + Math.Exp(-t)));
        }

        public static List<double> Predict(List<List<double>> features, List<double> weigths)
        {
            List<double> z = new List<double>();
            double tmp = 0;

            for (int i = 0; i < features[0].Count(); i++)
            {
                tmp = 0;
                for (int j = 0; j < weigths.Count(); j++)
                {
                    tmp += features[j][i] + weigths[j];
                }
                //                    Console.WriteLine("Tmp = " + Sigmoid(tmp));
                Console.WriteLine("Predit Son " + tmp + "Sigmoig Tmp = " + Sigmoid(tmp));
                z.Add(Sigmoid(tmp));
            }
            return z;
        }

        public static double Cost(List<List<double>> features, List<double> weigths, List<double> labels)
        {
            List<double> predictions = new List<double>();
            double Cost1 = 0;
            double Cost2 = 0;
            predictions = Predict(features, weigths);

            for (int i = 0; i < predictions.Count(); i++)
            {
                Cost1 += Math.Log(predictions[i]) * labels[i];
            }
            for (int i = 0; i < predictions.Count(); i++)
            {
                Cost2 += Math.Log(1 - predictions[i]) * (1 - labels[i]);
            }

            //            Console.WriteLine(Cost1 + Cost2);
            return (Cost1 + Cost2) / labels.Count();


        }

        public static List<double> UpdateWeights(List<List<double>> features, List<double> weigths, List<double> labels, double lr = 0.01)
        {
            List<double> predictions = Predict(features, weigths);
            List<double> gradient = new List<double>();
            double tmp = 0;

            for (int i = 0; i < features.Count(); i++)
            {
                tmp = 0;
                for (int j = 0; j < predictions.Count(); j++)
                {
                    tmp += features[i][j] * (predictions[j] - labels[j]);
                }
                tmp /= features.Count();
                tmp *= lr;
                gradient.Add(tmp);
            }

            for (int i = 0; i < weigths.Count(); i++)
            {
                weigths[i] -= gradient[i];
                //                Console.WriteLine(weigths[i]);
            }
            return weigths;
        }

        public static int Desicion(double prob)
        {
            if (prob >= 0.5)
                return 1;
            else
                return 0;
        }

        public static List<double> Classify(List<double> predictions)
        {
            List<double> classify = new List<double>();
            for (int i = 0; i < predictions.Count(); i++)
            {
                Console.WriteLine("Olasılıklar "+ predictions[i]);
                classify.Add(Desicion(predictions[i]));
            }
            return classify;
        }

        public static List<double> Pred(List<double> predictions)
        {
            List<double> classify = new List<double>();
            for (int i = 0; i < predictions.Count(); i++)
            {
                classify.Add(Desicion(predictions[i]));
            }
            return classify;
        }


        public static List<double> Train(List<List<double>> features, List<double> weigths, List<double> labels, double lr = 0.01, int iter = 3000)
        {
            double cost = 0;
            for (int i = 0; i < iter; i++)
            {
                weigths = UpdateWeights(features, weigths, labels, lr);
                //                for (int j = 0; j < weigths.Count(); j++)
                //                    Console.WriteLine(" aaa " + weigths[j]);
                //                Console.WriteLine("---------");
                cost = Cost(features, weigths, labels);
                Console.WriteLine(i);
            }
            return weigths;
        }

    }
}


