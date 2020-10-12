namespace CodeAtWork.Models.Misc
{
    public class ValidationResult
    {
        public bool HasValidationFailed { get; set; }
        public string ValidationMsg { get; set; }
        public string AdditionalMsg { get; set; }
    }
}