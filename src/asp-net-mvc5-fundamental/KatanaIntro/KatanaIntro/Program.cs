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

        public Task Invoke(IDictionary<string, object> environment)
        {
            foreach (var kv in environment)
            {
                Console.WriteLine("{0} : {1}", kv.Key, kv.Value);
            }

            var response = environment["owin.ResponseBody"] as Stream;
            using (var writer = new StreamWriter(response))
            {
                return writer.WriteLineAsync("Hellooo!!");
            }

            //await _next(environment);
        }
    }
}
