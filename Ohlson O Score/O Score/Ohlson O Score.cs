using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sirket_Basarı_Tahmini.NewFolder1
{
    class Ohlson_O_Score
    {
        double totalAsset { get; set; }
        double GNP { get; set; }
        double totalLiabilities { get; set; }
        double workingCapital { get; set; }
        double currentLiabilities { get; set; }
        double currentAssets { get; set; }
        double netIncome { get; set; }
        double netIncomeL { get; set; }
        double FFO { get; set; }
        double X { get; set; }
        double Y { get; set; }
        double[] C = { -1.32, -0.407, 6.03, -1.43, 0.0757, -1.72, -2.37, -1.83, 0.285, -0.521 };

        public Ohlson_O_Score(double totalAsset, double GNP, double totalLiabilities, double workingCapital, double currentLiabilities, double currentAssets, double netIncome, double netIncomeL, double FFO, double Y)
        {
            this.totalAsset = totalAsset;
            this.GNP = GNP;
            this.totalLiabilities = totalLiabilities;
            this.workingCapital = workingCapital;
            this.currentLiabilities = currentLiabilities;
            this.currentAssets = currentAssets;
            this.netIncome = netIncome;
            this.netIncomeL = netIncomeL;
            this.FFO = FFO;
            this.Y = Y;
        }

        public double calculateOScore()
        {
            return C[0] + CalX1() + CalX2() + CalX3() + CalX4() + CalX5() + CalX6() + CalX7() + CalX8() + CalX9();
        }

        public double calculateOScoreProbability()
        {
            return Math.Pow(Math.E, calculateOScore()) / (1 + Math.Pow(Math.E, calculateOScore()));
        }

        public int CalX()
        {
            if (totalLiabilities > totalAsset)
                return 1;
            return 0;
        }

        public double CalX1()
        {
            return C[1] * Math.Log(totalAsset / GNP);
        }

        public double CalX2()
        {
            return C[2] * totalLiabilities / totalAsset;
        }

        public double CalX3()
        {
            return C[3] * workingCapital / currentAssets;
        }

        public double CalX4()
        {
            return C[4] * currentLiabilities / totalAsset;
        }

        public double CalX5()
        {
            return C[5] * CalX();
        }

        public double CalX6()
        {
            return C[6] * netIncome / totalAsset;
        }

        public double CalX7()
        {
            return C[7] * FFO / totalLiabilities;
        }

        public double CalX8()
        {
            return C[8] * Y;
        }

        public double CalX9()
        {
            return C[9] * ((netIncome - netIncomeL) / (Math.Abs(netIncome) + Math.Abs(netIncomeL)));
        }

    }

    

}
