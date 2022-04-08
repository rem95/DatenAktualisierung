using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using DatenAktualisierung.Models;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;
using Person = DatenAktualisierung.Models.Person;



namespace DatenAktualisierung.Controllers
{
    public class DatenAktualisierenController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MitarbeiterContext _contextMitarbeiter;
        private static SemaphoreSlim semaphore;

        // A padding interval to make the output more orderly.
        private static int padding;
        public IActionResult Index()
        {
            return View();
        }



        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public DatenAktualisierenController(ILogger<HomeController> logger, MitarbeiterContext mitarbeiter)
        {

            _logger = logger;

            _contextMitarbeiter = mitarbeiter;

        }
        public IActionResult login()
        {

            return View();

        }


        //notwendig
       

        // importiere aus FTP Verzeichnis
        //notwendig
        [Route("writePersonFTP/")]
        public async Task<IActionResult> schreibe_Personen_in_Db_FTP()
        {
            List<Person> list = Download_Person_from_FTP_optimiert(getFiles());

            if (list.Count() != 0)
            {
                Console.WriteLine("Die liste ist nicht leer");

                var persons = from c in _contextMitarbeiter.Person select c;

                _contextMitarbeiter.Person.RemoveRange(persons);



                await _contextMitarbeiter.SaveChangesAsync();
                await _contextMitarbeiter.AddRangeAsync(list);
                await _contextMitarbeiter.SaveChangesAsync();

                sendMail("Die Datei Q1.Person.csv wurde erfolgreich in die Datenbank geschrieben.");

            }
            else {
                sendMail("Die Datei Q1.Person.csv wurde nicht erfolgreich in die Datenbank geschrieben.");

            }
            var test = "hat geklappt";

            return Json(test.ToString());
        }



        public void Trigger_Import_Personal()
        {            
            RecurringJob.AddOrUpdate(() => schreibe_Personen_in_Db_FTP(), Cron.Daily());
        }
        // notwendig
        public List<Person> Download_Person_from_FTP_optimiert(String filename)
        {
            Console.WriteLine("Starte FTP");
            List<Person> liste = new List<Person>();
            string host = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FTPServer")["host"];
            string UserId = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FTPServer")["UserId"];
            string Password = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FTPServer")["Password"];

            List<String> data = new List<String>();

            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + host + "/" + filename);

            request.Method = WebRequestMethods.Ftp.GetFileSize.ToString();






            try
            {
                request.Method = WebRequestMethods.Ftp.DownloadFile;


                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential(UserId, Password);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();


                Stream responseStream = response.GetResponseStream();



                if (responseStream == null)
                {
                    Console.WriteLine("Die Datei Q1.Person.csv ist leer");
                }
                else
                {
                    Console.WriteLine(responseStream.ToString());
                }
                CultureInfo test = new CultureInfo("de-DE");

                var config = new CsvConfiguration(test)
                {

                    HeaderValidated = null,
                    MissingFieldFound = null,
                    HasHeaderRecord = true,
                    Delimiter = ",",
                    BadDataFound = null

                };

                using (var reader = new StreamReader(responseStream, Encoding.GetEncoding("iso-8859-1")))


                using (var csv = new CsvReader(reader, config))
                {


                    liste = csv.GetRecords<Person>().ToList();
                    Console.WriteLine(liste.Count());
                }

                // Zeilen verändert
                foreach (var element in liste)
                {
                    if (element.Vorname.Equals(""))
                    {
                        element.Name = element.Nachname;
                    }
                    else
                    {

                        element.Name = element.Nachname + ", " + element.Vorname;
                    }


                }


            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;

                Console.WriteLine("Die Datei ");
                sendMail("Die Datei Q1.Person.csv ist leer \n Bitte prüfen Sie ihr FTP Verzeichnis");
            }

            catch (Exception ex)
            {
                sendMail("Die Datei konnte nicht gelesen werden");
                Console.WriteLine("Fehler die Datei ist beschädigt oder nicht da");
            }


            return liste;
        }


        [Route("getFiles/")]
        // Datei Q1.Person suchen
        // notwendig
        public String getFiles()
        {
            String select = "";
            List<String> test = new List<String>();
            List<Person> liste = new List<Person>();
            string host = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FTPServer")["host"];
            string UserId = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FTPServer")["UserId"];
            string Password = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FTPServer")["Password"];

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + host + "/");
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                request.Credentials = new NetworkCredential(UserId, Password);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string names = reader.ReadToEnd();

                reader.Close();
                response.Close();

                test = names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();


                //select = test.Where(c => c.StartsWith("Q1.Person")).FirstOrDefault();
                select = test.Where(c => c.Equals("Q1Person.csv")).FirstOrDefault();


                Console.WriteLine(select);
            }
            catch (Exception)
            {
                throw;
            }


            return select;
        }



        public String GetToken()
        {
            string name = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Credentials")["name"];
            string password = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Credentials")["password"];

            var client = new RestClient("https://login.microsoftonline.com/9bdd1af9-deee-4e3e-a148-29d4ce6dd529/oauth2/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=password\r\n&username= gennadiy.shevtsov@teamprojekt-outsourcing.de\r\n&password=20Sovy03\r\n&client_id=43d851b3-c3ea-4024-8e7b-e1e63ffd096a\r\n&client_secret=J7T1B~bWWE9Rd2DPN_Gtwm_J.bsa9Oq7.1\r\n&resource=https://analysis.windows.net/powerbi/api",
            ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            dynamic data = JObject.Parse(response.Content);

            Console.WriteLine(data.access_token);

            return data.access_token;
        }


        //notwendig
        [Route("SendMail/{message?}")]
        public async Task<IActionResult> sendMail(String message)
        {
            string name = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Credentials")["name"];
            string password = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Credentials")["password"];


            try
            {

                var smtpClient = new SmtpClient("sslout.df.eu")
                {
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Credentials")["name"],
                    new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Credentials")["password"]),

                    Port = 587,
                    EnableSsl = true,
                };

                smtpClient.Send(name, "remzie.myumyunova@teamprojekt-outsourcing.de", "Personal Import Fehlermeldung", message);

            }
            catch (Exception ex)
            {

                Console.WriteLine("Uebetragung Fehlgeschlagen");

            }




            Console.WriteLine("erfolgt");

            return Json("erfolg");
        }

        [Route("db_slim/{Stunden?}")]
        public async Task<IActionResult> semaphore_Slim_db()
        {
            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

            await semaphoreSlim.WaitAsync();

            DateTime date1 = DateTime.Now;

            Console.WriteLine("hour" + date1.Hour);

            Console.WriteLine("hour" + date1.Minute);

            if (date1.Hour == 4)
            {
                Console.WriteLine("hour" + date1.Hour);
            }

            try
            {
                while (true)
                {
                    Thread.Sleep(1000);

                    date1 = DateTime.Now;

                    Console.WriteLine("test");
                    Console.WriteLine(date1.Hour);
                    Console.WriteLine(date1.Minute);
                    Console.WriteLine(date1.Minute == 49);
                    Console.WriteLine("Sekunden" + date1.Second);

                    if (date1.Hour == 00 && date1.Minute == 04 && date1.Second == 1)
                    {

                        Console.WriteLine("Zeitstempel wird ausgelöst");

                        await schreibe_Personen_in_Db_FTP();
                    }

                }

            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                semaphoreSlim.Release();
            }


            return Json("erfolg");
        }

        public void Sync()
        {
            RecurringJob.AddOrUpdate(() => schreibe_Personen_in_Db_FTP(), Cron.Daily());
        }
    }



}



    

