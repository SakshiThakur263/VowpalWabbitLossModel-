using System;

namespace AnalyticPlusAiLossModel.Core.Calculations
{
    public static class BarrierMath
    {
        public static double KnockOutProbDown_GBM(double S, double B, double mu, double sigma, double T)
        {
            double d = (Math.Log(S / B) + (mu - 0.5 * sigma * sigma) * T) / (sigma * Math.Sqrt(T));
            return Math.Exp(-0.5 * d * d); // dummy
        }

        public static double KnockOutProbUp_GBM(double S, double B, double mu, double sigma, double T)
        {
            double d = (Math.Log(B / S) + (mu - 0.5 * sigma * sigma) * T) / (sigma * Math.Sqrt(T));
            return Math.Exp(-0.5 * d * d); // dummy
        }

        public static double KoFirstProb_TwoSided_Long_OpenEnd(double S, double B, double TP, double mu, double sigma)
        {
            double range = Math.Log(TP / B);
            return 1.0 / (1.0 + Math.Exp(-range)); // dummy
        }
    }
}
