namespace OrderProcessing.Application.Validation
{
    public record ValidationResult(bool IsValid, List<string> Errors)
    {
        public static ValidationResult Success() => new(true, new() { });
        public static ValidationResult Fail(string error) => new(false, new() { error });
    }
}
