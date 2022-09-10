using FluentValidation;
using Task3.Application.Coins.Commands;

namespace Task3.Application.Coins.Validators;

public class MoveCoinsCommandValidator : AbstractValidator<MoveCoinsCommand>
{
    private const string EMPTY_SRC_USER_MESSAGE =
        "Source user id can't be a non-positive value";
    private const string EMPTY_DST_USER_MESSAGE =
        "Destination user id can't be a non-positive value";
    private const string NEGATIVE_AMOUNT_MESSAGE =
        "It's impossible to make an emission with non-positive amount of coins";

    public MoveCoinsCommandValidator()
    {
        RuleFor(c => c.SrcUserName).NotEmpty()
            .WithMessage(EMPTY_SRC_USER_MESSAGE);

        RuleFor(c => c.DstUserName).NotEmpty()
            .WithMessage(EMPTY_DST_USER_MESSAGE);

        RuleFor(c => c.Amount).GreaterThan(0)
            .WithMessage(NEGATIVE_AMOUNT_MESSAGE);
    }
}
