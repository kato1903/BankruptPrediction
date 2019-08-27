using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altman
{
    class Altman
    {
        double workingCapital { get; set; }
        double totalAssets { get; set; }
        double retainedEarnings { get; set; }
        double ebit { get; set; }
        double equity { get; set; }
        double totalLiabilities { get; set; }
        double sales { get; set; }

        public Altman(double workingCapital, double totalAssets, double retainedEarnings, double ebit, double equity, double totalLiabilities, double sales)
        {
            this.workingCapital = workingCapital;
            this.totalAssets = totalAssets;
            this.retainedEarnings = retainedEarnings;
            this.ebit = ebit;
            this.equity = equity;
            this.totalLiabilities = totalLiabilities;
            this.sales = sales;
        }

        public double CalculateZScore()
        {
            return 1.2 * (workingCapital / totalAssets) + 1.4 * (retainedEarnings / totalAssets) + 3.3 * (ebit / totalAssets) + 0.6 * (equity / totalLiabilities) + (sales / totalAssets);
        }

    }
}
