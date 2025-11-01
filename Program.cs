using System.Globalization;
using AnalyticPlusAiLossModel.Core.Calculations;
using AnalyticPlusAiLossModel.Ai;
using AnalyticPlusAiLossModel.Tools;
using AnalyticPlusAiLossModel.Core.Models;

namespace AnalyticPlusAiLossModel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Analytic + AI Loss & Profit Prediction Demo ===\n");

            // 1️⃣ Generate synthetic dataset
            var data = TrainingDataGenerator.Generate(500);
            Console.WriteLine($"✅ Generated {data.Count} synthetic rows.\n");

            // 2️⃣ Output directory
            string dataDir = Path.Combine(AppContext.BaseDirectory, "Data");
            Directory.CreateDirectory(dataDir);
            string outputCsv = Path.Combine(dataDir, "predicted_results_with_profit.csv");

            // 3️⃣ Pricing configuration
            var config = new PricingConfig
            {
                Teff_Years = 20.0 / 252.0
            };

            // 4️⃣ AI Calibrator (FakeAiCalibrator for now)
            IAiCalibrator ai = new FakeAiCalibrator();

            // 5️⃣ Analytic + AI loss model
            var lossModel = new AnalyticPlusAiLossModel.Core.Calculations.AnalyticPlusAiLossModel(ai);

            // 6️⃣ Extended predictor
            var predictor = new FutureOutcomePredictor(lossModel);

            // 7️⃣ Create CSV writer
            using var writer = new StreamWriter(outputCsv);
            writer.WriteLine("logDistKO,sigma,mu,T,pAnalytic,hasTP,ratio,LossProbability,ExpectedLoss,ExpectedProfit");

            // 8️⃣ Loop over all generated samples
            foreach (var row in data)
            {
                double logDistKO = row["logDistKO"];
                double sigma = row["sigma"];
                double mu = row["mu"];
                double T = row["T"];
                double pAnalytic = row["pAnalytic"];
                double hasTP = row["hasTP"];
                double ratio = row["ratio"];

                var product = new KnockOutDto
                {
                    KnockOutLevel = 950,
                    TakeProfitLevel = 1100,
                    Direction = hasTP > 0.5 ? Direction.Long : Direction.Short,
                    Ratio = ratio,
                    EndTimeUtc = DateTimeOffset.UtcNow.AddDays(20)
                };

                var request = LossProbabilityRequest.Create(product, 1000 * (1 - logDistKO), sigma, config, mu);
                var (expectedLoss, expectedProfit, lossProb) = predictor.PredictOutcomes(request);

                writer.WriteLine(string.Join(",",
                    logDistKO.ToString(CultureInfo.InvariantCulture),
                    sigma.ToString(CultureInfo.InvariantCulture),
                    mu.ToString(CultureInfo.InvariantCulture),
                    T.ToString(CultureInfo.InvariantCulture),
                    pAnalytic.ToString(CultureInfo.InvariantCulture),
                    hasTP.ToString(CultureInfo.InvariantCulture),
                    ratio.ToString(CultureInfo.InvariantCulture),
                    lossProb.ToString(CultureInfo.InvariantCulture),
                    expectedLoss.ToString(CultureInfo.InvariantCulture),
                    expectedProfit.ToString(CultureInfo.InvariantCulture)
                ));
            }

            Console.WriteLine($"✅ Saved predictions with loss/profit to: {outputCsv}");

            // 9️⃣ Example output
            var exampleProduct = new KnockOutDto
            {
                KnockOutLevel = 950,
                TakeProfitLevel = 1100,
                Direction = Direction.Long,
                Ratio = 1.0,
                EndTimeUtc = DateTimeOffset.UtcNow.AddDays(20)
            };

            var exampleRequest = LossProbabilityRequest.Create(exampleProduct, 1000, 0.25, config, 0.0);
            var (futureLoss, futureProfit, lossProbExample) = predictor.PredictOutcomes(exampleRequest);

            Console.WriteLine($"\n🎯 Example Results:");
            Console.WriteLine($"   • Loss Probability : {lossProbExample:P2}");
            Console.WriteLine($"   • Expected Loss    : {futureLoss:F2}");
            Console.WriteLine($"   • Expected Profit  : {futureProfit:F2}");
            Console.WriteLine("\n=== Finished successfully ===");
        }
    }
}
