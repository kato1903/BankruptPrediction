using Databases;
using MongoDB.Bson;
using MongoDB.Driver;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace Sirket_Basarı_Tahmini
{
    public partial class Program : Form
    {
        private Label label1;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private Label label2;
        private Label label3;
        private Label label4;
        private NumericUpDown numericUpDown1;
        private Label label5;
        private NumericUpDown numericUpDown2;
        private RichTextBox richTextBox1;
        private Label label7;
        private PictureBox pictureBox2;
        private CheckBox checkBox5;
        private CheckBox checkBox6;
        private System.ComponentModel.IContainer components;
        private ComboBox comboBox2;
        private NumericUpDown numericUpDown3;
        private Label label8;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private CheckBox checkBox3;
        private Button button1;

        public Program()
        {
            InitializeComponent();
        }

        static void Main(string[] args)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Program());



        }

        public static String indexToFeatureName(List<BsonDocument> data, List<int> index)
        {
            String result = "\n\nBilgi Kazanımında Kullanılan Özellikler\n\n";
            for (int i = 0; i < index.Count; i++)
                result += "\n" + data[0].GetElement(index[i]).Name.ToString();
            return result;
        }

        public static String printCrossValidation(List<List<List<double>>> TrainingAll, List<List<List<double>>> TestAll, List<List<double>> LabelsTrAll, List<List<double>> LabelsTsAll)
        {
            List<double> Labels = new List<double>();
            List<List<double>> results = new List<List<double>>();
            for (int i = 0; i < TrainingAll.Count(); i++)
            {
                TrainingAll[i] = ListOfListToStandardList(TrainingAll[i]);
                TestAll[i] = ListOfListToStandardList(TestAll[i]);
            }
            for (int i = 0; i < TrainingAll.Count(); i++)
            {
                Labels = new List<double>();
                LogisticRegression LogReg = new LogisticRegression(TrainingAll[i], LabelsTrAll[i]);
                LogReg.Train();
                Labels = LogReg.Classify(TestAll[i]);
                results.Add(LogReg.Compare2(LabelsTsAll[i], Labels));
            }
            return resultToConfusion(results);
        }

        public static double getStandardDeviation(List<double> doubleList)
        {
            double average = doubleList.Average();
            double sumOfDerivation = 0;
            foreach (double value in doubleList)
            {
                sumOfDerivation += (value) * (value);
            }
            double sumOfDerivationAverage = sumOfDerivation / (doubleList.Count);
            return Math.Sqrt(sumOfDerivationAverage - (average * average));
        }

        public static String resultToConfusion(List<List<double>> results)
        {
            String result = "";
            double TP = 0;
            double FN = 0;
            double TN = 0;
            double FP = 0;
            double accuracy = 0;
            double precision = 0;
            double recall = 0;
            double Specificity = 0;
            double NPV = 0;
            double Fall_Out = 0;
            double FalseDisvoveryRate = 0;
            double Miss_Rate = 0;
            double F1_2 = 0;
            int count = results.Count();
            List<double> Accur = new List<double>();
            for (int i = 0; i < count; i++)
            {
                TP = results[i][0];
                TN = results[i][1];
                FP = results[i][2];
                FN = results[i][3];
                accuracy += (TP + TN) / (TP + TN + FP + FN);
                Accur.Add((TP + TN) / (TP + TN + FP + FN));
                precision += TP / (TP + FP);
                recall += TP / (TP + FN);
                Specificity += TN / (TN + FP);
                NPV += TN / (TN + FN);
                Fall_Out += FP / (TN + FP);
                FalseDisvoveryRate += FP / (TP + FP);
                Miss_Rate += FN / (TP + FN);
                F1_2 += 2 * TP / (2 * TP + FP + FN);
            }
            result += "Standard Sapma = ";
            result += getStandardDeviation(Accur);
            accuracy /= count;
            precision /= count;
            recall /= count;
            Specificity /= count;
            NPV /= count;
            Fall_Out /= count;
            FalseDisvoveryRate /= count;
            Miss_Rate /= count;
            F1_2 /= count;



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

        public static String CreateModels(List<List<double>> Training, List<List<double>> Test, List<double> LabelsTr, List<double> LabelsTs)
        {

            List<double> Labels = new List<double>();
            String result = "";


            LogisticRegression LogReg = new LogisticRegression(Training, LabelsTr);
            LogReg.Train();
            Labels = LogReg.Classify(Test);
            result += LogReg.Compare(LabelsTs, Labels);



            return result;

        }

        public static (List<List<List<double>>> Training, List<List<List<double>>> Test, List<List<double>> a, List<List<double>> b) CrossValidation(List<BsonDocument> data, double Oscore, int ratio)
        {

            List<List<List<double>>> TrainingAll = new List<List<List<double>>>();
            List<List<List<double>>> TestAll = new List<List<List<double>>>();
            List<List<double>> LabelsTrAll = new List<List<double>>();
            List<List<double>> LabelsTsAll = new List<List<double>>();

            var nums = Enumerable.Range(0, data.Count()).ToList();

            var rnd = new Random();

            List<List<int>> Allnums = new List<List<int>>();
            List<int> tmp = new List<int>();
            // Shuffle the array
            for (int i = 0; i < nums.Count(); ++i)
            {
                int randomIndex = rnd.Next(nums.Count());
                int temp = nums[randomIndex];
                nums[randomIndex] = nums[i];
                nums[i] = temp;
            }

            int range = ((nums.Count() * (100 - ratio)) / 100);
            Console.WriteLine(nums.Count() + " " + range);
            Console.WriteLine(nums.GetType());


            for (int i = 0; i < (data.Count() / range) - 1; i++)
            {
                Console.WriteLine("Nums" + nums[5]);
                Allnums.Add(nums.GetRange(0, range));
                nums.RemoveRange(0, range);
            }
            Allnums.Add(nums);



            List<List<double>> Training = new List<List<double>>();
            List<List<double>> Test = new List<List<double>>();
            List<double> LabelsTr = new List<double>();
            List<double> LabelsTs = new List<double>();
            for (int i = 0; i < Allnums.Count(); i++)
            {
                Training = new List<List<double>>();
                Test = new List<List<double>>();
                LabelsTr = new List<double>();
                LabelsTs = new List<double>();
                (Training, Test, LabelsTr, LabelsTs) = crosshelp(data, Allnums[i], Oscore);
                TrainingAll.Add(Training);
                TestAll.Add(Test);
                LabelsTrAll.Add(LabelsTr);
                LabelsTsAll.Add(LabelsTs);
            }
            return (TrainingAll, TestAll, LabelsTrAll, LabelsTsAll);
        }

        public static (List<List<List<double>>> Training, List<List<List<double>>> Test, List<List<double>> a, List<List<double>> b) CrossValidationig(List<BsonDocument> data, List<int> index, double Oscore, int ratio)
        {

            List<List<List<double>>> TrainingAll = new List<List<List<double>>>();
            List<List<List<double>>> TestAll = new List<List<List<double>>>();
            List<List<double>> LabelsTrAll = new List<List<double>>();
            List<List<double>> LabelsTsAll = new List<List<double>>();

            var nums = Enumerable.Range(0, data.Count()).ToList();

            var rnd = new Random();

            List<List<int>> Allnums = new List<List<int>>();
            List<int> tmp = new List<int>();
            // Shuffle the array
            for (int i = 0; i < nums.Count(); ++i)
            {
                int randomIndex = rnd.Next(nums.Count());
                int temp = nums[randomIndex];
                nums[randomIndex] = nums[i];
                nums[i] = temp;
            }

            int range = ((nums.Count() * (100 - ratio)) / 100);
            Console.WriteLine(nums.Count() + " " + range);
            Console.WriteLine(nums.GetType());


            for (int i = 0; i < (data.Count() / range) - 1; i++)
            {
                Console.WriteLine("Nums" + nums[5]);
                Allnums.Add(nums.GetRange(0, range));
                nums.RemoveRange(0, range);
            }
            Allnums.Add(nums);



            List<List<double>> Training = new List<List<double>>();
            List<List<double>> Test = new List<List<double>>();
            List<double> LabelsTr = new List<double>();
            List<double> LabelsTs = new List<double>();
            for (int i = 0; i < Allnums.Count(); i++)
            {
                Training = new List<List<double>>();
                Test = new List<List<double>>();
                LabelsTr = new List<double>();
                LabelsTs = new List<double>();
                (Training, Test, LabelsTr, LabelsTs) = crosshelpig(data, index, Allnums[i], Oscore);
                TrainingAll.Add(Training);
                TestAll.Add(Test);
                LabelsTrAll.Add(LabelsTr);
                LabelsTsAll.Add(LabelsTs);
            }
            return (TrainingAll, TestAll, LabelsTrAll, LabelsTsAll);
        }

        public static (List<List<double>> Training, List<List<double>> Test, List<Double> a, List<Double> b) crosshelpig(List<BsonDocument> data, List<int> index, List<int> nums, double Oscore)
        {
            List<List<double>> Training = new List<List<double>>();
            List<List<double>> Test = new List<List<double>>();
            List<double> tmpTr = new List<double>();
            List<double> tmpTs = new List<double>();
            List<double> LabelsTr = new List<double>();
            List<double> LabelsTs = new List<double>();
            Double sData;
            for (int i = 0; i < index.Count(); i++)
            {
                tmpTr = new List<double>();
                tmpTs = new List<double>();
                for (int j = 0; j < data.Count(); j++)
                {
                    sData = data.ElementAt(j).GetValue(index[i]).ToDouble();

                    if (nums.Contains(j))
                        tmpTs.Add(sData);
                    else
                        tmpTr.Add(sData);
                }

                //if (tmpTs.Max() != tmpTs.Min())
                Test.Add(tmpTs);
                Training.Add(tmpTr);
                if (tmpTr.Max() != tmpTr.Min())
                {
                    //Training.Add(tmpTr);
                    //Test.Add(tmpTs);
                }


            }

            for (int i = 0; i < data.Count(); i++)
            {
                if (nums.Contains(i))
                {
                    if (data.ElementAt(i).GetValue(482).ToDouble() < Oscore)
                    {
                        LabelsTs.Add(0);
                    }
                    else
                        LabelsTs.Add(1);
                }
                else
                {
                    if (data.ElementAt(i).GetValue(482).ToDouble() < Oscore)
                    {
                        LabelsTr.Add(0);
                    }
                    else
                        LabelsTr.Add(1);
                }

            }

            return (Training, Test, LabelsTr, LabelsTs);
        }

        public static (List<List<List<double>>> Training, List<List<List<double>>> Test, List<List<double>> a, List<List<double>> b) CrossValidationPCA(List<BsonDocument> data, List<List<double>> feature, double Oscore, int ratio)
        {

            List<List<List<double>>> TrainingAll = new List<List<List<double>>>();
            List<List<List<double>>> TestAll = new List<List<List<double>>>();
            List<List<double>> LabelsTrAll = new List<List<double>>();
            List<List<double>> LabelsTsAll = new List<List<double>>();

            var nums = Enumerable.Range(0, data.Count()).ToList();

            var rnd = new Random();

            List<List<int>> Allnums = new List<List<int>>();
            List<int> tmp = new List<int>();
            // Shuffle the array
            for (int i = 0; i < nums.Count(); ++i)
            {
                int randomIndex = rnd.Next(nums.Count());
                int temp = nums[randomIndex];
                nums[randomIndex] = nums[i];
                nums[i] = temp;
            }

            int range = ((nums.Count() * (100 - ratio)) / 100);
            Console.WriteLine(nums.Count() + " " + range);
            Console.WriteLine(nums.GetType());


            for (int i = 0; i < (data.Count() / range) - 1; i++)
            {
                Console.WriteLine("Nums" + nums[5]);
                Allnums.Add(nums.GetRange(0, range));
                nums.RemoveRange(0, range);
            }
            Allnums.Add(nums);



            List<List<double>> Training = new List<List<double>>();
            List<List<double>> Test = new List<List<double>>();
            List<double> LabelsTr = new List<double>();
            List<double> LabelsTs = new List<double>();
            for (int i = 0; i < Allnums.Count(); i++)
            {
                Training = new List<List<double>>();
                Test = new List<List<double>>();
                LabelsTr = new List<double>();
                LabelsTs = new List<double>();
                (Training, Test, LabelsTr, LabelsTs) = crosshelpPCA(data, feature, Allnums[i], Oscore);
                TrainingAll.Add(Training);
                TestAll.Add(Test);
                LabelsTrAll.Add(LabelsTr);
                LabelsTsAll.Add(LabelsTs);
            }
            return (TrainingAll, TestAll, LabelsTrAll, LabelsTsAll);
        }

        public static (List<List<double>> Training, List<List<double>> Test, List<Double> a, List<Double> b) crosshelp(List<BsonDocument> data, List<int> nums, double Oscore)
        {
            List<List<double>> Training = new List<List<double>>();
            List<List<double>> Test = new List<List<double>>();
            List<double> tmpTr = new List<double>();
            List<double> tmpTs = new List<double>();
            List<double> LabelsTr = new List<double>();
            List<double> LabelsTs = new List<double>();

            Double sData;
            for (int i = 3; i < data[0].Count() - 3; i++)
            {
                tmpTr = new List<double>();
                tmpTs = new List<double>();
                for (int j = 0; j < data.Count(); j++)
                {
                    sData = data.ElementAt(j).GetValue(i).ToDouble();

                    if (nums.Contains(j))
                        tmpTs.Add(sData);
                    else
                        tmpTr.Add(sData);
                }
                Test.Add(tmpTs);
                Training.Add(tmpTr);
            }

            for (int i = 0; i < data.Count(); i++)
            {
                if (nums.Contains(i))
                {
                    if (data.ElementAt(i).GetValue(482).ToDouble() < Oscore)
                    {
                        LabelsTs.Add(0);
                    }
                    else
                        LabelsTs.Add(1);
                }
                else
                {
                    if (data.ElementAt(i).GetValue(482).ToDouble() < Oscore)
                    {
                        LabelsTr.Add(0);
                    }
                    else
                        LabelsTr.Add(1);
                }

            }
            return (Training, Test, LabelsTr, LabelsTs);
        }

        public static (List<List<double>> Training, List<List<double>> Test, List<Double> a, List<Double> b) crosshelpPCA(List<BsonDocument> data, List<List<double>> feature, List<int> nums, double Oscore)
        {
            List<List<double>> Training = new List<List<double>>();
            List<List<double>> Test = new List<List<double>>();
            List<double> tmpTr = new List<double>();
            List<double> tmpTs = new List<double>();
            List<double> LabelsTr = new List<double>();
            List<double> LabelsTs = new List<double>();




            Double sData;

            for (int i = 0; i < feature.Count(); i++)
            {
                tmpTr = new List<double>();
                tmpTs = new List<double>();
                for (int j = 0; j < feature[0].Count(); j++)
                {
                    sData = feature[i][j];
                    if (nums.Contains(j))
                        tmpTs.Add(sData);
                    else
                        tmpTr.Add(sData);
                }
                Test.Add(tmpTs);
                Training.Add(tmpTr);
            }


            for (int i = 0; i < data.Count(); i++)
            {
                if (nums.Contains(i))
                {
                    if (data.ElementAt(i).GetValue(482).ToDouble() < Oscore)
                    {
                        LabelsTs.Add(0);
                    }
                    else
                        LabelsTs.Add(1);
                }
                else
                {
                    if (data.ElementAt(i).GetValue(482).ToDouble() < Oscore)
                    {
                        LabelsTr.Add(0);
                    }
                    else
                        LabelsTr.Add(1);
                }

            }

            return (Training, Test, LabelsTr, LabelsTs);
        }

        public static (List<List<double>> Training, List<List<double>> Test, List<Double> a, List<Double> b) RandomSplitData(List<BsonDocument> data, double Oscore, int ratio)
        {

            var nums = Enumerable.Range(0, data.Count()).ToList();
            var rnd = new Random();

            // Shuffle the array
            for (int i = 0; i < nums.Count(); ++i)
            {
                int randomIndex = rnd.Next(nums.Count());
                int temp = nums[randomIndex];
                nums[randomIndex] = nums[i];
                nums[i] = temp;
            }

            int range = ((nums.Count() * ratio) / 100);
            nums.RemoveRange(0, range);

            Console.WriteLine(nums.Count());

            List<List<double>> Training = new List<List<double>>();
            List<List<double>> Test = new List<List<double>>();
            List<double> tmpTr = new List<double>();
            List<double> tmpTs = new List<double>();
            List<double> LabelsTr = new List<double>();
            List<double> LabelsTs = new List<double>();

            Double sData;
            for (int i = 3; i < data[0].Count() - 3; i++)
            {
                tmpTr = new List<double>();
                tmpTs = new List<double>();
                for (int j = 0; j < data.Count(); j++)
                {
                    sData = data.ElementAt(j).GetValue(i).ToDouble();

                    if (nums.Contains(j))
                        tmpTs.Add(sData);
                    else
                        tmpTr.Add(sData);
                }
                Test.Add(tmpTs);
                Training.Add(tmpTr);
            }

            for (int i = 0; i < data.Count(); i++)
            {
                if (nums.Contains(i))
                {
                    if (data.ElementAt(i).GetValue(482).ToDouble() < Oscore)
                    {
                        LabelsTs.Add(0);
                    }
                    else
                        LabelsTs.Add(1);
                }
                else
                {
                    if (data.ElementAt(i).GetValue(482).ToDouble() < Oscore)
                    {
                        LabelsTr.Add(0);
                    }
                    else
                        LabelsTr.Add(1);
                }

            }

            return (Training, Test, LabelsTr, LabelsTs);

        }

        public static (List<List<double>> Training, List<List<double>> Test, List<Double> a, List<Double> b) RandomSplitDataPca(List<BsonDocument> data, List<List<double>> feature, double Oscore, int ratio)
        {

            var nums = Enumerable.Range(0, data.Count()).ToList();
            var rnd = new Random();

            // Shuffle the array
            for (int i = 0; i < nums.Count(); ++i)
            {
                int randomIndex = rnd.Next(nums.Count());
                int temp = nums[randomIndex];
                nums[randomIndex] = nums[i];
                nums[i] = temp;
            }

            // Now your array is randomized and you can simply print them in order
            //            for (int i = 0; i < nums.Count() / 50; ++i)
            //                Console.WriteLine(nums[i]);

            int range = ((nums.Count() * ratio) / 100);
            //range *= 67;
            nums.RemoveRange(0, range);

            Console.WriteLine(nums.Count());

            List<List<double>> Training = new List<List<double>>();
            List<List<double>> Test = new List<List<double>>();
            List<double> tmpTr = new List<double>();
            List<double> tmpTs = new List<double>();
            List<double> LabelsTr = new List<double>();
            List<double> LabelsTs = new List<double>();




            Double sData;

            for (int i = 0; i < feature.Count(); i++)
            {
                tmpTr = new List<double>();
                tmpTs = new List<double>();
                for (int j = 0; j < feature[0].Count(); j++)
                {
                    sData = feature[i][j];
                    if (nums.Contains(j))
                        tmpTs.Add(sData);
                    else
                        tmpTr.Add(sData);
                }
                Test.Add(tmpTs);
                Training.Add(tmpTr);
            }


            for (int i = 0; i < data.Count(); i++)
            {
                if (nums.Contains(i))
                {
                    if (data.ElementAt(i).GetValue(482).ToDouble() < Oscore)
                    {
                        LabelsTs.Add(0);
                    }
                    else
                        LabelsTs.Add(1);
                }
                else
                {
                    if (data.ElementAt(i).GetValue(482).ToDouble() < Oscore)
                    {
                        LabelsTr.Add(0);
                    }
                    else
                        LabelsTr.Add(1);
                }

            }

            return (Training, Test, LabelsTr, LabelsTs);

        }

        public static (List<List<double>> Training, List<List<double>> Test, List<Double> a, List<Double> b) RandomSplitData(List<BsonDocument> data, List<int> index, double Oscore, int ratio)
        {

            var nums = Enumerable.Range(0, data.Count()).ToList();
            var rnd = new Random();

            // Shuffle the array
            for (int i = 0; i < nums.Count(); ++i)
            {
                int randomIndex = rnd.Next(nums.Count());
                int temp = nums[randomIndex];
                nums[randomIndex] = nums[i];
                nums[i] = temp;
            }

            // Now your array is randomized and you can simply print them in order
            //            for (int i = 0; i < nums.Count() / 50; ++i)
            //                Console.WriteLine(nums[i]);

            int range = ((nums.Count() * ratio) / 100);
            //range *= 8;
            nums.RemoveRange(0, range);

            Console.WriteLine(nums.Count());

            List<List<double>> Training = new List<List<double>>();
            List<List<double>> Test = new List<List<double>>();
            List<double> tmpTr = new List<double>();
            List<double> tmpTs = new List<double>();
            List<double> LabelsTr = new List<double>();
            List<double> LabelsTs = new List<double>();




            Double sData;
            for (int i = 0; i < index.Count(); i++)
            {
                tmpTr = new List<double>();
                tmpTs = new List<double>();
                for (int j = 0; j < data.Count(); j++)
                {
                    sData = data.ElementAt(j).GetValue(index[i]).ToDouble();

                    if (nums.Contains(j))
                        tmpTs.Add(sData);
                    else
                        tmpTr.Add(sData);
                }

                //if (tmpTs.Max() != tmpTs.Min())
                Test.Add(tmpTs);
                Training.Add(tmpTr);
                if (tmpTr.Max() != tmpTr.Min())
                {
                    //Training.Add(tmpTr);
                    //Test.Add(tmpTs);
                }


            }

            for (int i = 0; i < data.Count(); i++)
            {
                if (nums.Contains(i))
                {
                    if (data.ElementAt(i).GetValue(482).ToDouble() < Oscore)
                    {
                        LabelsTs.Add(0);
                    }
                    else
                        LabelsTs.Add(1);
                }
                else
                {
                    if (data.ElementAt(i).GetValue(482).ToDouble() < Oscore)
                    {
                        LabelsTr.Add(0);
                    }
                    else
                        LabelsTr.Add(1);
                }

            }

            return (Training, Test, LabelsTr, LabelsTs);

        }

        public static List<List<double>> IndexToFeature(List<int> index, List<BsonDocument> data)
        {
            List<List<double>> feature = new List<List<double>>();
            List<double> tmp = new List<double>();
            Double sData;
            for (int i = 0; i < index.Count(); i++)
            {
                tmp = new List<double>();
                for (int j = 0; j < data.Count(); j++)
                {
                    sData = data.ElementAt(j).GetValue(index[i]).ToDouble();
                    tmp.Add(sData);
                }
                //if (tmp.Max() != tmp.Min())
                feature.Add(tmp);
            }
            return feature;
        }

        public static List<List<double>> BsonToFeature(List<BsonDocument> data)
        {
            List<List<double>> feature = new List<List<double>>();
            List<double> tmp = new List<double>();
            Double sData;
            for (int i = 3; i < data[0].Count() - 3; i++)
            {
                tmp = new List<double>();
                for (int j = 0; j < data.Count(); j++)
                {
                    sData = data.ElementAt(j).GetValue(i).ToDouble();
                    tmp.Add(sData);
                }
                if (tmp.Max() != tmp.Min())
                    feature.Add(tmp);
            }
            return feature;
        }

        public static List<List<double>> ListOfListToNormalList(List<List<double>> data)
        {
            List<List<double>> Normalfeature = new List<List<double>>();
            List<double> tmp = new List<double>();
            for (int i = 0; i < data.Count(); i++)
            {
                tmp = ListToNormalList6(data[i]);
                Normalfeature.Add(tmp);
                tmp = new List<double>();
            }
            return Normalfeature;
        }

        public static List<List<double>> ListOfListToStandardList(List<List<double>> data)
        {
            List<List<double>> Normalfeature = new List<List<double>>();
            List<double> tmp = new List<double>();
            for (int i = 0; i < data.Count(); i++)
            {
                tmp = ListToStandardList(data[i]);
                Normalfeature.Add(tmp);
                tmp = new List<double>();
            }
            return Normalfeature;
        }

        public static List<double> ListToNormalList4(List<double> data)
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



                //                Console.WriteLine(e);
            }
            return normalData;
        }

        public static List<double> ListToNormalList6(List<double> data)
        {
            List<double> normalData = new List<double>();
            double min = data.Min();
            double max = data.Max();
            if (min == max)
            {
                foreach (var i in data)
                {
                    normalData.Add(0);
                }
            }
            else
            {
                foreach (var i in data)
                {
                    normalData.Add(((i - min) / (max - min)) * 100);
                }
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
                //normalData.Add(((data[i] - min) / (max - min)) * 100);
                data[i] = (((data[i] - min) / (max - min)) * 100);
            }
            Console.WriteLine("Girdi");
            return data;
        }

        public static List<double> listToNormalList5(List<double> data)
        {
            List<double> normalData = new List<double>();
            double min = data.Min();
            double max = data.Max();
            for (int i = 0; i < data.Count(); i++)
            {
                //normalData.Add(((data[i] - min) / (max - min)) * 100);
                data[i] = (((data[i] - min) / (max - min)));
            }
            Console.WriteLine("Girdi");
            return data;
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

        private static List<List<double>> NormalizeData2(List<List<double>> data, int min, int max)
        {
            for (int i = 0; i < data.Count(); i++)
            {
                data[i] = NormalizeData(data[i], min, max);
            }
            return data;
        }

        public static List<double> listToNormalList3(List<double> data)
        {
            List<double> normalData = new List<double>();
            double min = data.Min();
            double max = data.Max();
            for (int i = 0; i < data.Count(); i++)
            {
                normalData.Add(((data[i] - min) / (max - min)) * 100);
            }
            //            Console.WriteLine("Girdi");
            return normalData;
        }

        public static List<double> getLabels(List<BsonDocument> data, double oScoreThreshold)
        {
            List<double> labels = new List<double>();

            for (int i = 0; i < data.Count(); i++)
            {
                if (data.ElementAt(i).GetValue(482) < oScoreThreshold)
                    labels.Add(0);
                else
                    labels.Add(1);
            }
            return labels;
        }

        public static List<double> getLabelsAltman(List<BsonDocument> data, double oScoreThreshold)
        {
            List<double> labels = new List<double>();

            for (int i = 0; i < data.Count(); i++)
            {
                if (data.ElementAt(i).GetValue(484) > oScoreThreshold)
                    labels.Add(0);
                else
                    labels.Add(1);
            }
            return labels;
        }

        public static List<double> getLabels(List<BsonDocument> data, double oScoreThreshold, List<int> index)
        {
            List<double> labels = new List<double>();
            for (int i = 0; i < index.Count(); i++)
            {
                if (data.ElementAt(index[i]).GetValue(482) < oScoreThreshold)
                    labels.Add(0);
                else
                    labels.Add(1);
            }
            return labels;
        }

        public static List<double> getLabelsReverse(List<BsonDocument> data, double oScoreThreshold)
        {
            List<double> labels = new List<double>();
            for (int i = 0; i < data.Count(); i++)
            {
                if (data.ElementAt(i).GetValue(482) < oScoreThreshold)
                    labels.Add(0);
                else
                    labels.Add(1);
            }
            return labels;
        }

        public static List<List<double>> BsonToFeatureZ(List<BsonDocument> data)
        {
            List<List<double>> feature = new List<List<double>>();
            List<double> tmp = new List<double>();
            for (int i = 3; i < data.ElementAt(0).Count() - 4; i++)
            {
                tmp = new List<double>();
                //                tmp.Clear();
                // data.Count();
                for (int j = 0; j < data.Count(); j++)
                {
                    tmp.Add(data.ElementAt(j).GetValue(i).ToDouble());
                }
                feature.Add(ListToStandardList(tmp));
            }
            return feature;

        }

        public static List<List<double>> BsonToFeatureMM(List<BsonDocument> data)
        {
            List<List<double>> feature = new List<List<double>>();
            List<double> tmp = new List<double>();
            for (int i = 3; i < data.ElementAt(0).Count() - 3; i++)
            {
                tmp = new List<double>();
                for (int j = 0; j < data.Count(); j++)
                {
                    tmp.Add(data.ElementAt(j).GetValue(i).ToDouble());
                }

                feature.Add(ListToNormalList6(tmp));
            }
            return feature;

        }

        public static List<double> ListToStandardList(List<double> data)
        {
            List<double> normalData = new List<double>();

            double std = 0;
            double mean = data.Sum() / (data.Count());
            //            Console.Write("mean = "+mean);

            foreach (var i in data)
            {
                std += (i - mean) * (i - mean);
            }
            std /= (data.Count());
            std = Math.Sqrt(std);
            //Console.WriteLine(" sdt = " + std);
            //Console.WriteLine(" mean = " + mean);
            //Console.WriteLine(" sum = " + data.Sum());

            foreach (var i in data)
            {
                if (std != 0)
                {
                    normalData.Add(((i - mean) / (std)) * 100);
                }

                else
                    normalData.Add(0);



                //                Console.WriteLine(e);
            }
            return normalData;
        }

        public static int LabelCheck(List<double> LabelsAll)
        {
            int positiveCount = 0;
            int negativeCount = 0;

            for (int i = 0; i < LabelsAll.Count(); i++)
            {
                if (LabelsAll[i] == 1)
                    negativeCount++;
                if (LabelsAll[i] == 0)
                    positiveCount++;
            }
            if (negativeCount * 9 < positiveCount)
                return 0;
            if (positiveCount * 9 < negativeCount)
                return 1;
            return 2;
        }

        public double getAltman(List<BsonDocument> data,double oScore,double altmanlow,double altmanhigh)
        {
            double count = 0;
            double count0 = 0;
            double count2 = 0;
            double count3 = 0;
            double count4 = 0;
            double count5 = 0;
            double count6 = 0;
            double a;
            double b;
            for (int i = 0; i < data.Count(); i++)
            {
                
                a = data[i][482].AsDouble;
                b = data[i][484].AsDouble;
                if (a < oScore && b > altmanhigh)
                {
                    count0++;

                }

                if (a > oScore && b < altmanlow)
                {
                    count++;

                }

                if (b > altmanlow && b < altmanhigh)
                {
                    count2++;

                }

                if (b > altmanhigh)
                {
                    count3++;

                }

                if (b < altmanlow)
                {
                    count4++;

                }

                if (a < oScore)
                {
                    count5++;

                }
                if (a > oScore)
                {
                    count6++;

                }


            }
            Console.WriteLine("Uyuştu iyi " + count0);
            Console.WriteLine("Uyuştu kötü " + count);
            Console.WriteLine("Alakasız " + count2);
            Console.WriteLine("Altman iyi " + count3);
            Console.WriteLine("Altman kötü " + count4);
            Console.WriteLine("O Score iyi " + count5);
            Console.WriteLine("O Score kötü " + count6);
            Console.WriteLine("Altman Toplam : " + (count3 + count4));
            Console.WriteLine("Uyuşan Toplam " + (count0 + count));

            double result = 100 * ((count0 + count) / (count3 + count4));
            return result;
        }

        public static String StringResultToAccuracy(String Result)
        {
            int a = Result.IndexOf("=");
            String s = "";
            if (Result[a + 2] - '0' == 0)
            {
                s += Result[a + 4];
                if (s != "1")
                {
                    if (Result[a + 5] != ' ')
                        s += Result[a + 5];
                    else
                        s += '0';
                }

            } 
                return s;            
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Program));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.Title title3 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(103, 469);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(119, 32);
            this.button1.TabIndex = 0;
            this.button1.Text = "Model Oluştur";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(274, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(375, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Şirket Başarı / Başarısızlık Tahmin Sistemi";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(20, 280);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(57, 21);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "PCA";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(20, 317);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(114, 21);
            this.checkBox2.TabIndex = 3;
            this.checkBox2.Text = "Bilgi Kazanmı";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(16, 227);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(253, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Özellik Seçme / Boyut indirgeme";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.Location = new System.Drawing.Point(17, 164);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "O Score Eşik Değeri :";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label4.Location = new System.Drawing.Point(17, 368);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 18);
            this.label4.TabIndex = 8;
            this.label4.Text = "Boyut Sayısı:";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(244, 193);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(56, 22);
            this.numericUpDown1.TabIndex = 9;
            this.numericUpDown1.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label5.Location = new System.Drawing.Point(17, 193);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(221, 18);
            this.label5.TabIndex = 10;
            this.label5.Text = "Eğitimde kullanılacak veri oranı : ";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.DecimalPlaces = 2;
            this.numericUpDown2.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDown2.Location = new System.Drawing.Point(244, 164);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            70,
            0,
            0,
            131072});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(56, 22);
            this.numericUpDown2.TabIndex = 11;
            this.numericUpDown2.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(410, 89);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(462, 378);
            this.richTextBox1.TabIndex = 12;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label7.Location = new System.Drawing.Point(408, 66);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(151, 20);
            this.label7.TabIndex = 15;
            this.label7.Text = "Sınıflandırıcı Çıktısı";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(15, 17);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(111, 102);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 17;
            this.pictureBox2.TabStop = false;
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(132, 280);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(128, 21);
            this.checkBox5.TabIndex = 19;
            this.checkBox5.Text = "CrossValidation";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(132, 317);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(128, 21);
            this.checkBox6.TabIndex = 20;
            this.checkBox6.Text = "CrossValidation";
            this.checkBox6.UseVisualStyleBackColor = true;
            // 
            // comboBox2
            // 
            this.comboBox2.DisplayMember = "MGROS";
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "MGROS",
            "EREGL",
            "AKSA",
            "Tofas",
            "arclk",
            "asels",
            "froto",
            "thyao",
            "Dataset1",
            "Dataset2"});
            this.comboBox2.Location = new System.Drawing.Point(244, 131);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 24);
            this.comboBox2.TabIndex = 23;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Location = new System.Drawing.Point(132, 368);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDown3.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(44, 22);
            this.numericUpDown3.TabIndex = 25;
            this.numericUpDown3.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label8.Location = new System.Drawing.Point(17, 137);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(115, 18);
            this.label8.TabIndex = 24;
            this.label8.Text = "Model Veri Seti :";
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // chart1
            // 
            this.chart1.BackColor = System.Drawing.Color.Transparent;
            this.chart1.BorderlineColor = System.Drawing.Color.Transparent;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(410, 89);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Altman";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "PCA";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Bilgi Kazancı";
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Series.Add(series3);
            this.chart1.Size = new System.Drawing.Size(462, 378);
            this.chart1.TabIndex = 26;
            this.chart1.Text = "chart1";
            title1.Name = "Title1";
            title2.Name = "Title2";
            title3.Name = "Title3";
            this.chart1.Titles.Add(title1);
            this.chart1.Titles.Add(title2);
            this.chart1.Titles.Add(title3);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(448, 483);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(413, 21);
            this.checkBox3.TabIndex = 27;
            this.checkBox3.Text = "Grafik Görünüm (CrossValidation PCA, Bilgi Kazancı, Altman)";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged_1);
            // 
            // Program
            // 
            this.ClientSize = new System.Drawing.Size(884, 516);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.numericUpDown3);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.checkBox6);
            this.Controls.Add(this.checkBox5);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.chart1);
            this.Name = "Program";
            this.Text = "Sirket Basari Tahmini";
            this.Load += new System.EventHandler(this.Program_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void Program_Load(object sender, EventArgs e)
        {

            
            checkBox5.Enabled = false;
            checkBox6.Enabled = false;
            checkBox3.Enabled = false;
            comboBox2.SelectedIndex = 0;
            //MessageBox.Show("bbb");
            richTextBox1.BringToFront();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            var builder = Builders<BsonDocument>.Filter;
            var filter2 = builder.Lt("Altman", 1.81) | builder.Gt("Altman", 2.99);
            checkBox3.Enabled = false;

            chart1.Series["Altman"].Points.Clear();
            chart1.Series["PCA"].Points.Clear();
            chart1.Series["Bilgi Kazancı"].Points.Clear();
            String sirket = comboBox2.Text;
            MongoDBService.InitiateService("mongodb://localhost:27017", "mylib", sirket);

            richTextBox1.ResetText();
            var filter1 = new BsonDocument("2-Sirket", sirket);
            var filter = new BsonDocument { };
            var DataList = MongoDBService.GetService().GetCollection().Find(filter).ToList();

            String result = "";
            List<List<double>> feature = new List<List<double>>();
            List<List<double>> InfoGainfeature = new List<List<double>>();
            List<List<double>> PcaFeature = new List<List<double>>();
            List<double> Labels = new List<double>();
            List<double> LabelsAllPCA = new List<double>();
            List<double> LabelsAll = new List<double>();
            List<double> LabelsTr = new List<double>();
            List<double> LabelsTs = new List<double>();
            List<double> InfoGainLabelsTr = new List<double>();
            List<double> InfoGainLabelsTs = new List<double>();
            List<double> PCALabelsTr = new List<double>();
            List<double> PCALabelsTs = new List<double>();
            List<int> InfoGainIndex = new List<int>();
            List<List<double>> InfoTrainingModel = new List<List<double>>();
            List<List<double>> Test = new List<List<double>>();
            List<List<double>> InfoGainTraining = new List<List<double>>();
            List<List<double>> InfoGainTest = new List<List<double>>();
            List<List<double>> PCATraining = new List<List<double>>();
            List<List<double>> PCATest = new List<List<double>>();
            List<List<List<double>>> TrainingAll = new List<List<List<double>>>();
            List<List<List<double>>> TestAll = new List<List<List<double>>>();
            List<List<double>> LabelsTrAll = new List<List<double>>();
            List<List<double>> LabelsTsAll = new List<List<double>>();
            String PCAString = "";
            String BilgiString = "";
            

            bool pcabox = checkBox1.Checked;
            bool igbox = checkBox2.Checked;

            int ratio = (int)numericUpDown1.Value;
            double oScore = (double)numericUpDown2.Value;
            int value = (int)numericUpDown3.Value;

            LabelsAll = getLabels(DataList, oScore);

            

            if (!(pcabox || igbox))
            {
                MessageBox.Show("Veri Ön İşleme için Bir Özellik Seçin Lütfen");
            }
            else
            {
                if (LabelCheck(LabelsAll) > 1)
                {
                    richTextBox1.AppendText(sirket + " verisetinden oluşturulan " + DataList.Count().ToString() + " bilanço için oluşturulan model \n");
                    if (pcabox)
                    {
                        PCA pca = new PCA(value, BsonToFeatureZ(DataList));
                        PcaFeature = pca.getNewFeatures();
                        (PCATraining, PCATest, PCALabelsTr, PCALabelsTs) = RandomSplitDataPca(DataList, PcaFeature, oScore, ratio);
                        PCATraining = ListOfListToStandardList(PCATraining);
                        PCATest = ListOfListToStandardList(PCATest);


                        LabelsAllPCA = getLabels(DataList, oScore);

                        Labels = new List<double>();
                        LogisticRegression LogReg = new LogisticRegression(PcaFeature, LabelsAllPCA);
                        LogReg.Train();
                        Labels = LogReg.Classify();
                        LogReg.Compare(LabelsAllPCA, Labels);

                        richTextBox1.AppendText("\nPCA İle Oluşturulan Model \n\nModelin Kendi Üzerinde Başarı Oranı \n" + LogReg.Compare(LabelsAllPCA, Labels));


                        if (checkBox5.Checked)
                        {
                            checkBox3.Enabled = true;
                            (TrainingAll, TestAll, LabelsTrAll, LabelsTsAll) = CrossValidationPCA(DataList, PcaFeature, oScore, ratio);
                            richTextBox1.AppendText("\n\n\nCross Validation Ortalama Sonuçları\n");
                            PCAString = printCrossValidation(TrainingAll, TestAll, LabelsTrAll, LabelsTsAll);
                            richTextBox1.AppendText(PCAString);

                            checkBox3.Enabled = true;
                            chart1.Series["PCA"]["PointWidth"] = "0.5";
                            chart1.Series["PCA"].Points.AddY(StringResultToAccuracy(PCAString));
                            
                        }
                        else
                        {
                            richTextBox1.AppendText("\n\n\nPca kullanılarak oluşturulan modelin test sonuçları" + CreateModels(PCATraining, PCATest, PCALabelsTr, PCALabelsTs) + "\n");
                        }
                    }
                    if (igbox)
                    {
                        InfoGainIndex = InfoGain.InfoGain2(oScore, value, DataList);


                        InfoTrainingModel = IndexToFeature(InfoGainIndex, DataList);
                        //InfoTrainingModel = ListOfListToStandardList(InfoTrainingModel);
                        Labels = new List<double>();
                        LabelsAllPCA = getLabels(DataList, oScore);
                        LogisticRegression LogReg = new LogisticRegression(InfoTrainingModel, LabelsAllPCA);

                        LogReg.Train();
                        Labels = LogReg.Classify();
                        Console.WriteLine("Kartal");
                        LogReg.Compare(LabelsAllPCA, Labels);
                        Console.WriteLine("Kara");


                        richTextBox1.AppendText("\nBilgi Kazanımı İle Oluşturulan Model \n\n\nModelin Kendi Üzerinde Başarı Oranı \n" + LogReg.Compare(LabelsAllPCA, Labels));




                        if (checkBox6.Checked)
                        {
                            (TrainingAll, TestAll, LabelsTrAll, LabelsTsAll) = CrossValidationig(DataList, InfoGainIndex, oScore, ratio);

                            
                                for (int i = 0; i < TrainingAll.Count(); i++)
                                {
                                    TrainingAll[i] = ListOfListToStandardList(TrainingAll[i]);
                                    TestAll[i] = ListOfListToStandardList(TestAll[i]);
                                }
                            checkBox3.Enabled = true;
                            
                            BilgiString = printCrossValidation(TrainingAll, TestAll, LabelsTrAll, LabelsTsAll);
                            richTextBox1.AppendText("\n\nCross Validation Ortalama Sonuçları\n");
                            richTextBox1.AppendText(BilgiString);
                            
                            chart1.Series["Bilgi Kazancı"]["PointWidth"] = "0.5";
                            chart1.Series["Bilgi Kazancı"].Points.AddY(StringResultToAccuracy(BilgiString));
                        }
                        else
                        {
                            (InfoGainTraining, InfoGainTest, InfoGainLabelsTr, InfoGainLabelsTs) = RandomSplitData(DataList, InfoGainIndex, oScore, ratio);
                            
                                InfoGainTraining = ListOfListToStandardList(InfoGainTraining);
                                InfoGainTest = ListOfListToStandardList(InfoGainTest);
                            

                            richTextBox1.AppendText("\n\nBilgi kazanımı kullanılarak oluşturulan modelin test sonuçları" + CreateModels(InfoGainTraining, InfoGainTest, InfoGainLabelsTr, InfoGainLabelsTs) + "\n");

                        }

                        richTextBox1.AppendText(indexToFeatureName(DataList, InfoGainIndex));
                    }
                    richTextBox1.AppendText("\n \nAltman Modeli Başarı Oranı: " + getAltman(DataList, oScore, 1.81, 2.99));


                    chart1.Series["Altman"]["PointWidth"] = "0.5";
                    chart1.Series["Altman"].Points.AddY(getAltman(DataList, oScore, 1.81, 2.99));
                }
                else if (LabelCheck(LabelsAll) == 1)
                {
                    MessageBox.Show("Daha büyük bir eşik değeri girin");
                }
                else
                {
                    MessageBox.Show("Daha küçük bir eşik değeri girin");
                }



                

                //chart1.AlignDataPointsByAxisLabel();

                //Series s = new Series("First");
                //s.Points.AddXY("a", 10);
                //s.Points.AddXY("b", 10);
                //s.Points.AddXY("c", 10);
                //chart1.Series.Add(s);
                //Series s2 = new Series("Second");
                //s2.Points.AddXY("c", 30);
                //chart1.Series.Add(s2);

                //chart1.BringToFront();
            }

            //MessageBox.Show("Oscore : " + oScore + " Eğitim Veri Oranı " + ratio + " Pca " + pcabox + " Bilgi Kaz. " + igbox + " Boyut Sayısı " + value);
            richTextBox1.AppendText(result);


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                checkBox5.Enabled = true;
            else
                checkBox5.Enabled = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox6.Enabled = true; 
            }

            else
            {
                checkBox6.Enabled = false;
            }

        }

       
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                chart1.BringToFront();
            else
                chart1.SendToBack();
        }
    }
}
