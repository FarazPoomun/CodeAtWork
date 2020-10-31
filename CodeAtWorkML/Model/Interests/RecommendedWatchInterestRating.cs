using Microsoft.ML.Data;
using System;

namespace CodeAtWorkML.Model
{
    public class RecommendedWatchInterestRating
    {
        [LoadColumn(0)]
        public float VideoId;
        [LoadColumn(1)]
        public float InterestId;

        [LoadColumn(2)]
        public float Label;
    }
}