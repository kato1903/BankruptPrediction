using Discretization;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altman
{
    class Program
    {
        public static MongoDBService A = new MongoDBService("mongodb://localhost:27017", "mylib", "arclk");

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

        static void Main(string[] args)
        {

            String a = "Cross Validation Ortalama Sonuçları Accuracy = 0,9 Precision - Positive Predicted Value = 0,747878787878788 Recall / Sensivitiy - True Positive Rate = 0,674603174603175 Specificity - True Negative Rate = 0,952276485734023 Negative Predicted Value = 0,94841931292751 F1 Score = 0,677232323232323";

            Console.WriteLine(StringResultToAccuracy(a));

            //A.InitiateService("mongodb://localhost:27017", "mylib", "arclk");
            //double oScoreThreshold = 0.0;
            //var filter = new BsonDocument { };
            //var bilanco = A.GetService().GetCollection().Find(filter).ToList();
            //int count = 0;
            //int count2 = 0;
            //for (int i = 0; i < bilanco.Count(); i++)
            //{
            //    //double totalAsset = bilanco[i].GetValue("165-TOPLAM AKTİFLER").ToDouble();
            //    double totalAsset = bilanco[i].GetValue("162-Toplam Aktifler").ToDouble();
                
            //    double GNP = bilanco[i].GetValue("GNP").ToDouble();

            //    //double totalLiabilities = bilanco[i].GetValue("385-TOPLAM PASİFLER").ToDouble();
            //    //double totalLiabilities = bilanco[i].GetValue("235-UZUN VADELİ YÜKÜMLÜLÜKLER").ToDouble();

            //    //double workingCapital = bilanco[i].GetValue("#288-Sermaye").ToDouble();

            //    //double retainedEarnings = bilanco[i].GetValue("#345-Kar Yedekleri").ToDouble();

            //    //double equity = bilanco[i].GetValue("287-ÖZ SERMAYE (ANA ORTAKLIĞA AİT)").ToDouble();

            //    //double sales = bilanco[i].GetValue("395-SATIŞ GELİRLERİ").ToDouble();

            //    //double ebit = bilanco[i].GetValue("451-VERGİ ÖNCESİ KAR/ZARAR").ToDouble();




            //    double totalLiabilities = bilanco[i].GetValue("232-Uzun Vadeli Yükümlülükler").ToDouble();

            //    double workingCapital = bilanco[i].GetValue("281-Sermaye").ToDouble();

            //    double retainedEarnings = bilanco[i].GetValue("290-Kar Yedekleri").ToDouble();

            //    double equity = bilanco[i].GetValue("280-Öz Sermaye (Ana Ortaklığa Ait)").ToDouble();

            //    double sales = bilanco[i].GetValue("385-Satış Gelirleri").ToDouble();

            //    double ebit = bilanco[i].GetValue("411-Vergi Öncesi Kar/Zarar").ToDouble();


            //    var Score = new Altman(workingCapital, totalAsset, retainedEarnings, ebit, equity, totalLiabilities, sales);


            //    if (Score.CalculateZScore() < 1.81)
            //    {
            //        count++;
            //        Console.WriteLine("count = " + i + " " + Score.CalculateZScore() + " " + bilanco[i].GetElement(2));
            //    }
                

            //    if (Score.CalculateZScore() > 2.99)
            //    {
            //        count2++;
            //        //Console.WriteLine("count = " + count2 + " " + Score.CalculateZScore() + " " + bilanco[i].GetElement(2));
            //    }

            //    var update = Builders<BsonDocument>.Update.Set("Altman", Score.CalculateZScore());



            //    A.GetService().GetCollection().UpdateOne(bilanco[i], update);


            //}

            //Console.WriteLine("iyi " + count2 + " kötü " + count);

        }
    }
}
