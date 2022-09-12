using FluentValidation;
using Task3.Application.Coins.Commands;

namespace Task3.Application.Coins.Validators;

public class MoveCoinsCommandValidator : AbstractValidator<MoveCoinsCommand>
{
    private const string EMPTY_SRC_USER_MESSAGE =
        "Source user name can't be empty";
    private const string EMPTY_DST_USER_MESSAGE =
        "Destination user name can't be empty";
    private const string EQUAL_SRC_DST_USERS_MESSAGE =
        "Source user can't be a destination user";
    private const string NEGATIVE_AMOUNT_MESSAGE =
        "It's impossible to transfer non-positive amount of coins";

    public MoveCoinsCommandValidator()
    {
        RuleFor(c => c.SrcUserName).NotEmpty()
            .WithMessage(EMPTY_SRC_USER_MESSAGE);

        RuleFor(c => c.DstUserName).NotEmpty()
            .WithMessage(EMPTY_DST_USER_MESSAGE);

        RuleFor(c => c.SrcUserName).NotEqual(c => c.DstUserName)
            .WithMessage(EQUAL_SRC_DST_USERS_MESSAGE);

        RuleFor(c => c.Amount).GreaterThan(0)
            .WithMessage(NEGATIVE_AMOUNT_MESSAGE);
    }
}
