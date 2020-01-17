using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.Glossary.Models;
using NCI.OCPL.Api.Glossary.Services;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Defines the configuration for the Best Bets API
    /// </summary>
    public class Startup : NciStartupBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:NCI.OCPL.Api.Glossary.Startup"/> class.
        /// </summary>
        /// <param name="env">Env.</param>
        public Startup(IHostingEnvironment env)
            : base(env) { }


        /*****************************
         * ConfigureServices methods *
         *****************************/

        /// <summary>
        /// Adds the configuration mappings.
        /// </summary>
        /// <param name="services">Services.</param>
        protected override void AddAdditionalConfigurationMappings(IServiceCollection services)
        {
            // services.Configure<CGBBIndexOptions>(Configuration.GetSection("CGBestBetsIndex"));

        }

        /// <summary>
        /// Adds in the application specific services
        /// </summary>
        /// <param name="services">Services.</param>
        protected override void AddAppServices(IServiceCollection services)
        {
            //Add our Term Query Service
            services.AddTransient<ITermQueryService, ESTermQueryService>();
            services.AddTransient<ITermsQueryService, ESTermsQueryService>();
            services.AddTransient<IAutosuggestQueryService, ESAutosuggestQueryService>();
            services.Configure<GlossaryAPIOptions>(Configuration.GetSection("GlossaryAPI"));
        }

        /*****************************
         *     Configure methods     *
         *****************************/

        /// <summary>
        /// Configure the specified app and env.
        /// </summary>
        /// <returns>The configure.</returns>
        /// <param name="app">App.</param>
        /// <param name="env">Env.</param>
        /// <param name="loggerFactory">Logger.</param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        protected override void ConfigureAppSpecific(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            return;
        }

    }
}