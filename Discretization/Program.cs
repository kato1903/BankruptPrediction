using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Discretization
{
    class Program
    {
        public static MongoDBService A = new MongoDBService("mongodb://localhost:27017", "mylib", "Bilanco");

        static void Main(string[] args)
        {
            
            A.InitiateService("mongodb://localhost:27017", "mylib", "Bilanco");
            InfoGain(0.38);

        }

        public static double Entropi(List<int> All)
        {

            double E = 0;
            double sampleCount = All.Sum();
            //            Console.WriteLine(sampleCount);

            for (int i = 0; i < All.Count; i++)
            {
                if (All[i] != 0)
                    E += All[i] / sampleCount * Math.Log(All[i] / sampleCount, 2);
            }
            //            Console.WriteLine("aaaa " + E);
            return -E;
        }

        public static double CalInfoGain(double E, List<int> low, List<int> mid, List<int> high)
        {
            double Info = 0;
            double ratio1;
            double ratio2;
            double ratio3;

            ratio1 = (double)low.Sum() / (low.Sum() + mid.Sum() + high.Sum());
            ratio2 = (double)mid.Sum() / (low.Sum() + mid.Sum() + high.Sum());
            ratio3 = (double)high.Sum() / (low.Sum() + mid.Sum() + high.Sum());
            //            Console.WriteLine(ratio1 + " " + ratio2 + " " + ratio3);

            Info += ratio1 * Entropi(low) + ratio2 * Entropi(mid) + ratio3 * Entropi(high);
            return E - Info;
        }

        public static void getInfogainList(int start, int end, double oScoreThreshold)
        {
            double allEntropy = AllEntropy(oScoreThreshold);
            var filter = new BsonDocument { };
            var bilanco = A.GetService().GetCollection().Find(filter).ToList();
            String fName;

            List<double> info = new List<double>();
            for (int i = start; i <= end; i++)
            {
                fName = bilanco[0].GetElement(i).Name.ToString();
                //                Console.WriteLine(fName + " " + calculate(fName, i, oScoreThreshold, allEntropy));
//                info.Add(calculate2(fName, oScoreThreshold, allEntropy));
            }
            info.Sort();
            foreach (var document in info)
            {
                Console.WriteLine(document);
            }
        }

        public static double AllEntropy(double oScoreThreshold)
        {
            var filter = new BsonDocument { };
            var cursor = A.GetService().GetCollection().Find(filter).ToCursor();
            int bad = 0;
            int good = 0;
            foreach (var document in cursor.ToEnumerable())
            {
                //                Console.WriteLine(document.GetValue("Ohlson_O_Score"));
                if (document.GetValue("Ohlson_O_Score") > oScoreThreshold)
                    bad++;
                else
                    good++;
            }
            List<int> All = new List<int>();
            All.Add(good);
            All.Add(bad);
            return Entropi(All);
        }

        public static double calculate(string fName, int Test, double oScoreThreshold, double allEntropy)
        {

            //            MongoDBService M = new MongoDBService("mongodb://localhost:27017", "mylib", "Bilanco");
            //            M.InitiateService("mongodb://localhost:27017", "mylib", "Bilanco");

            var projection = Builders<BsonDocument>.Projection.Include(fName).Exclude("_id");

            var filter = new BsonDocument { };
            var bilanco = A.GetService().GetCollection().Find(filter).Project(projection).ToList();
            //            var metaData = M.GetService().GetCollection().Find(filter).ToList();
            Console.WriteLine("          aaaa " + bilanco[3].GetType() + bilanco[0].GetValue(0));

            List<double> sayilar = new List<double>();
            for (int i = 0; i < 354; i++)
                sayilar.Add(bilanco[i].GetValue(0).ToDouble());
            int low, high;
            sayilar.Sort();



            low = sayilar.Count() / 3;

            var lowFilter = Builders<BsonDocument>.Filter.Lte(fName, sayilar[low - 1]);
            var lowFilter2 = Builders<BsonDocument>.Filter.Lte(fName, sayilar[low - 1]).ToBson().ToList();
            //            var lowFilter2 = Builders<BsonDocument>.Filter.Lte(1, 2);

            Console.WriteLine(lowFilter2[1] + " " + lowFilter2[0]);

            //            var lowCursor = A.GetCollection().Find(lowFilter).ToCursor();
            var lowCursor2 = A.GetCollection().Find(lowFilter).ToList();
            high = ((sayilar.Count() - A.GetCollection().Find(lowFilter).ToList().Count()) / 2) + A.GetCollection().Find(lowFilter).ToList().Count();
            //            Console.WriteLine(A.GetCollection().Find(lowFilter).ToList().Count());
            if (A.GetCollection().Find(lowFilter).ToList().Count() == 354)
            {
                Console.WriteLine("Giriyor mu ");
                return 0;
            }
            high--;
            var highFilter = Builders<BsonDocument>.Filter.Gt(fName, sayilar[high]);

            //            var highCursor = A.GetCollection().Find(highFilter).ToCursor();
            var highCursor2 = A.GetCollection().Find(highFilter).ToList();
            //            Console.WriteLine(A.GetCollection().Find(highFilter).ToList().Count());



            var filterBuilder = Builders<BsonDocument>.Filter;

            var MidFilter = filterBuilder.Gt(fName, sayilar[low - 1]) & filterBuilder.Lte(fName, sayilar[high]);



            //            var midCursor = A.GetCollection().Find(MidFilter).ToCursor();
            var midCursor2 = A.GetCollection().Find(MidFilter).ToList();
            //            Console.WriteLine(A.GetCollection().Find(MidFilter).ToList().Count());


            int lowGood = 0;
            int lowBad = 0;
            int midGood = 0;
            int midBad = 0;
            int highGood = 0;
            int highBad = 0;


            foreach (var document in lowCursor2)
            {
                //                Console.WriteLine(document.GetValue("Ohlson_O_Score"));
                if (document.GetValue("Ohlson_O_Score") > oScoreThreshold)
                    lowBad++;
                else
                    lowGood++;
            }

            //foreach (var document in midCursor.ToEnumerable())
            //{
            //    //                Console.WriteLine(document.GetValue("Ohlson_O_Score"));
            //    if (document.GetValue("Ohlson_O_Score") > oScoreThreshold)
            //        midBad++;
            //    else
            //        midGood++;
            //}

            foreach (var document in midCursor2)
            {
                //                Console.WriteLine(document.GetValue("Ohlson_O_Score"));
                if (document.GetValue("Ohlson_O_Score") > oScoreThreshold)
                    midBad++;
                else
                    midGood++;
            }


            foreach (var document in highCursor2)
            {
                //                Console.WriteLine(document.GetValue("Ohlson_O_Score"));
                if (document.GetValue("Ohlson_O_Score") > oScoreThreshold)
                    highBad++;
                else
                    highGood++;
            }


            //            Console.WriteLine("lowGood = " + lowGood + " lowBad = " + lowBad);
            //            Console.WriteLine("midGood = " + midGood + " midBad = " + midBad);
            //            Console.WriteLine("highGood = " + highGood + " highBad = " + highBad);
            //            Console.WriteLine(lowGood + lowBad + midGood + midBad + highGood + highBad);



            List<int> lowL = new List<int>(); lowL.Add(lowBad); lowL.Add(lowGood);
            List<int> midL = new List<int>(); midL.Add(midBad); midL.Add(midGood);
            List<int> highL = new List<int>(); highL.Add(highBad); highL.Add(highGood);


            //            Console.WriteLine(InfoGain(Entropi(All), lowL, midL, highL));


            //            var update = Builders<BsonDocument>.Update.Set("InformationGain", InfoGain(Entropi(All), lowL, midL, highL));

            //            if (Test != 482)
            //                M.GetService().GetCollection().UpdateOne(metaData[Test - 3], update);

            //            if (InfoGain(Entropi(All), lowL, midL, highL) > 0.10)
            //            {
            //                Console.WriteLine(fName + " InfoGain = " + InfoGain(Entropi(All), lowL, midL, highL));
            //            }


            return CalInfoGain(allEntropy, lowL, midL, highL);
        }

        public static double InfoGain(double oScoreThreshold)
        {
            double allEntropy = AllEntropy(oScoreThreshold);
            var filter = new BsonDocument { };
            var DataList = A.GetService().GetCollection().Find(filter).ToList();
//            Console.WriteLine(DataList[0].GetValue(3).ToDouble());
            int lowGood = 0;
            int lowBad = 0;
            int midGood = 0;
            int midBad = 0;
            int highGood = 0;
            int highBad = 0;
            List<double> tmp = new List<double>();
            int low = DataList.Count() / 3;
            int high;

            for (int i = 3; i < DataList.ElementAt(0).Count() - 1; i++)
            {
                tmp = new List<double>();
                lowGood = 0;
                lowBad = 0;
                midGood = 0;
                midBad = 0;
                highGood = 0;
                highBad = 0;
                for (int j = 0; j < DataList.Count(); j++)
                {
                    tmp.Add(DataList.ElementAt(j).GetValue(i).ToDouble());
                }
                tmp.Sort();
                tmp.LastIndexOf(tmp[low]);
                high = ((DataList.Count() - tmp.LastIndexOf(tmp[low])) / 2) + tmp.LastIndexOf(tmp[low]);
//                Console.WriteLine("low  = " + low + " tmp[low]" + tmp[low] + " High = " + high + "tmp[high]" + tmp[high] + " Last index" + tmp.LastIndexOf(tmp[low]));
                for (int j = 0; j < DataList.Count(); j++)
                {
                    if (DataList.ElementAt(j).GetValue(i).ToDouble() <= tmp[low-1])
                    {
                        if (DataList.ElementAt(j).GetValue(482) < oScoreThreshold)
                            lowGood++;
                        else
                            lowBad++;
                    }
                    else if (DataList.ElementAt(j).GetValue(i).ToDouble() < tmp[high])
                    {
                        if (DataList.ElementAt(j).GetValue(482) < oScoreThreshold)
                            midGood++;
                        else
                            midBad++;
                    }
                    else
                    {
                        if (DataList.ElementAt(j).GetValue(482) < oScoreThreshold)
                            highGood++;
                        else
                            highBad++;
                    }
                }
                List<int> lowL = new List<int>(); lowL.Add(lowBad); lowL.Add(lowGood);
                List<int> midL = new List<int>(); midL.Add(midBad); midL.Add(midGood);
                List<int> highL = new List<int>(); highL.Add(highBad); highL.Add(highGood);
                //                Console.WriteLine(lowBad + " " + lowGood + " " + midBad + " " + midGood + " " + highBad + " " + highGood);
                int a = lowBad + lowGood;
                int b = midBad + midGood;
                int c = highBad + highGood;
//                Console.WriteLine(a + " " + b + " " + c);
//                Console.WriteLine("i = " + DataList[0].GetElement(i).Name.ToString() + " " + InfoGain(allEntropy, lowL, midL, highL));
                if(CalInfoGain(allEntropy, lowL, midL, highL) > 0.10)
                {
                    Console.WriteLine("i = " + i + " " + DataList[0].GetElement(i).Name.ToString() + " " + CalInfoGain(allEntropy, lowL, midL, highL));
                }
            }
            return 0;
        }
    }
}
