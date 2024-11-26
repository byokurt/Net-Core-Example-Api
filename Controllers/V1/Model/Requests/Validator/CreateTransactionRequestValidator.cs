using DotnetCoreExampleApi.Controllers.V1.Model.Requests.Enums;
using FluentValidation;
using FluentValidation.Results;

namespace DotnetCoreExampleApi.Controllers.V1.Model.Requests.Validator;

public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
{
    protected override bool PreValidate(ValidationContext<CreateTransactionRequest> context, ValidationResult result)
    {
        if (context.InstanceToValidate == null)
        {
            result.Errors.Add(new ValidationFailure("Model", "Please ensure a model was supplied."));
            
            return false;
        }

        return true;
    }

    public CreateTransactionRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(model => model.Type).NotEmpty().IsInEnum().NotEqual(TransactionType.Unknown);
    }
}