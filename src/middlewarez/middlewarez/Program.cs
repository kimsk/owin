using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;

namespace middlewarez
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

    public class Middleware4
    {
        private readonly AppFunc _next;

        public Middleware4(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            Console.WriteLine("Begin #4");
            await _next.Invoke(environment);
            Console.WriteLine("End #4");
        }
    }

    public class Middleware5 : OwinMiddleware
    {
        public Middleware5(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            Console.WriteLine("Begin #5" + context.Request.ContentType);
            await Next.Invoke(context);
            Console.WriteLine("End #5");
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Func<IOwinContext, Func<Task>, Task>
            app.Use(async (env, next) =>
            {
                Console.WriteLine("Begin # 1");
                Console.WriteLine("Requesting: #1 " + env.Request.Path);
                
                await next();
                
                Console.WriteLine("End #1");
            });

            // Func<AppFunc, AppFunc>
            app.Use(new Func<AppFunc, AppFunc>(next => (async env =>
            {
                Console.WriteLine("Requesting: #2 " + env["owin.RequestPath"]);
                Console.WriteLine("Begin # 2");

                var response = env["owin.ResponseBody"] as Stream;
                using (var writer = new StreamWriter(response))
                {
                    await writer.WriteLineAsync("Hello from #2 !!");
                }

                await next.Invoke(env);

                Console.WriteLine("End #2");
            })));

            // delegate
            app.Use(new Func<AppFunc, AppFunc>(next => env => Invoke(next, env)));

            app.Use<Middleware4>();

            app.Use(typeof (Middleware5));
        }

        private async Task Invoke(AppFunc next, IDictionary<string, object> env)
        {
            Console.WriteLine("Begin #3");
            await next.Invoke(env);
            Console.WriteLine("End #3");
        }
    }
}
