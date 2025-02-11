using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

public class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider AuthenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationScheme = await AuthenticationSchemeProvider.GetAllSchemesAsync();
        
        const string bearerSecuritySchemeName = "Bearer";

        if (authenticationScheme.Any(authScheme => authScheme.Name == bearerSecuritySchemeName))
        {
            var requirements = new Dictionary<string, OpenApiSecurityScheme>
            {
                [bearerSecuritySchemeName] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = bearerSecuritySchemeName,
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token"
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;

            foreach(var operation in document.Paths.Values.SelectMany(p => p.Operations))
            {
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = bearerSecuritySchemeName, Type = ReferenceType.SecurityScheme } }] = Array.Empty<string>()
                });
            }            
    }
}
}
