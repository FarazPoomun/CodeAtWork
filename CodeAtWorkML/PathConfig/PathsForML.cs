namespace CodeAtWorkML.PathConfig
{
    public struct PathsForML
    {
        private static readonly string DataFolderPath = @"C:\Users\P12480E\source\repos\CodeAtWork\CodeAtWorkML\Data";

        public static string trainingDataPathForNextWatch = $@"{DataFolderPath}\recommendation-ratings-train.csv";
        public static string testDataPathForNextWatch = $@"{DataFolderPath}\recommendation-ratings-test.csv";
        public static string trainingPathForInterestWatch = $@"{DataFolderPath}\recommendation-ratings-train.csv";
        public static string testDataPathForInterestWatch = $@"{DataFolderPath}\recommendation-ratings-test.csv";

        public static string TrainedModelForNextWatch = $@"{DataFolderPath}\TrainedModel\NextWatchRecommenderModel.zip";
        public static string TrainedModelForInterestWatch = $@"{DataFolderPath}\TrainedModel\InterestsRecommenderModel.zip";

    }
}
