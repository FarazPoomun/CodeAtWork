using CodeAtWorkML.Model;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeAtWork.ML
{
    public class RecommendedWatchML
    {
        MLContext mlContext;
        private readonly static string DataFolderPath = @"C:\Users\P12480E\source\repos\CodeAtWork\CodeAtWorkML\Data";

        private readonly string TrainedModelForNextWatch = $@"{DataFolderPath}\TrainedModel\NextWatchRecommenderModel.zip";
        private readonly string TrainedModelForInterestWatch = $@"{DataFolderPath}\TrainedModel\InterestsRecommenderModel.zip";
        public RecommendedWatchML()
        {
            mlContext = new MLContext();
        }

        public void GetRecommendations()
        {
            UseModelForSinglePrediction(mlContext, LoadModel());
        }

        private ITransformer LoadModel()
        {
            ITransformer trainedModel = mlContext.Model.Load(TrainedModelForNextWatch, out DataViewSchema modelSchema);
            return trainedModel;
        }
  
        private void UseModelForSinglePrediction(MLContext mlContext, ITransformer model)
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<RecommendedWatchRating, RecommendedWatchPrediction>(model);

            List<RecommendedWatchRating> inputs = new List<RecommendedWatchRating>
            {
                new RecommendedWatchRating { prevWatch = 1, nextWatch = 2 },
                new RecommendedWatchRating { prevWatch = 1, nextWatch = 4 },
                new RecommendedWatchRating { prevWatch = 1, nextWatch = 6 }
            };

            foreach (var testInput in inputs)
            {
                var movieRatingPrediction = predictionEngine.Predict(testInput);
                if (Math.Round(movieRatingPrediction.Score, 1) > 3.5)
                {
                    Console.WriteLine("Movie " + testInput.prevWatch + " is recommended for user " + testInput.nextWatch);
                }
                else
                {
                    Console.WriteLine("Movie " + testInput.nextWatch + " is not recommended for user " + testInput.nextWatch);
                }
            }

            Console.ReadLine();
        }
    }
}