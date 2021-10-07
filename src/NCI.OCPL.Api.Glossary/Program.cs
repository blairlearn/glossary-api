using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using NCI.OCPL.Api.Common;


namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Defines the start up program
    /// </summary>
    public class Program : NciApiProgramBase
    {
        /// <summary>
        /// The main entry point for running the API.
        /// </summary>
        public static void Main(string[] args)
        {
            CreateHostBuilder<Startup>(args).Build().Run();
        }
    }
}
