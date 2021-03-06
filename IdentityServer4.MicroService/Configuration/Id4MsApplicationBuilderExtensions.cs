﻿using IdentityServer4.MicroService.AppSettings;
using IdentityServer4.MicroService.Configuration;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Tenant;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Id4MsApplicationBuilderExtensions
    {
        /// <summary>
        /// Creates a builder.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddIdentityServer4MicroServiceBuilder(this IServiceCollection services)
        {
            return new Id4MsServiceBuilder(services);
        }

        /// <summary>
        /// Creates a builder.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The config.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddIdentityServer4MicroService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var builder = services.AddIdentityServer4MicroServiceBuilder();

            var ConnectionSection = configuration.GetSection("ConnectionStrings");

            builder.Services.Configure<ConnectionStrings>(ConnectionSection);

            builder
                .AddCoreService()
                .AddAuthorization()
                .AddEmailService(configuration.GetSection("MessageSender:Email"))
                .AddSmsService(configuration.GetSection("MessageSender:Sms"));

            builder.Services.AddMemoryCache();

            return builder;
        }

        /// <summary>
        /// Configures EF implementation of TenantStore with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddTenantStore(
            this IId4MsServiceBuilder builder,
            Action<DbContextOptionsBuilder> storeOptionsAction = null)
        {
            builder.Services.AddDbContext<TenantDbContext>(storeOptionsAction);
            builder.Services.AddScoped<TenantDbContext>();

            return builder;
        }

        /// <summary>
        /// Configures EF implementation of IdentityStore with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <param name="identityOptions">The identity options action.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddIdentityStore(
            this IId4MsServiceBuilder builder,
            Action<DbContextOptionsBuilder> storeOptionsAction = null, Action<IdentityOptions> identityOptions = null)
        {
            builder.Services.AddDbContext<IdentityDbContext>(storeOptionsAction);
            builder.Services.AddScoped<IdentityDbContext>();

            builder.Services.AddIdentity<AppUser, AppRole>(identityOptions)
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

            return builder;
        }

        /// <summary>
        /// Configures SqlCache Service
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="connection">database connection string</param>
        /// <param name="schemaName">table schemaName</param>
        /// <param name="tableName">table name</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddSqlCacheStore(
           this IId4MsServiceBuilder builder,
           string connection,string schemaName= "dbo", string tableName= "AppCache")
        {
            builder.Services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = connection;
                options.SchemaName = schemaName;
                options.TableName = tableName;
            });

            return builder;
        }
    }
}
