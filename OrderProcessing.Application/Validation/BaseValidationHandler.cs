namespace OrderProcessing.Application.Validation
{
    public abstract class BaseValidationHandler : IOrderValidationHandler
    {
        private IOrderValidationHandler? _nextHandler;

        public IOrderValidationHandler SetNext(IOrderValidationHandler handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public ValidationResult Handle(ValidationContext context)
        {
            var result = Validate(context);

            if (!result.IsValid)
            {
                return result;
            }

            if (_nextHandler != null)
            {
                return _nextHandler.Handle(context);
            }

            return ValidationResult.Success();
        }

        protected abstract ValidationResult Validate(ValidationContext context);
    }
}
