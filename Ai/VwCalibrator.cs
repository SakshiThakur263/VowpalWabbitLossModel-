using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using VW;

namespace AnalyticPlusAiLossModel.Ai
{
    public sealed class VwCalibrator : IAiCalibrator, IDisposable
    {
        private readonly VowpalWabbit vw;

        public VwCalibrator(string args = "--loss_function=logistic --link=logistic --learning_rate 0.5 --l2 1e-6 --adaptive --invariant")
        {
            vw = new VowpalWabbit(args);
        }

        public double Calibrate(IDictionary<string, double> features)
        {
            string ex = " |f " + string.Join(" ", features.Select(kv => $"{kv.Key}:{kv.Value.ToString(CultureInfo.InvariantCulture)}"));

            using var sw = new StringWriter();
            var oldOut = Console.Out;
            Console.SetOut(sw);
            try { vw.Predict(ex); }
            finally { Console.SetOut(oldOut); }

            string output = sw.ToString().Trim();
            return double.TryParse(output, NumberStyles.Float, CultureInfo.InvariantCulture, out double pred)
                ? pred
                : 0.0;
        }

        public void Dispose() => vw.Dispose();
    }
}
