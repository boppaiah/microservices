using FluentValidation.Results;

namespace Ordering.Application.Exception
{
    public class ValidationException : ApplicationException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException() :
            base($"One or more validation exceptions.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> validationFailures) :
            this()
        {
            Errors = validationFailures
                .GroupBy(e => e.PropertyName, p => p.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }
    }
}
