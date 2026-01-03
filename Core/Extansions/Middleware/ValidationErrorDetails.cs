using FluentValidation.Results;

namespace Core.Extensions
{
    public class ValidationErrorDetails : ErrorDetails
    {
        // FluentValidation'dan gelen hataları liste olarak tutacağız
        public IEnumerable<ValidationFailure> Errors { get; set; }
    }
}