﻿using System;
using System.Linq;
using System.Reflection;
using App.Metrics;
using App.Metrics.Reporting.InfluxDB;
using API.Infrastructure.Filter;
using API.Infrastructure.Pipeline;
using AutoMapper;
using CorrelationId;
using DataModel;
using DataModel.Models;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Dashboard;
using IdentityServer4.AccessTokenValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StructureMap;
using Swashbuckle.AspNetCore.Swagger;
using System.Text;
using API.Infrastructure.Identity;
using API.Infrastructure.MessageQueue;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Hangfire.PostgreSql;

namespace API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, ILogger<Startup> logger)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(path: $"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            _logger = logger;
            _env = env;
        }

        public IConfigurationRoot Configuration { get; }
        private readonly ILogger<Startup> _logger;
        private readonly IHostingEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));

            services.AddMediatR(typeof(Startup));

            services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString(ConnectionStringKeys.App)));

            services.AddHangfire(x =>
                x.UsePostgreSqlStorage(Configuration.GetConnectionString(ConnectionStringKeys.Hangfire)));

            services.AddCorrelationId();

            services.AddOptions();

            var metricsConfigSection = Configuration.GetSection(nameof(MetricsOptions));
            var influxOptions = new MetricsReportingInfluxDbOptions();
            Configuration.GetSection(nameof(MetricsReportingInfluxDbOptions)).Bind(influxOptions);

            var metrics = AppMetrics.CreateDefaultBuilder()
                .Configuration.Configure(metricsConfigSection.AsEnumerable())
                .Report.ToInfluxDb(influxOptions)
                .Build();

            services.AddMetrics(metrics);
            services.AddMetricsTrackingMiddleware();
            services.AddMetricsEndpoints();
            services.AddMetricsReportingHostedService();

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Knowledge API",
                    Description = "Knowledge API v1",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Mads Engel Lundt",
                        Email = "contact@elcc.dk",
                        Url = "https://elcc.dk"
                    }
                });
                c.CustomSchemaIds(x => x.FullName);
            });

            // Pipeline
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MetricsProcessor<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

            services.AddMvc(opt =>
                {
                    opt.Filters.Add(typeof(ExceptionFilter));
                    opt.Filters.Add(typeof(LocaleFilterAttribute));
                })
                .AddMetrics()
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining<Startup>(); });

            // Identity
            var identityOptionsSection = Configuration.GetSection(nameof(Infrastructure.Identity.IdentityOptions));
            services.Configure<Infrastructure.Identity.IdentityOptions>(identityOptionsSection);

            // configure jwt authentication
            var identityOptions = identityOptionsSection.Get<Infrastructure.Identity.IdentityOptions>();
            var key = Encoding.ASCII.GetBytes(identityOptions.ApiSecret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                //.AddIdentityServerAuthentication(options =>
                //{
                //    options.Authority = identityOptions.Authority;
                //    options.ApiName = identityOptions.ApiName;
                //    options.ApiSecret = identityOptions.ApiSecret;
                //    options.RequireHttpsMetadata = _env.IsProduction();
                //    options.EnableCaching = true;
                //    options.CacheDuration = TimeSpan.FromMinutes(10);
                //})
                .AddJwtBearer("Bearer", options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };

                    options.Authority = identityOptions.Authority;
                    options.RequireHttpsMetadata = _env.IsProduction();

                    options.Audience = identityOptions.Audience;
                });

            IContainer container = new Container();
            container.Configure(config => config.Populate(services));

            var mediator = container.GetInstance<IMediator>();
            GlobalConfiguration.Configuration.UseMediator(mediator);

            metrics.ReportRunner.RunAllAsync();


            // Check for missing dependencies
            var controllers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
                .ToList();

            var sp = services.BuildServiceProvider();
            foreach (var controllerType in controllers)
            {
                _logger.LogInformation($"Found {controllerType.Name}");
                try
                {
                    sp.GetService(controllerType);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex,
                        $"Cannot create instance of controller {controllerType.FullName}, it is missing some services");
                }
            }

            services.AddLogging(builder => builder
                .AddConfiguration(Configuration)
                .AddConsole()
                .AddDebug()
                .AddEventSourceLogger()
                .AddSentry());

            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env)
        {
            app.UseCorrelationId(new CorrelationIdOptions
            {
                UseGuidForCorrelationId = true
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }

            app.UseMetricsAllEndpoints();
            app.UseMetricsAllMiddleware();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Knowledge API v1");
            });

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                SchedulePollingInterval = TimeSpan.FromSeconds(30),
                ServerCheckInterval = TimeSpan.FromMinutes(1),
                ServerName = $"{Environment.MachineName}.{Guid.NewGuid()}",
                WorkerCount = Environment.ProcessorCount * 5
            });

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                IsReadOnlyFunc = (DashboardContext context) => true,
                Authorization = new[] { new MyAuthorizationFilter() }
            });
            app.UseAuthentication();

            app.UseMvc();
        }

        public class MyAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                var httpContext = context.GetHttpContext();

                // Allow all authenticated users to see the Dashboard (potentially dangerous).
                //return httpContext.User.Identity.IsAuthenticated;
                return true;
            }
        }
    }
}
