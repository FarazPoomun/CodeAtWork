using Microsoft.ML.Data;
using System;

namespace CodeAtWorkML.Model
{
    public class RecommendedWatchRating
    {
        [LoadColumn(0)]
        public float prevWatch;
        [LoadColumn(1)]
        public float nextWatch;

        [LoadColumn(2)]
        public float Label;
    }
}