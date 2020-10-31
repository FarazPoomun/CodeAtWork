using CodeAtWorkML.Model;
using CodeAtWorkML.PathConfig;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Generic;
using System.IO;

namespace CodeAtWorkML
{
    class Program
    {
        private static bool trainForInterest;
        static void Main(string[] args)
        {
            MLContext mlContext = new MLContext();
            (IDataView trainingDataView, IDataView testDataView) = LoadData(mlContext);
            ITransformer model = BuildAndTrainModel(mlContext, trainingDataView);
            EvaluateModel(mlContext, testDataView, model);
            UseModelForSinglePrediction(mlContext, model);
            SaveModel(mlContext, trainingDataView.Schema, model);

            /*
             * Train for Interests
             */
            trainForInterest = true;
           (IDataView InterestTrainingDataView, IDataView InterestTestDataView) = LoadData(mlContext);
            model = BuildAndTrainModel(mlContext, InterestTrainingDataView);
            EvaluateModel(mlContext, InterestTestDataView, model);
            //UseModelForSinglePrediction(mlContext, model);
            SaveModel(mlContext, InterestTrainingDataView.Schema, model);
            Console.ReadLine();
        }

        public static (IDataView training, IDataView test) LoadData(MLContext mlContext)
        {
            if (!trainForInterest)
            {
                IDataView trainingDataView = mlContext.Data.LoadFromTextFile<RecommendedWatchRating>(PathsForML.trainingDataPathForNextWatch, hasHeader: true, separatorChar: ',');
                IDataView testDataView = mlContext.Data.LoadFromTextFile<RecommendedWatchRating>(PathsForML.testDataPathForNextWatch, hasHeader: true, separatorChar: ',');

                return (trainingDataView, testDataView);
            }
            else
            {
                IDataView trainingDataView = mlContext.Data.LoadFromTextFile<RecommendedWatchInterestRating>(PathsForML.trainingPathForInterestWatch, hasHeader: true, separatorChar: ',');
                IDataView testDataView = mlContext.Data.LoadFromTextFile<RecommendedWatchInterestRating>(PathsForML.testDataPathForInterestWatch, hasHeader: true, separatorChar: ',');

                return (trainingDataView, testDataView);

            }
        }

        public static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainingDataView)
        {
            if (!trainForInterest)
            {
                IEstimator<ITransformer> estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "prevWatchEncoded", inputColumnName: "prevWatch")
    .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "nextWatchEncoded", inputColumnName: "nextWatch"));

                var options = new MatrixFactorizationTrainer.Options
                {
                    MatrixColumnIndexColumnName = "prevWatchEncoded",
                    MatrixRowIndexColumnName = "nextWatchEncoded",
                    LabelColumnName = "Label",
                    NumberOfIterations = 20,
                    ApproximationRank = 100
                };

                var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));
                ITransformer model = trainerEstimator.Fit(trainingDataView);

                return model;
            }
            else
            {
                IEstimator<ITransformer> estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "VideoIdEncoded", inputColumnName: "VideoId")
               .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "InterestIdEncoded", inputColumnName: "InterestId"));

                var options = new MatrixFactorizationTrainer.Options
                {
                    MatrixColumnIndexColumnName = "VideoIdEncoded",
                    MatrixRowIndexColumnName = "InterestIdEncoded",
                    LabelColumnName = "Label",
                    NumberOfIterations = 20,
                    ApproximationRank = 100
                };

                var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));
                ITransformer model = trainerEstimator.Fit(trainingDataView);

                return model;
            }
        }

        public static void EvaluateModel(MLContext mlContext, IDataView testDataView, ITransformer model)
        {
            Console.WriteLine("=============== Evaluating the model ===============");
            var prediction = model.Transform(testDataView);
            var metrics = mlContext.Regression.Evaluate(prediction, labelColumnName: "Label", scoreColumnName: "Score");
            Console.WriteLine("Root Mean Squared Error : " + metrics.RootMeanSquaredError.ToString());
            Console.WriteLine("RSquared: " + metrics.RSquared.ToString());
        }

        public static void UseModelForSinglePrediction(MLContext mlContext, ITransformer model)
        {
            Console.WriteLine("=============== Making a prediction ===============");
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
        }

        public static void SaveModel(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
        {
            string modelPath = trainForInterest? PathsForML.TrainedModelForInterestWatch: PathsForML.TrainedModelForNextWatch;
            mlContext.Model.Save(model, trainingDataViewSchema, modelPath);
        }
    }
}
