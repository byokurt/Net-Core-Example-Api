using FluentValidation;
using FluentValidation.Results;

namespace DotnetCoreExampleApi.Controllers.V1.Model.Requests.Validator;

public class QueryUserRequestValidator : AbstractValidator<QueryUsersRequest>
{
    protected override bool PreValidate(ValidationContext<QueryUsersRequest> context, ValidationResult result)
    {
        if (context.InstanceToValidate == null)
        {
            result.Errors.Add(new ValidationFailure("Model", "Please ensure a model was supplied."));
            
            return false;
        }

        return true;
    }

    public QueryUserRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(model => model.Page).GreaterThan(0);
        
        RuleFor(model => model.PageSize).ExclusiveBetween(0, 100);
    }
}

