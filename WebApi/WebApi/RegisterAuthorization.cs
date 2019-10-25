using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi
{
    public static class RegisterAuthorization
    {
        public static void AddAuthorizationPolicy(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                ConfigureSystemFuncationalityAuthorization(options);
            });
        }

        public static void ConfigureSystemFuncationalityAuthorization(AuthorizationOptions options)
        {
            foreach (var item in PermissionConstants.SystemPermissionsSet)
            {
                options.AddPolicy(
                    item,
                    it => it.RequireAssertion(ctx =>
                     {
                         return ctx.User.IsInRole("Administrator") || ctx.User.HasClaim(PermissionConstants.PERMISSION_CLAIM_TYPE, item);
                     }));
            }
        }
    }
}
