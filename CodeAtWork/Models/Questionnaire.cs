using System;

namespace CodeAtWork.Models
{
    public class Questionnaire
    {
        public int VideoQuestionnaireId { get; set; }
        public Guid VideoId { get; set; }
        public string Question { get; set; }
    }


    public class QuestionnaireWithOptions : Questionnaire
    {
        public int VideoQuestionnaireOptionId { get; set; }
        public string OptionValue { get; set; }
    }
}