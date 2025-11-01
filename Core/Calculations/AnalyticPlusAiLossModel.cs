using AnalyticPlusAiLossModel.Ai;
using AnalyticPlusAiLossModel.Core.Models;

namespace AnalyticPlusAiLossModel.Core.Calculations
{
    public sealed class AnalyticPlusAiLossModel : ILossProbabilityModel
    {
        private readonly IAiCalibrator ai;

        public AnalyticPlusAiLossModel(IAiCalibrator? ai = null)
        {
            this.ai = ai ?? new NoopAi();
        }

        public double Predict(LossProbabilityRequest r)
        {
            double pAnalytic;

            if (r.Product.TakeProfitLevel is double tp)
            {
                // two-sided open-end baseline
                pAnalytic = r.Product.Direction == Direction.Long
                    ? BarrierMath.KoFirstProb_TwoSided_Long_OpenEnd(r.Spot, r.Product.KnockOutLevel, tp, r.Mu, r.Sigma)
                    : BarrierMath.KoFirstProb_TwoSided_Long_OpenEnd(1.0 / r.Spot, 1.0 / r.Product.KnockOutLevel, 1.0 / tp, -r.Mu, r.Sigma);
            }
            else
            {
                pAnalytic = r.Product.Direction == Direction.Long
                    ? BarrierMath.KnockOutProbDown_GBM(r.Spot, r.Product.KnockOutLevel, r.Mu, r.Sigma, r.HorizonYears)
                    : BarrierMath.KnockOutProbUp_GBM(r.Spot, r.Product.KnockOutLevel, r.Mu, r.Sigma, r.HorizonYears);
            }

            var features = new Dictionary<string, double>
            {
                ["logDistKO"] = r.Product.Direction == Direction.Long
                    ? Math.Log(r.Spot / r.Product.KnockOutLevel)
                    : Math.Log(r.Product.KnockOutLevel / r.Spot),
                ["sigma"] = r.Sigma,
                ["mu"] = r.Mu,
                ["T"] = r.HorizonYears,
                ["pAnalytic"] = pAnalytic,
                ["hasTP"] = r.Product.TakeProfitLevel.HasValue ? 1.0 : 0.0,
                ["ratio"] = r.Product.Ratio
            };

            double correction = ai.Calibrate(features);

            // Blend in logit space
            double eps = 1e-9;
            double logitBase = Math.Log((pAnalytic + eps) / Math.Max(eps, 1 - pAnalytic));
            double logitAdj = logitBase + correction;
             double pFinal = 1.0 / (1.0 + Math.Exp(-logitAdj));

            return Math.Clamp(pFinal, 0.0, 1.0);
        }

        private sealed class NoopAi : IAiCalibrator
        {
            public double Calibrate(IDictionary<string, double> _) => 0.0;
        }
    }
}
