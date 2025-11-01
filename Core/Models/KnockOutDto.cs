using AnalyticPlusAiLossModel.Core.Models;
using System;

namespace AnalyticPlusAiLossModel.Core.Models
{
    public sealed class KnockOutDto
    {
        public required double KnockOutLevel { get; init; }
        public double? TakeProfitLevel { get; init; }
        public required Direction Direction { get; init; }
        public required double Ratio { get; init; }
        public DateTimeOffset? EndTimeUtc { get; init; }
    }
}
