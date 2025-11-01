using System.Globalization;

namespace AnalyticPlusAiLossModel.Tools
{
    public static class TrainingDataGenerator
    {
        private static readonly Random rng = new(42);

        public static List<Dictionary<string, double>> Generate(int n)
        {
            var data = new List<Dictionary<string, double>>();
            Directory.CreateDirectory("Data");

            using var csv = new StreamWriter("Data/training_data.csv");
            using var vw = new StreamWriter("Data/training_data.vw");
            csv.WriteLine("label,logDistKO,sigma,mu,T,pAnalytic,hasTP,ratio");

            for (int i = 0; i < n; i++)
            {
                double sigma = 0.1 + rng.NextDouble() * 0.4;
                double mu = rng.NextDouble() * 0.02 - 0.01;
                double logDistKO = rng.NextDouble() * 0.5;
                double T = 0.02 + rng.NextDouble() * 0.2;
                double pAnalytic = 1 / (1 + Math.Exp(-5 * (0.3 - logDistKO)));
                double hasTP = rng.NextDouble() > 0.5 ? 1 : 0;
                double ratio = 0.5 + rng.NextDouble();

                int label = rng.NextDouble() < pAnalytic ? 1 : 0;

                csv.WriteLine($"{label},{logDistKO},{sigma},{mu},{T},{pAnalytic},{hasTP},{ratio}");
                vw.WriteLine($"{label} |f logDistKO:{logDistKO.ToString(CultureInfo.InvariantCulture)} sigma:{sigma.ToString(CultureInfo.InvariantCulture)} mu:{mu.ToString(CultureInfo.InvariantCulture)} T:{T.ToString(CultureInfo.InvariantCulture)} pAnalytic:{pAnalytic.ToString(CultureInfo.InvariantCulture)} hasTP:{hasTP} ratio:{ratio}");

                data.Add(new()
                {
                    ["logDistKO"] = logDistKO,
                    ["sigma"] = sigma,
                    ["mu"] = mu,
                    ["T"] = T,
                    ["pAnalytic"] = pAnalytic,
                    ["hasTP"] = hasTP,
                    ["ratio"] = ratio
                });
            }

            return data;
        }
    }
}
