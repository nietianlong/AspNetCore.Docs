using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ResponseCachingMiddleware
{
    public class Startup
    {
        #region snippet1
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCaching();
            services.AddRazorPages();

            // When using CORS, uncomment CORS calls.
            // UseCors must be called before UseResponseCaching
            // in Configure.
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("myAllowSpecificOrigins",
            //                      builder =>
            //                      {
            //                          builder.WithOrigins("http://example.com",
            //                                              "http://www.contoso.com");
            //                      });
            //});
        }
        #endregion

        #region snippet2
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            // app.UseCors("myAllowSpecificOrigins");

            app.UseResponseCaching();

            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl = 
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(10)
                    };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = 
                    new string[] { "Accept-Encoding" };

                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
        #endregion
    }
}
