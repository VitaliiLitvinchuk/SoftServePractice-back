using FluentValidation;
using FluentValidation.AspNetCore;

namespace Api.Modules.Validator;

public static class Validators
{
    public static void AddValidators(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<Program>();
    }
}
