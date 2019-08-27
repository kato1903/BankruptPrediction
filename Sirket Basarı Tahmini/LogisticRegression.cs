using Databases;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sirket_Basarı_Tahmini
{
    class LogisticRegression
    {

        List<List<double>> Features;
        List<double> Labels;
        List<double> Weights;


        public LogisticRegression(List<List<double>> features, List<double> labels)
        {
            Features = features ?? throw new ArgumentNullException(nameof(features));
            Labels = labels ?? throw new ArgumentNullException(nameof(labels));
            Weights = InitializeWeights(features.Count());
        }

        public List<double> listToNormalList2(List<double> data)
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


        public List<double> getLabels(List<BsonDocument> data, double oScoreThreshold)
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

        public List<double> InitializeWeights(int featureCount)
        {
            List<double> weigths = new List<double>();
            for (int i = 0; i < featureCount; i++)
                weigths.Add(-10);
            return weigths;
        }

        public List<List<double>> BsonToFeature(List<BsonDocument> data)
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

        public void yazdir(List<double> data)
        {
            for (int i = 0; i < data.Count(); i++)
                Console.WriteLine(data[i]);
        }

        public String Compare(List<double> labels, List<double> predLabel)
        {
            String result = "";
            double TP = 0;
            double FN = 0;
            double TN = 0;
            double FP = 0;
            for (int i = 0; i < labels.Count(); i++)
            {
                if (((labels[i] == 1) && predLabel[i] == 1))
                    TP++;
                if (((labels[i] == 0) && predLabel[i] == 0))
                    TN++;
                if (((labels[i] == 0) && predLabel[i] == 1))
                    FP++;
                if (((labels[i] == 1) && predLabel[i] == 0))
                    FN++;
            }
            //            double rate = (double)(right / labels.Count());
            result += "\nConfusion Matrix ";
            Console.WriteLine("\nConfusion Matrix ");
            result += "\n" + TP + "\t" + FN + "\t" + (TP + FN) + " \n" + FP + "\t" + TN + "\t" + (FP + TN);
            Console.WriteLine("\n" + TP + "\t" + FN + "\t" + (TP + FN) + " \n" + FP + "\t" + TN + "\t" + (FP + TN));
            result += "\n" + (TP + FP) + "\t" + (FN + TN);
            Console.WriteLine("\n" + (TP + FP) + "\t" + (FN + TN));
            var accuracy = (TP + TN) / (TP + TN + FP + FN);
            var precision = TP / (TP + FP);
            var recall = TP / (TP + FN);
            var Specificity = TN / (TN + FP);
            var NPV = TN / (TN + FN);
            var Fall_Out = FP / (TN + FP);
            var FalseDisvoveryRate = FP / (TP + FP);
            var Miss_Rate = FN / (TP + FN);
            double F1 = 0;
            var F1_2 = 2 * TP / (2 * TP + FP + FN);
            if ((precision + recall) != 0)
                F1 = 2 * precision * recall / (precision + recall);
            result += "\nAccuracy = " + accuracy;
            Console.WriteLine("Accuracy = " + accuracy);
            result += "\nPrecision - Positive Predicted Value = " + precision;
            Console.WriteLine("Precision - Positive Predicted Value = " + precision);
            result += "\nRecall / Sensivitiy - True Positive Rate = " + recall;
            Console.WriteLine("Recall / Sensivitiy - True Positive Rate = " + recall);
            result += "\nSpecificity - True Negative Rate = " + Specificity;
            Console.WriteLine("Specificity - True Negative Rate = " + Specificity);
            result += "\nNegative Predicted Value = " + NPV;
            Console.WriteLine("Negative Predicted Value = " + NPV);

            result += "\nF1 Score = " + F1_2;
            Console.WriteLine("F1 Score = " + F1_2);

            result += "\n\nFall-out - False Positive Rate " + Fall_Out;
            //Console.WriteLine("\nFall-out - False Positive Rate " + Fall_Out);
            result += "\nFalse Discovery Rate = " + FalseDisvoveryRate;
            //Console.WriteLine("False Discovery Rate = " + FalseDisvoveryRate);
            result += "\nMiss Rate - False Negative Rate " + Miss_Rate;
            //Console.WriteLine("Miss Rate - False Negative Rate " + Miss_Rate);

            return result;
        }

        public List<double> Compare2(List<double> labels, List<double> predLabel)
        {
            List<double> results = new List<double>();
            double TP = 0;
            double FN = 0;
            double TN = 0;
            double FP = 0;
            for (int i = 0; i < labels.Count(); i++)
            {
                if (((labels[i] == 1) && predLabel[i] == 1))
                    TP++;
                if (((labels[i] == 0) && predLabel[i] == 0))
                    TN++;
                if (((labels[i] == 0) && predLabel[i] == 1))
                    FP++;
                if (((labels[i] == 1) && predLabel[i] == 0))
                    FN++;
            }
            results.Add(TP);
            results.Add(TN);
            results.Add(FP);
            results.Add(FN);

            return results;
        }

        public List<double> ListToNormalList(List<double> data)
        {
            List<double> normalData = new List<double>();

            double std = 0;
            double mean = data.Sum() / data.Count();
            //            Console.Write("mean = "+mean);

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


        public double Sigmoid(double t)
        {
            return (1.0 / (1 + Math.Exp(-t)));
        }

        public List<double> Predict(List<List<double>> features, List<double> weigths)
        {
            List<double> z = new List<double>();
            double tmp = 0;

            for (int i = 0; i < features[0].Count(); i++)
            {
                tmp = 0;

                for (int j = 0; j < weigths.Count(); j++)
                {
                    tmp += features[j][i] * weigths[j];
                }
                //                                    Console.WriteLine("Tmp = " + Sigmoid(tmp));
                tmp = Sigmoid(tmp);
                z.Add(tmp);
                //z.Add(tmp);
            }
            return z;
        }

        public double Cost(List<List<double>> features, List<double> weigths, List<double> labels)
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

            //Console.WriteLine("Cost ici " + Cost1 + Cost2);
            return (Cost1 + Cost2) / labels.Count();


        }

        public List<double> UpdateWeights(List<List<double>> features, List<double> weigths, List<double> labels, double lr = 0.1)
        {
            //Console.WriteLine(features.Count() + " " + weigths.Count());
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

        public int Desicion(double prob)
        {
            if (prob > 0.5)
                return 1;
            else
                return 0;
        }

        public List<double> Classify()
        {
            List<double> predictions = Predict(Features, Weights);
            //yazdir(predictions);
            Console.WriteLine("Feature Count " + Features.Count() + " Weight Count" + Weights.Count() + " Prediction " + predictions.Count() + " " + Features[0].Count());
            List<double> classify = new List<double>();
            for (int i = 0; i < predictions.Count(); i++)
            {
                //                Console.WriteLine("Probabilities " + predictions[i]);
                classify.Add(Desicion(predictions[i]));
            }
            return classify;
        }

        public List<double> Classify(List<List<Double>> abc)
        {
            List<double> predictions = Predict(abc, Weights);
            Console.WriteLine("Feature Count " + Features.Count() + " Weight Count" + Weights.Count() + " Prediction " + predictions.Count());
            List<double> classify = new List<double>();
            for (int i = 0; i < predictions.Count(); i++)
            {
                //Console.WriteLine("Probabilities " + predictions[i]);
                classify.Add(Desicion(predictions[i]));
            }
            return classify;
        }

        public List<double> Pred(List<double> predictions)
        {
            List<double> classify = new List<double>();
            for (int i = 0; i < predictions.Count(); i++)
            {
                classify.Add(Desicion(predictions[i]));
            }
            return classify;
        }


        public List<double> Train(double lr = 0.01, int iter = 7000)
        {
            //double cost = 0;
            for (int i = 0; i < iter; i++)
            {
                Weights = UpdateWeights(Features, Weights, Labels, lr);
                //                for (int j = 0; j < weigths.Count(); j++)
                //                    Console.WriteLine(" aaa " + weigths[j]);
                //                Console.WriteLine("---------");
                //                cost = Cost(Features, Weights, Labels);
                //                Console.WriteLine("Cost = " + cost);
                //                Console.WriteLine(Weights[0]);
            }
            return Weights;
        }

    }
}
