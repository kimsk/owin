using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin;

namespace KatanaIntro
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    class Program
    {
        static void Main(string[] args)
        {
            string uri = "http://localhost:11112";
            using (WebApp.Start<Startup>(uri))
            {
                Console.WriteLine("Started!");
                Console.ReadKey();
                Console.WriteLine("Stopped!");
            }
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.Run(ctx => ctx.Response.WriteAsync("Hello World!"));
            app.UseHelloWorldComponent();

            app.Use(async (env, next) =>
            {
                foreach (var kv in env.Environment)
                {
                    Console.WriteLine("{0} : {1}", kv.Key, kv.Value);
                }

                await next();
            });

            app.Use(async (env, next) =>
            {
                Console.WriteLine("Requesting: " + env.Request.Path);

                await next();

                Console.WriteLine("Response: " + env.Response.StatusCode);
            });

            app.Use<HelloWorldComponent>();
            
        }
    }

    public static class AppBuilderExtensions
    {
        public static void UseHelloWorldComponent(this IAppBuilder app)
        {
            app.Use<HelloWorldComponent>();
        }
    }

    public class HelloWorldComponent
    {
        AppFunc _next;

        public HelloWorldComponent(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var response = environment["owin.ResponseBody"] as Stream;
            using (var writer = new StreamWriter(response))
            {
                await writer.WriteLineAsync("Hellooo!!");
            }

            await _next(environment);
        }
    }
}
