using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TestWebApp.Infrastructure;
using TestWebApp.Infrastructure.Identity;

namespace TestWebApp
{
    public class JWTProvider
    {
    }

    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;
        private static TestWebContext context;
        private static SignInManager<ApplicationUser> _userManager;

        public TokenProviderMiddleware(
            RequestDelegate next,
            IOptions<TokenProviderOptions> options, IServiceProvider serviceProvider)
        {
            _next = next;
            _options = options.Value;
            context = (TestWebContext)serviceProvider.GetService(typeof(TestWebContext));
            _userManager = (SignInManager<ApplicationUser>)serviceProvider.GetService(typeof(SignInManager<ApplicationUser>));
        }

        public Task Invoke(HttpContext context)
        {
            // If the request path doesn't match, skip
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                return _next(context);
            }

            // Request must be POST with Content-Type: application/x-www-form-urlencoded
            if (!context.Request.Method.Equals("POST")
               || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            return GenerateToken(context);
        }

        private Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            // DON'T do this in production, obviously!
            //if (username == "test@test.com.au" && password == "Kareena_sun2006")
            var result = _userManager.PasswordSignInAsync(username, password, false,  false);
            if (result.Result.Succeeded)
            {
                return Task.FromResult(new ClaimsIdentity(new System.Security.Principal.GenericIdentity(username, "Token"), new Claim[] { }));
            }
            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }

        private async Task GenerateToken(HttpContext context)
        {
            var username = context.Request.Form["username"];
            var password = context.Request.Form["password"];

            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                if(context.Request.QueryString.HasValue)
                {
                    string [] arr = context.Request.QueryString.ToString().Trim('?').Split('&');
                    username = arr[0].ToString().Split('=')[1];
                    password = arr[1].ToString().Split('=')[1];

                    //for (int i = 0; i < arr.ToArray().Length; i++)
                    //{

                    //}

                }
            }

            var identity = await GetIdentity(username, password);
            if (identity == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid username or password.");
                return;
            }

            var now = DateTime.UtcNow;

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new Claim[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
            };

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials: _options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_options.Expiration.TotalSeconds
            };

            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }
    }

    public class TokenProviderOptions
    {
        public string Path { get; set; } = "/token";

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);

        public SigningCredentials SigningCredentials { get; set; }
    }

    public class CustomJwtDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string algorithm;
        private readonly TokenValidationParameters validationParameters;

        public CustomJwtDataFormat(string algorithm, TokenValidationParameters validationParameters)
        {
            this.algorithm = algorithm;
            this.validationParameters = validationParameters;
        }

        public AuthenticationTicket Unprotect(string protectedText)
            => Unprotect(protectedText, null);

        public AuthenticationTicket Unprotect(string protectedText, string purpose)
        {
            var handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = null;
            SecurityToken validToken = null;

            try
            {
                principal = handler.ValidateToken(protectedText, this.validationParameters, out validToken);

                var validJwt = validToken as JwtSecurityToken;

                if (validJwt == null)
                {
                    throw new ArgumentException("Invalid JWT");
                }

                if (!validJwt.Header.Alg.Equals(algorithm, StringComparison.Ordinal))
                {
                    throw new ArgumentException($"Algorithm must be '{algorithm}'");
                }

                // Additional custom validation of JWT claims here (if any)
            }
            catch (SecurityTokenValidationException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }

            // Validation passed. Return a valid AuthenticationTicket:
            return new AuthenticationTicket(principal, new AuthenticationProperties(), "Cookie");
        }

        // This ISecureDataFormat implementation is decode-only
        public string Protect(AuthenticationTicket data)
        {
            throw new NotImplementedException();
        }

        public string Protect(AuthenticationTicket data, string purpose)
        {
            throw new NotImplementedException();
        }
    }
}
