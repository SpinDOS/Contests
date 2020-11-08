// using System.Text.RegularExpressions;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
//
// namespace P.ValidatePassport
// {
//     public class Program
//     {
//         public static void Main(string[] args)
//         {
//             CreateHostBuilder(args).Build().Run();
//         }
//
//         public static IHostBuilder CreateHostBuilder(string[] args)
//         {
//             return Host.CreateDefaultBuilder(args)
//                 .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>().UseUrls("http://0.0.0.0:7777"); });
//         }
//     }
//
//     public class Startup
//     {
//         // This method gets called by the runtime. Use this method to add services to the container.
//         // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
//         public void ConfigureServices(IServiceCollection services)
//         {
//         }
//
//         // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//         public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//         {
//             if (env.IsDevelopment())
//             {
//                 app.UseDeveloperExceptionPage();
//             }
//
//             app.UseRouting();
//             
//             app.UseEndpoints(endpoints =>
//             {
//                 endpoints.MapGet("/ping", async context => 
//                     { });
//                 endpoints.MapGet("/shutdown", async context => 
//                     app.ApplicationServices.GetService<IHostApplicationLifetime>().StopApplication());
//                 endpoints.MapGet("/validatePassportCode", async context => 
//                     await new PassportValidator().ValidatePassport(context));
//             });
//         }
//     }
//
//     internal sealed class PassportValidator
//     {
//         public Task ValidatePassport(HttpContext context)
//         {
//             if (!context.Request.Path.Value.StartsWith("/validatePassportCode"))
//             {
//                 context.Response.StatusCode = 404;
//                 return Task.CompletedTask;
//             }
//             
//             if (!context.Request.Query.TryGetValue("passport_code", out var queryVal) || queryVal.Count != 1)
//             {
//                 context.Response.StatusCode = 400;
//                 return Task.CompletedTask;
//             }
//
//             const string badPassportCode = "{\"status\": false}";
//             const string goodPassportCode = "{{\"normalized\": \"{0}-{1}\", \"status\": true}}";
//
//             var passportCode = ParsePassportCode(queryVal[0]);
//             return passportCode == null
//                 ? context.Response.WriteAsync(badPassportCode)
//                 : context.Response.WriteAsync(string.Format(goodPassportCode, passportCode.Value.series, passportCode.Value.number));
//         }
//
//         private (string series, string number)? ParsePassportCode(string query)
//         {
//             if (Regex.IsMatch(query, @"^\d{10}$"))
//                 return (query.Substring(0, 4), query.Substring(4, 6));
//             if (Regex.IsMatch(query, @"^\d{4} \d{6}$"))
//                 return (query.Substring(0, 4), query.Substring(5, 6));
//             if (Regex.IsMatch(query, @"^\(\d{4}\) \d{6}$"))
//                 return (query.Substring(1, 4), query.Substring(7, 6));
//             if (Regex.IsMatch(query, @"^\(\d{4}\)\d{6}$"))
//                 return (query.Substring(1, 4), query.Substring(6, 6));
//             if (Regex.IsMatch(query, @"^\d{4}-\d{6}$"))
//                 return (query.Substring(0, 4), query.Substring(5, 6));
//             if (Regex.IsMatch(query, @"^PC-\d{4}-\d{6}$"))
//                 return (query.Substring(3, 4), query.Substring(8, 6));
//             return null;
//         }
//     }
// }

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace TaskC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>().UseUrls("http://0.0.0.0:7777"); });
        }
    }

    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/ping", async context => 
                    { });
                endpoints.MapGet("/shutdown", async context => 
                    app.ApplicationServices.GetService<IHostApplicationLifetime>().StopApplication());
                endpoints.MapGet("/validatePassword",  context => 
                    ValidatePassword(context));
            });
        }

        private static Task ValidatePassword(HttpContext context)
        {
            if (!context.Request.Query.ContainsKey("password"))
            {
                context.Response.StatusCode = 400;
                return Task.CompletedTask;
            }

            bool status = false;
            try
            {
                var password = context.Request.Query["password"];
                status = password == "Pass12345678901=";
            }
            catch
            {
                
            }

            return context.Response.WriteAsync($"\"status\":{status.ToString().ToLower()}");
        }
    }
    
    
    // public class Program
    // {
    //
    //     public static void Main(string[] args) =>
    //         Host.CreateDefaultBuilder(args)
    //             .ConfigureWebHostDefaults(webBuilder =>
    //              {
    //                  webBuilder
    //                     .UseKestrel()
    //                     .Configure(app =>
    //                      {
    //                          app.UseRouting();
    //                          //на минималках ;)
    //                          app.UseEndpoints(route =>
    //                          {
    //                              route.MapGet("/validatePassword", context => ValidatePassword(context));
    //                              route.MapGet("/ping",
    //                                           context => context.Response.CompleteAsync());
    //                              route.MapGet("/shutdown",
    //                                           context =>
    //                                           {
    //                                               app.ApplicationServices.GetService<IHostApplicationLifetime>()
    //                                                  .StopApplication();
    //                                               return context.Response.CompleteAsync();
    //                                           });
    //                          });
    //                      })
    //                     .UseUrls("http://localhost:7777");
    //              })
    //             .Build().Run();
    // }
}