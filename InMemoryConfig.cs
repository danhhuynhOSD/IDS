using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Test;
using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Services;

namespace IdentityServer
{
    public static class InMemoryConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
          new List<IdentityResource>
          {
          new IdentityResources.OpenId(),
          new IdentityResources.Profile()
          };

        public static IEnumerable<ApiScope> GetApiScopes() =>
          new List<ApiScope> 
          { 
              new ApiScope("companyApi", "CompanyEmployee API"),
              new ApiScope("TMS_Api", "Training Management System API"),
              new ApiScope("LMS_Api", "Learning Management System API"),
          };

        public static IEnumerable<ApiResource> GetApiResources() =>
            new List<ApiResource>
            {
                new ApiResource("companyApi", "CompanyEmployee API")
                {
                    Scopes = { "companyApi" }
                },

                new ApiResource("TMS_Api",  "Training Management System API")
                {
                    Scopes = { "TMS_Api" }
                },

                new ApiResource("LMS_Api",  "Learning Management System API")
                {
                    Scopes = { "LMS_Api" }
                }
            };

        public static List<TestUser> GetUsers() =>
          new List<TestUser>
          {
              new TestUser
              {
                  SubjectId = "a9ea0f25-b964-409f-bcce-c923266249b4",
                  Username = "Mick",
                  Password = "MickPassword",
                  Claims = new List<Claim>
                  {
                      new Claim("given_name", "Mick"),
                      new Claim("family_name", "Mining")
                  }
              },
              new TestUser
              {
                  SubjectId = "c95ddb8c-79ec-488a-a485-fe57a1462340",
                  Username = "Jane",
                  Password = "JanePassword",
                  Claims = new List<Claim>
                  {
                      new Claim("given_name", "Jane"),
                      new Claim("family_name", "Downing")
                  }
              }
          };

        public class CustomProfileService : IProfileService
        {
            public Task GetProfileDataAsync(ProfileDataRequestContext context)
            {
                var sub = context.Subject.GetSubjectId();
                var user = InMemoryConfig.GetUsers()
                    .Find(u => u.SubjectId.Equals(sub));
                context.IssuedClaims.AddRange(user.Claims);
                return Task.CompletedTask;
            }
            public Task IsActiveAsync(IsActiveContext context)
            {
                var sub = context.Subject.GetSubjectId();
                var user = InMemoryConfig.GetUsers()
                    .Find(u => u.SubjectId.Equals(sub));
                context.IsActive = user != null;
                return Task.CompletedTask;
            }
        }

        public static IEnumerable<Client> GetClients() =>
            new List<Client>
            {
                new Client
                {
                    ClientName = "Angular-Client",
                    ClientId = "angular-client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = new List<string>{ "http://localhost:4200/signin-callback", "http://localhost:4200/assets/silent-callback.html" },
                    RequirePkce = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "TMS_Api"
                    },
                    AllowedCorsOrigins = { "http://localhost:4200" },
                    RequireClientSecret = false,
                    PostLogoutRedirectUris = new List<string> { "http://localhost:4200/signout-callback" },
                    RequireConsent = false,
                    AccessTokenLifetime = 1000
                },

                new Client
                {
                    ClientName = "React-Client",
                    ClientId = "react-client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = new List<string>{ "http://localhost:3000/signin-oidc", "http://localhost:3000/silent-renew"},
                    RequirePkce = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "LMS_Api"
                    },
                    AllowedCorsOrigins = { "http://localhost:3000" },
                    RequireClientSecret = false,
                    PostLogoutRedirectUris = new List<string> { "http://localhost:3000/signout-callback" },
                    RequireConsent = false,
                    AccessTokenLifetime = 1000
                }
             };
    }
}
