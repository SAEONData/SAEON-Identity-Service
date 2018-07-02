﻿using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Identity.Service.Config
{
    public class ConfigControllerLogic
    {
        public Client GetClientResource(string clientId)
        {
            Client clientResource = new Client();

            var data = GetClientResources().FirstOrDefault(x => x.ClientId == clientId);
            var dataModel = data.ToModel();
            if (data != null)
            {
                clientResource = new Client()
                {
                    dbid = data.Id,
                    Id = dataModel.ClientId,
                    Name = dataModel.ClientName,
                    IdentityTokenLifetime = dataModel.IdentityTokenLifetime,
                    AccessTokenLifetime = dataModel.AccessTokenLifetime,
                    GrantType = GrantTypeToString(dataModel.AllowedGrantTypes),
                    Secrets = data.ClientSecrets.Select(x => x.Value).ToList(),
                    Scopes = data.AllowedScopes.Select(x => x.Scope).ToList(),
                    CorsOrigins = data.AllowedCorsOrigins.Select(x => x.Origin).ToList(),
                    RedirectURIs = data.RedirectUris.Select(x => x.RedirectUri).ToList(),
                    PostLogoutRedirectURIs = data.PostLogoutRedirectUris.Select(x => x.PostLogoutRedirectUri).ToList(),
                    RequireConsent = data.RequireConsent,
                    RememberConsent = data.AllowRememberConsent,
                    OfflineAccess = data.AllowOfflineAccess,
                    AccessTokensViaBrowser = data.AllowAccessTokensViaBrowser
                };
            }

            return clientResource;
        }

        public List<IdentityServer4.EntityFramework.Entities.Client> GetClientResources()
        {
            var clientResources = new List<IdentityServer4.EntityFramework.Entities.Client>();

            var host = Program.host;
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                using (var context = serviceProvider.GetRequiredService<ConfigurationDbContext>())
                {
                    try
                    {
                        clientResources = context.Clients
                            .Include(c => c.AllowedGrantTypes)
                            .Include(c => c.ClientSecrets)
                            .Include(c => c.AllowedScopes)
                            .Include(c => c.AllowedCorsOrigins)
                            .Include(c => c.RedirectUris)
                            .Include(c => c.PostLogoutRedirectUris)
                            .OrderBy(c => c.ClientId).ToList();
                    }
                    catch (Exception ex)
                    {
                        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "Unabled to get ClientResources from DB.");
                    }

                }
            }

            return clientResources;
        }

        public IdentityServer4.EntityFramework.Entities.Client BuildClient(Client client)
        {
            var isClient = new IdentityServer4.Models.Client
            {
                ClientId = client.Id,
                ClientName = client.Name,
                RequireConsent = client.RequireConsent,
                AllowRememberConsent = client.RememberConsent,
                AllowOfflineAccess = client.OfflineAccess,
                AllowAccessTokensViaBrowser = client.AccessTokensViaBrowser,
                IdentityTokenLifetime = client.IdentityTokenLifetime,
                AccessTokenLifetime = client.AccessTokenLifetime,
                AllowedGrantTypes = StringToGrantType(client.GrantType)
            };

            if (client.Secrets != null)
            {
                foreach (var secret in client.Secrets)
                {
                    isClient.ClientSecrets.Add(new Secret(secret.Sha256()));
                }
            }

            if (client.Scopes != null)
            {
                foreach (var scope in client.Scopes)
                {
                    isClient.AllowedScopes.Add(scope);
                }
            }

            if (client.CorsOrigins != null)
            {
                foreach (var corsOrigin in client.CorsOrigins)
                {
                    isClient.AllowedCorsOrigins.Add(corsOrigin);
                }
            }

            if (client.RedirectURIs != null)
            {
                foreach (var uri in client.RedirectURIs)
                {
                    isClient.RedirectUris.Add(uri);
                }
            }

            if (client.PostLogoutRedirectURIs != null)
            {
                foreach (var uri in client.PostLogoutRedirectURIs)
                {
                    isClient.PostLogoutRedirectUris.Add(uri);
                }
            }

            var isEntClient = isClient.ToEntity();
            isEntClient.Id = client.dbid;

            return isEntClient;
        }

        public ICollection<string> StringToGrantType(string value)
        {
            switch (value.ToLower())
            {
                case "clientcredentials":
                    return GrantTypes.ClientCredentials;
                case "code":
                    return GrantTypes.Code;
                case "codeandclientcredentials":
                    return GrantTypes.CodeAndClientCredentials;
                case "hybrid":
                    return GrantTypes.Hybrid;
                case "hybridandclientcredentials":
                    return GrantTypes.HybridAndClientCredentials;
                case "implicit":
                    return GrantTypes.Implicit;
                case "implicitandclientcredentials":
                    return GrantTypes.ImplicitAndClientCredentials;
                case "resourceownerpassword":
                    return GrantTypes.ResourceOwnerPassword;
                case "resourceownerpasswordandclientcredentials":
                    return GrantTypes.ResourceOwnerPasswordAndClientCredentials;
                default:
                    return new List<string>();
            }
        }

        public string GrantTypeToString(ICollection<string> value)
        {        
            if (value.OrderBy(x => x).SequenceEqual(GrantTypes.ClientCredentials.OrderBy(x => x)))
                return "ClientCredentials";
            else if (value.OrderBy(x => x).SequenceEqual(GrantTypes.Code.OrderBy(x => x)))
                return "Code";
            else if (value.OrderBy(x => x).SequenceEqual(GrantTypes.CodeAndClientCredentials.OrderBy(x => x)))
                return "CodeAndClientCredentials";
            else if (value.OrderBy(x => x).SequenceEqual(GrantTypes.Hybrid.OrderBy(x => x)))
                return "Hybrid";
            else if (value.OrderBy(x => x).SequenceEqual(GrantTypes.HybridAndClientCredentials.OrderBy(x => x)))
                return "HybridAndClientCredentials";
            else if (value.OrderBy(x => x).SequenceEqual(GrantTypes.Implicit.OrderBy(x => x)))
                return "Implicit";
            else if (value.OrderBy(x => x).SequenceEqual(GrantTypes.ImplicitAndClientCredentials.OrderBy(x => x)))
                return "ImplicitAndClientCredentials";
            else if (value.OrderBy(x => x).SequenceEqual(GrantTypes.ResourceOwnerPassword.OrderBy(x => x)))
                return "ResourceOwnerPassword";
            else if (value.OrderBy(x => x).SequenceEqual(GrantTypes.ResourceOwnerPasswordAndClientCredentials.OrderBy(x => x)))
                return "ResourceOwnerPasswordAndClientCredentials";
            else
                return "";
        }

        public bool SaveClient(IdentityServer4.EntityFramework.Entities.Client client)
        {
            bool result = false;

            try
            {
                var host = Program.host;
                using (var scope = host.Services.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    using (var context = serviceProvider.GetRequiredService<ConfigurationDbContext>())
                    {
                        try
                        {
                            var dbClient = context.Clients
                                .Include(c => c.AllowedGrantTypes)
                                .Include(c => c.ClientSecrets)
                                .Include(c => c.AllowedScopes)
                                .Include(c => c.AllowedCorsOrigins)
                                .Include(c => c.RedirectUris)
                                .Include(c => c.PostLogoutRedirectUris)
                                .FirstOrDefault(x => x.Id == client.Id);

                            if (dbClient != null)
                            {
                                //UPDATE
                                dbClient.ClientId = client.ClientId;
                                dbClient.ClientName = client.ClientName;
                                dbClient.IdentityTokenLifetime = client.IdentityTokenLifetime;
                                dbClient.AccessTokenLifetime = client.AccessTokenLifetime;
                                dbClient.AllowedGrantTypes = client.AllowedGrantTypes;
                                dbClient.ClientSecrets = client.ClientSecrets;
                                dbClient.AllowedScopes = client.AllowedScopes;
                                dbClient.AllowedCorsOrigins = client.AllowedCorsOrigins;
                                dbClient.RedirectUris = client.RedirectUris;
                                dbClient.PostLogoutRedirectUris = client.PostLogoutRedirectUris;
                                dbClient.RequireConsent = client.RequireConsent;
                                dbClient.AllowRememberConsent = client.AllowRememberConsent;
                                dbClient.AllowOfflineAccess = client.AllowOfflineAccess;
                                dbClient.AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser;
                            }
                            else
                            {
                                //ADD
                                context.Clients.Add(client);
                            }

                            context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                            logger.LogError(ex, "Unabled to get ClientResources from DB.");

                            throw ex;
                        }
                    }
                }

                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool DeleteClient(string clientId)
        {
            bool result = false;

            var host = Program.host;
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                using (var context = serviceProvider.GetRequiredService<ConfigurationDbContext>())
                {
                    try
                    {
                        var client = context.Clients
                            .Include(c => c.AllowedGrantTypes)
                            .Include(c => c.ClientSecrets)
                            .Include(c => c.AllowedScopes)
                            .Include(c => c.AllowedCorsOrigins)
                            .Include(c => c.RedirectUris)
                            .Include(c => c.PostLogoutRedirectUris)
                            .FirstOrDefault(x => x.ClientId == clientId);

                        if (client != null)
                        {
                            context.Clients.Remove(client);
                            context.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "Unabled to get ClientResources from DB.");
                    }

                }
            }

            return result;
        }
    }
}
