using System.Collections.Generic;

namespace AnalyticPlusAiLossModel.Ai
{
    public interface IAiCalibrator
    {
        double Calibrate(IDictionary<string, double> features);
    }
}
