using System;
using AnalyticPlusAiLossModel.Core.Models;

namespace AnalyticPlusAiLossModel.Core.Calculations
{
    /// <summary>
    /// Predicts future loss and profit amounts based on analytical loss probability
    /// and AI-calibrated loss probability model.
    /// </summary>
    public sealed class FutureOutcomePredictor
    {
        private readonly AnalyticPlusAiLossModel _lossModel;

        public FutureOutcomePredictor(AnalyticPlusAiLossModel lossModel)
        {
            _lossModel = lossModel;
        }

        /// <summary>
        /// Predict both future loss and profit given a product configuration.
        /// </summary>
        public (double futureLoss, double futureProfit, double lossProb) PredictOutcomes(LossProbabilityRequest request)
        {
            double lossProb = _lossModel.Predict(request);

            // Estimate potential downside (loss) and upside (profit)
            double potentialLoss = Math.Abs(request.Product.KnockOutLevel - request.Spot) * request.Product.Ratio;
            double potentialProfit = request.Product.TakeProfitLevel.HasValue
                ? Math.Abs(request.Product.TakeProfitLevel.Value - request.Spot) * request.Product.Ratio
                : potentialLoss * 1.2; // if no TP, assume 20% higher upside

            // Expected values (weighted by probability)
            double expectedLoss = lossProb * potentialLoss;
            double expectedProfit = (1 - lossProb) * potentialProfit;

            return (expectedLoss, expectedProfit, lossProb);
        }
    }
}
