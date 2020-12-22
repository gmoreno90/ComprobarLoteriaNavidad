using ComprobarLoteriaNavidad.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ComprobarLoteriaNavidad
{
    class Program
    {
        public static IConfigurationRoot configuration;
        public static List<string> listNumbersInicial;
        public static List<string> listNumbersPremiados;
        static int Main(string[] args)
        {
            try
            {
                // Set up configuration sources.
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                    .AddJsonFile("appsettings.json", optional: true);
                
                configuration = builder.Build();

                MainAsync(args).Wait();
                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }


        }

        static async Task MainAsync(string[] args)
        {
            listNumbersInicial = configuration.GetSection("Numeros").Value.Split(',').ToList();
            listNumbersPremiados = new List<string>();
            string strResult = "";
            int milliseconds = Convert.ToInt32(configuration.GetSection("DelayCheck").Value);
            Timer t = new Timer(checkNumbers, null, 0, milliseconds);
            //checkNumbers();
            while (strResult != "q")
            {
                strResult = Console.ReadLine();
            }
        }

        //private static void ConfigureServices(IServiceCollection serviceCollection)
        //{
        //    // Add logging


        //    // Build configuration
        //    configuration = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
        //        .AddJsonFile("appsettings.json", false)
        //        .Build();

        //    // Add access to generic IConfigurationRoot
        //    serviceCollection.AddSingleton<IConfigurationRoot>(configuration);

        //    // Add app
        //    //serviceCollection.AddTransient<App>();
        //}

        private static void checkNumbers(Object o)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n\n\n---------------- CHECKING NUMBERS ----------------- " + DateTime.Now);
            string urlGet = configuration.GetSection("UrlCheck").Value;

            foreach (var numberLoteria in listNumbersInicial.ToList())
            {
                WebClient myWebClient = new WebClient();
                string result = myWebClient.DownloadString(string.Format(urlGet, Convert.ToInt32(numberLoteria.Trim())));
                result = result.Replace("busqueda=", "");
                var resultado = JsonConvert.DeserializeObject<ResultApi>(result);
                //var resultado = new ResultApi() { premio = 20 };
                if (resultado.premio != 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    listNumbersPremiados.Add(numberLoteria + " \t\t " + resultado.premio);
                    listNumbersInicial.Remove(numberLoteria);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine(numberLoteria + " \t\t " + resultado.premio);
            }
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("---------- RESUMEN DE PREMIOS ----------");
            foreach (var premiado in listNumbersPremiados.ToList())
            {
                Console.WriteLine(premiado);
            }
            Console.WriteLine("---------- FIN RESUMEN DE PREMIOS ----------");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
