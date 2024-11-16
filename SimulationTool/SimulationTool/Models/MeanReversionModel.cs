using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationTool.Models
{
    public class MeanReversionModel
    {
        public double Mu { get; set; } = 100;
        public double Theta { get; set; } = 0.1;
        public double Sigma { get; set; } = 1;
        public double InitialPrice { get; set; } = 110;
        public double TimeStep { get; set; } = 0.01;
        public int NumSteps { get; set; } = 1000;

        public List<double> SimulatePrices()
        {
            List<double> prices = new List<double>();
            double price = InitialPrice;
            Random rand = new Random();

            for (int t = 0; t < NumSteps; t++)
            {
                double randomNoise = NormalRandom(rand, 0, 1);
                price += Theta * (Mu - price) * TimeStep + Sigma * Math.Sqrt(TimeStep) * randomNoise;
                prices.Add(price);
            }

            return prices;
        }

        private double NormalRandom(Random rand, double mean, double stdDev)
        {
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }
    }
}
