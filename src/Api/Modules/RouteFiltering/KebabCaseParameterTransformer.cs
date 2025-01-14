namespace Api.Modules.RouteFiltering;

public class KebabCaseParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        return value == null ? null : value.ToString()!.ToKebabCase();
    }
}