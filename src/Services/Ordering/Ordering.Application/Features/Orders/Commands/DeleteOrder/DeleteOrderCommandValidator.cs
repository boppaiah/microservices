using FluentValidation;
using Ordering.Application.Features.Orders.Commands.Delete;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
    {
        public static string GetValidationMessage(string message)
        {
            message = "Validation failed for " + message;
            return message;
        }

        Func<string, string> messageProvider = GetValidationMessage;

        public DeleteOrderCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage(messageProvider("Id:{UserName} cannot be empty."))
                .NotNull();
        }
    }
}