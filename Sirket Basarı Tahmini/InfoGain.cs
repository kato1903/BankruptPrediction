using Databases;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sirket_Basarı_Tahmini
{
        

    class InfoGain
    {
        
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
            var bilanco = MongoDBService.GetService().GetCollection().Find(filter).ToList();
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
            var cursor = MongoDBService.GetService().GetCollection().Find(filter).ToCursor();
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
            var bilanco = MongoDBService.GetService().GetCollection().Find(filter).Project(projection).ToList();
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
            var lowCursor2 = MongoDBService.GetService().GetCollection().Find(lowFilter).ToList();
            high = ((sayilar.Count() - MongoDBService.GetService().GetCollection().Find(lowFilter).ToList().Count()) / 2) + MongoDBService.GetService().GetCollection().Find(lowFilter).ToList().Count();
            //            Console.WriteLine(A.GetCollection().Find(lowFilter).ToList().Count());
            if (MongoDBService.GetService().GetCollection().Find(lowFilter).ToList().Count() == 354)
            {
                Console.WriteLine("Giriyor mu ");
                return 0;
            }
            high--;
            var highFilter = Builders<BsonDocument>.Filter.Gt(fName, sayilar[high]);

            //            var highCursor = A.GetCollection().Find(highFilter).ToCursor();
            var highCursor2 = MongoDBService.GetService().GetCollection().Find(highFilter).ToList();
            //            Console.WriteLine(A.GetCollection().Find(highFilter).ToList().Count());



            var filterBuilder = Builders<BsonDocument>.Filter;

            var MidFilter = filterBuilder.Gt(fName, sayilar[low - 1]) & filterBuilder.Lte(fName, sayilar[high]);



            //            var midCursor = A.GetCollection().Find(MidFilter).ToCursor();
            var midCursor2 = MongoDBService.GetService().GetCollection().Find(MidFilter).ToList();
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

        public static List<int> InfoGain2(double oScoreThreshold,int count, List<BsonDocument> DataList)
        {
            double allEntropy = AllEntropy(oScoreThreshold);
            var filter = new BsonDocument { };
            //var DataList = MongoDBService.GetService().GetCollection().Find(filter).ToList();
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
            List<int> NewFeaturesIndex = new List<int>();
            List<double> values = new List<double>();
            List<Inf> inf = new List<Inf>();
            for (int i = 3; i < DataList.ElementAt(0).Count() - 2; i++)
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
                // tmp = ListToNormalList(tmp);
                tmp.Sort();
                tmp.LastIndexOf(tmp[low]);
                high = ((DataList.Count() - tmp.LastIndexOf(tmp[low])) / 2) + tmp.LastIndexOf(tmp[low]);
                //                Console.WriteLine("low  = " + low + " tmp[low]" + tmp[low] + " High = " + high + "tmp[high]" + tmp[high] + " Last index" + tmp.LastIndexOf(tmp[low]));
                for (int j = 0; j < DataList.Count(); j++)
                {
                    if (DataList.ElementAt(j).GetValue(i).ToDouble() <= tmp[low - 1])
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
                
                //if (CalInfoGain(allEntropy, lowL, midL, highL) > 0.168)
                //{
                //    Console.WriteLine("i = " + i + " " + DataList[0].GetElement(i).Name.ToString() + " " + CalInfoGain(allEntropy, lowL, midL, highL));
                if (i != 482 && i!= 481 && i!= 480)
                    inf.Add(new Inf(CalInfoGain(allEntropy, lowL, midL, highL), i));
                //Console.WriteLine(inf[0].value);
                    //InfoGains.Add(CalInfoGain(allEntropy, lowL, midL, highL), DataList[0].GetElement(i).Name.ToString());
                     //if(i!=482)
                     //    NewFeaturesIndex.Add(i);
                //}
            }
            inf.Sort((a, b) => a.value.CompareTo(b.value));
            inf.Reverse();
            for(int i = 0; i < count; i++)
            {
                //Console.WriteLine(DataList[0].GetElement(inf[i].name).Name.ToString() + " \t" + inf[i].value);
                NewFeaturesIndex.Add(inf[i].name);
            }
            return NewFeaturesIndex;
            //return CreateNewFeatures(NewFeaturesIndex);
        }

        public static List<List<double>> CreateNewFeatures(List<int> index)
        {
            List<double> tmp = new List<double>();
            List<List<double>> Ft = new List<List<double>>();
            var filter = new BsonDocument { };
            var DataList = MongoDBService.GetService().GetCollection().Find(filter).ToList();
            for (int i = 0; i < index.Count(); i++)
                Console.WriteLine(index[i]);


            Console.WriteLine(DataList.Count());
            for (int i = 0; i < index.Count(); i++)
            {
                tmp = new List<double>();
                for (int j = 0; j < DataList.Count(); j++)
                {
                    tmp.Add(DataList.ElementAt(j).GetValue(index[i]).ToDouble());
                }
                Ft.Add(tmp);
                tmp = new List<double>();
            }
            return Ft;
        }

        public static List<double> ListToNormalList2(List<double> data)
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

        internal class Inf
        {
            public double value { get; set; }
            public int name { get; set; }

            public Inf(double v1, int v2)
            {
                this.value = v1;
                this.name = v2;
            }
        }

    }

    
}
