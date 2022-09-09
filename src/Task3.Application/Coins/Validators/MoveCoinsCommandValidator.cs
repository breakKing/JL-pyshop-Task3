using FluentValidation;
using Task3.Application.Coins.Commands;

namespace Task3.Application.Coins.Validators;

public class MoveCoinsCommandValidator : AbstractValidator<MoveCoinsCommand>
{
    private const string NEGATIVE_SRC_USER_ID_MESSAGE =
        "Source user id can't be a non-positive value";
    private const string NEGATIVE_DST_USER_ID_MESSAGE =
        "Destination user id can't be a non-positive value";
    private const string NEGATIVE_AMOUNT_MESSAGE =
        "It's impossible to make an emission with non-positive amount of coins";

    public MoveCoinsCommandValidator()
    {
        RuleFor(c => c.SrcUserId).GreaterThan(0)
            .WithMessage(NEGATIVE_SRC_USER_ID_MESSAGE);

        RuleFor(c => c.DstUserId).GreaterThan(0)
            .WithMessage(NEGATIVE_DST_USER_ID_MESSAGE);

        RuleFor(c => c.Amount).GreaterThan(0)
            .WithMessage(NEGATIVE_AMOUNT_MESSAGE);
    }
}
