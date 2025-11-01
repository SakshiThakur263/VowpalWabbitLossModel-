using AnalyticPlusAiLossModel.Core.Models;

namespace AnalyticPlusAiLossModel.Core.Calculations
{
    public sealed class LossProbabilityRequest
    {
        public required KnockOutDto Product { get; init; }
        public required double Spot { get; init; }
        public required double Sigma { get; init; }
        public double Mu { get; init; } = 0.0;
        public double HorizonYears { get; init; }

        public static LossProbabilityRequest Create(KnockOutDto product, double spot, double sigma, PricingConfig config, double mu)
        {
            var horizon = product.EndTimeUtc is null
                ? config.Teff_Years
                : Math.Max(1.0 / 365.0, (product.EndTimeUtc.Value - DateTimeOffset.UtcNow).TotalDays / 365.0);

            return new()
            {
                Product = product,
                Spot = spot,
                Sigma = sigma,
                Mu = 0.0,
                HorizonYears = horizon
            };
        }
    }

    public interface ILossProbabilityModel
    {
        double Predict(LossProbabilityRequest r);
    }
}
