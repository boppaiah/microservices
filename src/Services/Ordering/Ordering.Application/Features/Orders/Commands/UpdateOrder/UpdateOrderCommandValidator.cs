using FluentValidation;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public static string GetValidationMessage(string message)
        {
            message = "Validation failed for " + message;
            return message;
        }

        Func<string, string> messageProvider = GetValidationMessage;

        public UpdateOrderCommandValidator()
        {
            RuleFor(p => p.UserName)
                .NotEmpty().WithMessage(messageProvider("user:{UserName} cannot be empty."))
                .NotNull()
                .MaximumLength(50).WithMessage(messageProvider("user:{UserName} cannot have more than 50 charecters."));

            RuleFor(p => p.EmailAddress)
                .NotEmpty().WithMessage(messageProvider("email:{EmailAddress} cannot be empty."))
                .NotNull();

            RuleFor(p => p.TotalPrice)
                .NotEmpty().WithMessage(messageProvider("total price:{TotalPrice} cannot be empty."))
                .NotNull()
                .GreaterThan(0).WithMessage(messageProvider("total price cannot be less than 0."));

        }
    }
}