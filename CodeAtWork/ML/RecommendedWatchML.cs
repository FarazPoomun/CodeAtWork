using CodeAtWork.BL;
using CodeAtWork.Models.Misc;
using CodeAtWorkML.Model;
using Microsoft.ML;
using System;
using System.Collections.Generic;

namespace CodeAtWork.ML
{
    public class RecommendedWatchML
    {
        MLContext mlContext;
        ML_BL ML_BL;
        private readonly static string DataFolderPath = @"C:\Users\P12480E\source\repos\CodeAtWork\CodeAtWorkML\Data";

        private readonly string TrainedModelForNextWatch = $@"{DataFolderPath}\TrainedModel\NextWatchRecommenderModel.zip";
        private readonly string TrainedModelForInterestWatch = $@"{DataFolderPath}\TrainedModel\InterestsRecommenderModel.zip";
        private Dictionary<int, List<int>> MLVideoIds;

        public RecommendedWatchML()
        {
            mlContext = new MLContext();
            ML_BL = new ML_BL();
            MLVideoIds = ML_BL.GetMLVideoIds();
        }

        public List<RecommendedWatchRating> GetRecommendationsFromPreviousWatch(int MLVideoId)
        {
            return GetRecommendationsFromPreviousWatch(mlContext, LoadModel(), MLVideoId);
        }
        public List<RecommendedWatchInterestRating> GetRecommendationsFromInterests(List<MLConnectorInterest> userInterests)
        {
            return GetRecommendationsFromInterests(mlContext, LoadModel(true), userInterests);
        }

        private ITransformer LoadModel(bool forInterest = false)
        {
            ITransformer trainedModel;
            if (!forInterest)
            {
                trainedModel = mlContext.Model.Load(TrainedModelForNextWatch, out DataViewSchema modelSchema);
            }
            else
            {
                trainedModel = mlContext.Model.Load(TrainedModelForInterestWatch, out DataViewSchema modelSchema);
            }
            return trainedModel;
        }

        private List<RecommendedWatchRating> GetRecommendationsFromPreviousWatch(MLContext mlContext, ITransformer model, int MLVideoId)
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<RecommendedWatchRating, RecommendedWatchPrediction>(model);

            List<RecommendedWatchRating> inputs = new List<RecommendedWatchRating>();
            List<RecommendedWatchRating> outputs = new List<RecommendedWatchRating>();

            foreach (var key in MLVideoIds.Keys)
            {
                if (key != MLVideoId)
                {
                    inputs.Add(new RecommendedWatchRating()
                    {
                        prevWatch = MLVideoId,
                        nextWatch = key
                    });
                }
            }

            foreach (var testInput in inputs)
            {
                var movieRatingPrediction = predictionEngine.Predict(testInput);
                if (Math.Round(movieRatingPrediction.Score, 1) > 4)
                {
                    outputs.Add(testInput);
                }
            }
            return outputs;
        }

        internal List<RecommendedWatchInterestRating> GetRecommendationsFromInterests(MLContext mlContext, ITransformer model, List<MLConnectorInterest> userInterests)
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<RecommendedWatchInterestRating, RecommendedWatchInterestPrediction>(model);

            List<RecommendedWatchInterestRating> inputs = new List<RecommendedWatchInterestRating>();
            List<RecommendedWatchInterestRating> outputs = new List<RecommendedWatchInterestRating>();

            foreach (var row in MLVideoIds)
            {
                foreach (var val in row.Value)
                {
                    var input = new RecommendedWatchInterestRating()
                    {
                        VideoId = row.Key,
                        InterestId = val
                    };
                    inputs.Add(input);
                }
            }

            foreach (var testInput in inputs)
            {
                var movieRatingPrediction = predictionEngine.Predict(testInput);
                if (Math.Round(movieRatingPrediction.Score, 1) > 4.5)
                {
                    outputs.Add(testInput);
                }
            }
            return outputs;
        }
    }
}