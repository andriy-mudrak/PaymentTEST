﻿using System.Collections.Generic;
using AutoMapper;
using BLL.Helpers;
using BLL.Helpers.Interfaces;
using BLL.Helpers.Mapping;
using BLL.Helpers.Mapping.Interfaces;
using BLL.Helpers.Queries;
using BLL.Helpers.Queries.Interfaces;
using BLL.Helpers.UserUpdating;
using BLL.Helpers.UserUpdating.Interfaces;
using BLL.Services;
using BLL.Services.Interfaces;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DAL.DBModels;
using Newtonsoft.Json;
using PaymentAPI.Middleware;
using Serilog;
using Stripe;

namespace PaymentAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAutoMapper(options => options.AddProfile<AutoMapperProfile>(), typeof(Startup));

            StripeConfiguration.ApiKey = Configuration["Stripe:SecretKey"];
            StripeConfiguration.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            services.AddDbContext<PaymentsDbContext>(options => options.UseSqlServer(Configuration.GetSection("ConnectionStrings:DefaultConnection").Value));

            services.AddTransient<IMappingProvider, MappingProvider>(); 
            services.AddTransient<IRetryHelper, RetryHelper>();
            services.AddTransient<IPaymentProvider, PaymentProvider>();
            services.AddTransient<IUserModifier, UserModifier>();
            services.AddTransient<ITransactionQueryCreator, TransactionQueryCreator>();
            services.AddTransient<IUserQueryCreator, UserQueryCreator>();

            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddScoped<MappingStripeSucceeded<Charge>>();
            services.AddScoped<MappingStripeRefund<Refund>>();
            services.AddScoped<MappingPaymentFailed<string>>();

            services.AddTransient<MappingResolver>(serviceProvider => key =>
            {
                switch (key)
                {
                    case PaymentServiceConstants.PaymentMappingType.StripeSucceeded:
                        return serviceProvider.GetService<MappingStripeSucceeded<Charge>>();
                    case PaymentServiceConstants.PaymentMappingType.StripeRefund:
                        return serviceProvider.GetService<MappingStripeRefund<Refund>>();
                    case PaymentServiceConstants.PaymentMappingType.Failed:
                        return serviceProvider.GetService<MappingPaymentFailed<string>>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

            services.AddScoped<PaymentAuthentication>();
            services.AddScoped<PaymentCapture>();
            services.AddScoped<PaymentCharge>();
            services.AddScoped<PaymentRefund>();

            services.AddTransient<ServiceResolver>(serviceProvider => key =>
            {
                switch (key)
                {
                    case PaymentServiceConstants.PaymentType.Auth:
                        return serviceProvider.GetService<PaymentAuthentication>();
                    case PaymentServiceConstants.PaymentType.Capture:
                        return serviceProvider.GetService<PaymentCapture>();
                    case PaymentServiceConstants.PaymentType.Charge:
                        return serviceProvider.GetService<PaymentCharge>();
                    case PaymentServiceConstants.PaymentType.Refund:
                        return serviceProvider.GetService<PaymentRefund>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            { 
                app.UseHsts();
            }
            app.UseSerilogRequestLogging();

            app.UseExceptionMiddleware();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
