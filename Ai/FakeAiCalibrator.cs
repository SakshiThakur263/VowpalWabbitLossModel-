 using System.Collections.Generic;

namespace AnalyticPlusAiLossModel.Ai
{
    public sealed class FakeAiCalibrator : IAiCalibrator
    {
        // Returns zero correction (neutral baseline)
        public double Calibrate(IDictionary<string, double> features) => 0.0;
    }
}
