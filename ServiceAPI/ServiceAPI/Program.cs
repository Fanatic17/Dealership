using Microsoft.AspNetCore.Hosting;
using ServiceAPI.Dal;
using System;
using System.Threading.Tasks;

namespace ServiceAPI
{
    class Program
    {
        static void Main(string[] args)
        {

            var host = new WebHostBuilder()
               .UseKestrel()
               .UseStartup<Startup>()
               .Build();

            Task restService = host.RunAsync();


            //using (var context = new GlobalDbContext())
            //{
            //    // create database
            //    context.Database.EnsureCreated();

             
            //}

            


            //System.Diagnostics.Process.Start("chrome.exe", "http://localhost/netcoreapp2.0/corsoing/");
            System.Diagnostics.Process.Start("cmd", "/C start http://localhost/netcoreapp2.0/corsoing/");
            restService.Wait();
        }
    }
}
