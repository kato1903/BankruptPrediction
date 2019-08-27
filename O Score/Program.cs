
using Discretization;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O_Score
{
    class Program
    {
        public static MongoDBService A = new MongoDBService("mongodb://localhost:27017", "mylib", "arclk");
        static void Main(string[] args)
        {
            A.InitiateService("mongodb://localhost:27017", "mylib", "arclk");
            double oScoreThreshold = 0.10;
            var filter = new BsonDocument { };
            var bilanco = A.GetService().GetCollection().Find(filter).ToList();
            int count = 0;
            //int count0 = 0;
            //int count2 = 0;
            //int count3 = 0;
            //int count4 = 0;
            //int count5 = 0;
            //int count6 = 0;
            //double a;
            //double b;
            //for (int i = 0; i < bilanco.Count(); i++)
            //{
            //    a = bilanco[i][482].AsDouble;
            //    b = bilanco[i][484].AsDouble;
            //    if (a < 0.38 && b > 2.99)
            //    {
            //        count0++;

            //    }

            //    if (a > 0.38 && b < 1.81)
            //    {
            //        count++;

            //    }

            //    if (b > 1.81 && b < 2.99)
            //    {
            //        count2++;

            //    }

            //    if (b > 2.99)
            //    {
            //        count3++;

            //    }

            //    if (b < 1.81)
            //    {
            //        count4++;

            //    }

            //    if (a < 0.38)
            //    {
            //        count5++;

            //    }
            //    if (a > 0.38)
            //    {
            //        count6++;

            //    }


            //}
            //Console.WriteLine("Uyuştu iyi " + count0);
            //Console.WriteLine("Uyuştu kötü " + count);
            //Console.WriteLine("Alakasız " + count2);
            //Console.WriteLine("Altman iyi " + count3);
            //Console.WriteLine("Altman kötü " + count4);
            //Console.WriteLine("O Score iyi " + count5);
            //Console.WriteLine("O Score kötü " + count6);
            //Console.WriteLine("Altman Toplam : " + (count3 + count4));
            //Console.WriteLine("Uyuşan Toplam " + (count0 + count));

            for (int i = 0; i < bilanco.Count(); i++)
            {
                //double totalAsset = bilanco[i].GetValue("165-TOPLAM AKTİFLER").ToDouble();
                double totalAsset = bilanco[i].GetValue("162-Toplam Aktifler").ToDouble();

                double GNP = bilanco[i].GetValue("GNP").ToDouble();

                //              
                //double totalLiabilities = bilanco[i].GetValue("235-UZUN VADELİ YÜKÜMLÜLÜKLER").ToDouble() + bilanco[i].GetValue("166-KISA VADELİ YÜKÜMLÜLÜKLER").ToDouble();
                double totalLiabilities = bilanco[i].GetValue("232-Uzun Vadeli Yükümlülükler").ToDouble() + bilanco[i].GetValue("163-Kısa Vadeli Yükümlülükler").ToDouble();

                //double workingCapital = bilanco[i].GetValue("287-ÖZ SERMAYE (ANA ORTAKLIĞA AİT)").ToDouble();
                double workingCapital = bilanco[i].GetValue("280-Öz Sermaye (Ana Ortaklığa Ait)").ToDouble();

                double currentLiabilities = bilanco[i].GetValue("163-Kısa Vadeli Yükümlülükler").ToDouble();


                double currentAssets = bilanco[i].GetValue("3-Dönen Varlıklar").ToDouble();


                double netIncome = bilanco[i].GetValue("304-Net Dönem Karı").ToDouble();


                double netIncomeL = 0;


                if (i > 0)
                {
                    netIncomeL = bilanco[i - 1].GetValue("304-Net Dönem Karı").ToDouble();
                }
                double FFO = bilanco[i].GetValue("392-Net Esas Faaliyet Karı/Zararı").ToDouble();
                double Y = 0;
                if (i > 2)
                {
                    if (((bilanco[i - 2].GetValue("2-Sirket").ToString() == bilanco[i].GetValue("2-Sirket").ToString()) && bilanco[i - 2].GetValue("305-Dönem Zararı (-)").ToDouble() > 0) || ((bilanco[i - 1].GetValue("2-Sirket").ToString() == bilanco[i].GetValue("2-Sirket").ToString()) && bilanco[i - 1].GetValue("305-Dönem Zararı (-)").ToDouble() > 0))
                        Y = 1;
                }
                else
                    Y = 0;
                var Score = new Ohlson_O_Score(totalAsset, GNP, totalLiabilities, workingCapital, currentLiabilities, currentAssets, netIncome, netIncomeL, FFO, Y);


                if (Score.calculateOScoreProbability() > oScoreThreshold)
                {
                    count++;
                    Console.WriteLine("count = " + count + " " + Score.calculateOScoreProbability() + " " + bilanco[i].GetElement(2));
                }

                var update = Builders<BsonDocument>.Update.Set("Ohlson_O_Score", Score.calculateOScoreProbability());



                A.GetService().GetCollection().UpdateOne(bilanco[i], update);




            }
        }


    }
}
