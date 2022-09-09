using FluentValidation;
using Task3.Application.Coins.Commands;

namespace Task3.Application.Coins.Validators;

public class CoinsEmissionCommandValidator : AbstractValidator<CoinsEmissionCommand>
{
    private const string NEGATIVE_AMOUNT_MESSAGE =
        "It's impossible to make an emission with non-positive amount of coins";

    public CoinsEmissionCommandValidator()
    {
        RuleFor(c => c.Amount).GreaterThan(0)
            .WithMessage(NEGATIVE_AMOUNT_MESSAGE);
    }
}
