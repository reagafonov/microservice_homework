// using System.IdentityModel.Tokens.Jwt;
// using Microsoft.IdentityModel.Tokens;
// using Microsoft.VisualBasic;
//
// namespace keycloak_userEditor;
//
// public static class AppExtensions
// {
//     /// <summary>
//     /// Use JWT Authentication.
//     /// </summary>
//     /// <param name="app">The <see cref="IAppBuilder"/> to use.</param>
//     /// <param name="audiences">The expected audiences.  Will be validated in the token.</param>
//     /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
//     public static IApplicationBuilder UseJwtTokenAuthentication(
//         this IApplicationBuilder app,
//         params string[] audiences)
//     {
//         var validationParameters = new TokenValidationParameters
//         {
//             ValidateLifetime = true,
//             ValidateAudience = true,
//             ValidateIssuer = true,
//             ValidateActor = true,
//             ValidateIssuerSigningKey = true,
//             ValidAudiences = audiences,
//             IssuerSigningKeyResolver = Constants.GetSigningKey,
//         };
//
//         var tokenHandler = new JwtSecurityTokenHandler();
//         app.UseJwtBearerAuthentication(
//             new JwtBearerAuthenticationOptions
//             {
//                 AuthenticationMode = AuthenticationMode.Active,
//                 AllowedAudiences = audiences,
//                 TokenHandler = tokenHandler,
//                 TokenValidationParameters = validationParameters,
//             });
//
//         return app;
//     }
// }
