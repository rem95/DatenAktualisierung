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
using DatenAktualisierung.Data;


namespace DatenAktualisierung.Controllers
{
    public class DatenAktualisierenController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MitarbeiterContext _contextMitarbeiter;
        private readonly KrankenquoteContext _contextSandbox;
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

        public DatenAktualisierenController(ILogger<HomeController> logger, MitarbeiterContext mitarbeiter, KrankenquoteContext contextSandbox)
        {

            _logger = logger;

            _contextMitarbeiter = mitarbeiter;

            _contextSandbox = contextSandbox;
        }
        public IActionResult login()
        {

            return View();

        }

        [Route("api/")]
        public async Task<IActionResult> schreibe_in_die_Datenbank()
        {
            System.Globalization.CultureInfo cultureinfo =
        new System.Globalization.CultureInfo("de-DE");



            DateTime newTime = new DateTime(2021, 5, 1);

            List<Anwesenheit_csv> list = Download_from_FTP_optimiert("Kranktage.csv");
            Stopwatch stopwatch = new Stopwatch();

            // sortiere nach Datum
            stopwatch.Start();

            var MinDate = (from d in list select d.Datum).Min();

            var CurrentDate = "20210601";
            Console.WriteLine(MinDate);

            var select = (from q in _contextSandbox.Anwesenheit_csv
                          where q.Datum.CompareTo(MinDate) > 0 || q.Datum.CompareTo(MinDate) == 0
                          select q).ToList();

            foreach (var test in select)
            {
                Console.WriteLine("referenzid" + test.Id);
            }

            _contextSandbox.Anwesenheit_csv.RemoveRange(select);

            await _contextSandbox.SaveChangesAsync();

            // loesche_bis_Datum(datum);

            Console.WriteLine("Elapsed Time is {0} s", stopwatch.ElapsedMilliseconds);



            await _contextSandbox.AddRangeAsync(list);
            await _contextSandbox.SaveChangesAsync();


            return Json("erfolgreich");
        }
        void loesche_bis_Datum(DateTime datum)
        {
            DateTime newTime = new DateTime(1980, 11, 1);
            DateTime newTime2 = new DateTime(1980, 11, 1);

            var data = (_contextSandbox.Anwesenheit_csv
                .Where(c => new DateTime(Int32.Parse(c.Datum.Substring(0, 4)), Int32.Parse(c.Datum.Substring(4, 2)), Int32.Parse(c.Datum.Substring(6, 2))) >= datum)
                .Select(c => c)).ToList();




            foreach (var check_data in data)
            {
                Console.WriteLine("Datum ist groesser_gleich  20210501" + check_data.Datum);
            }




        }
        // importiere aus lokaalen Verzeichnis
        //[Route("writePerson/")]
        //public async Task<IActionResult> schreibe_Personen_in_Db()
        //{
        //    var alleBerichte = from c in _contextMitarbeiter.Person select c;

        //    _contextMitarbeiter.Person.RemoveRange(alleBerichte);
        //    await _contextMitarbeiter.SaveChangesAsync();



        //    List<Person> list = Personal_from_FTP_optimiert("Q1.Personal.csv");

        //    await _contextMitarbeiter.AddRangeAsync(list);
        //    await _contextMitarbeiter.SaveChangesAsync();

        //    DateTime test = DateTime.Now;

        //    return Json("Datei wurden erfolgreich importiert am " + test.ToString());
        //}


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
            }

            DateTime test = DateTime.Now;

            return Json(test.ToString());
        }

        public void Trigger_Import_Personal()
        {

            RecurringJob.AddOrUpdate(() => schreibe_Personen_in_Db_FTP(), Cron.Daily());

        }

        [Route("db_api/{Stunden?}")]
        public async Task<IActionResult> hole_aus_DB_automatisch(int Stunden)
        {
            semaphore = new SemaphoreSlim(0, 3);
            Console.WriteLine("{0} tasks can enter the semaphore.",
                              semaphore.CurrentCount);
            Task[] tasks = new Task[5];

            // Create and start five numbered tasks.
            for (int i = 0; i <= 4; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    // Each task begins by requesting the semaphore.
                    Console.WriteLine("Task {0} begins and waits for the semaphore.",
                                      Task.CurrentId);

                    int semaphoreCount;
                    semaphore.Wait();
                    try
                    {
                        Interlocked.Add(ref padding, 100);

                        Console.WriteLine("Task {0} enters the semaphore.", Task.CurrentId);
                        schreibe_Personen_in_Db_FTP();
                        Thread.Sleep(1000 * 3600 * 24);// das ist ein tag
                        schreibe_in_die_Datenbank();
                        // The task just sleeps for 1+ seconds.
                        Thread.Sleep(1000 * 3600 * 24);
                    }
                    finally
                    {
                        semaphoreCount = semaphore.Release();
                    }
                    Console.WriteLine("Task {0} releases the semaphore; previous count: {1}.",
                                      Task.CurrentId, semaphoreCount);
                });
            }

            // Wait for half a second, to allow all the tasks to start and block.
            Thread.Sleep(50000);

            // Restore the semaphore count to its maximum value.
            Console.Write("Main thread calls Release(3) --> ");
            semaphore.Release(3);
            Console.WriteLine("{0} tasks can enter the semaphore.",
                              semaphore.CurrentCount);
            // Main thread waits for the tasks to complete.
            Task.WaitAll(tasks);

            Console.WriteLine("Main thread exits.");



            return Json("Semaphore wird gestartet");
        }
        public List<Csv_Model> Download_from_FTP(String filename)
        {

            string host = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FTPServer")["host"];
            string UserId = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FTPServer")["UserId"];
            string Password = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FTPServer")["Password"];

            List<String> data = new List<String>();

            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + host + "/" + filename);

            request.Method = WebRequestMethods.Ftp.DownloadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(UserId, Password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("iso-8859-1"), true);

            String datastream = reader.ReadToEnd();

            String[][] lines = datastream.Split('\n')
                                  .Select(p => p.Split('\t'))
                                  .ToArray();

            List<Csv_Model> list = new List<Csv_Model>();

            for (int i = 0; i < lines.Length - 1; i++)
            {

                if (i == 0)
                {
                }
                else
                {

                    Console.WriteLine("PersNr" + lines[i][0]);
                    string Name = lines[i][1].Split(',')[0];
                    string Vorname = lines[i][1].Split(',')[1];
                    DateTime Datum = new DateTime(Int32.Parse(lines[i][3].Substring(0, 4)), Int32.Parse(lines[i][3].Substring(4, 2)), Int32.Parse(lines[i][3].Substring(6, 2)));
                    Console.WriteLine("JAHR" + lines[i][3].Substring(0, 4));
                    Console.WriteLine("MONAT" + lines[i][3].Substring(4, 2));
                    Console.WriteLine("TAG" + lines[i][3].Substring(6, 2));
                    DateTime Stparse = DateTime.ParseExact(lines[i][3].Substring(0, 4) + "-" + lines[i][3].Substring(4, 2) + "-" + lines[i][3].Substring(6, 2), "yyyy-MM-dd", CultureInfo.CurrentCulture);

                    list.Add(new Csv_Model()
                    {
                        Personalnummer = lines[i][0],
                        Vorname = Vorname,
                        Name = Name,
                        Krank = lines[i][5].Equals("") ? Decimal.Parse("0.0") : Convert.ToDecimal((lines[i][5]), new CultureInfo("en-US")),
                        Fehlgrund = lines[i][4],
                        Anwesend = Decimal.Parse("2.0", new CultureInfo("en-US")),
                        Abteilung = lines[i][2],
                        Startdatum = Stparse,
                        Mandant = "4",
                        Startdatum_Monat = Stparse,
                        Startdatum_Tag = Stparse,
                        Startdatum_Woche = Stparse
                    });
                }
            }

            foreach (var mydatalist in list)
            {
                Console.WriteLine(" Bereich" + mydatalist.Abteilung);
            }

            Console.WriteLine($"Download Complete, status {response.StatusDescription}");

            reader.Close();
            response.Close();

            return list;
        }
        public List<Anwesenheit_csv> Download_from_FTP_optimiert(String filename)
        {
            List<Anwesenheit_csv> liste = new List<Anwesenheit_csv>();
            string host = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FTPServer")["host"];
            string UserId = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FTPServer")["UserId"];
            string Password = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FTPServer")["Password"];

            List<String> data = new List<String>();

            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + host + "/" + filename);

            request.Method = WebRequestMethods.Ftp.DownloadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(UserId, Password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();

            if (responseStream == null)
            {
                Console.WriteLine("Datei nicht gefunden");
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                HasHeaderRecord = true,
                Delimiter = "\t"
            };
            using (var reader = new StreamReader(responseStream, Encoding.GetEncoding("iso-8859-1")))

            using (var csv = new CsvReader(reader, config))
            {
                liste = csv.GetRecords<Anwesenheit_csv>().ToList();
                Console.WriteLine(liste.Count());
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


                select = test.Where(c => c.StartsWith("Q1.Person")).FirstOrDefault();

                Console.WriteLine(select);
            }
            catch (Exception)
            {
                throw;
            }


            return select;
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
                    Console.WriteLine("Die Datei Q1.Person.2021-05-27.csv ist leer");
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
                sendMail("Die Datei Q1.Person.2021-05-27.csv ist leer \n Bitte prüfen Sie ihr FTP Verzeichnis");
            }

            catch (Exception ex)
            {
                sendMail("Die Datei konnte nicht gelesen werden");
                Console.WriteLine("Fehler die Datei ist beschädigt oder nicht da");
            }

            sendMail("Die Datei Q1.Person.2021-05-27.csv wurde erfolgreich in die Datenbank geschrieben");

            return liste;
        }
        public List<Person> Personal_from_FTP_optimiert(String filename)
        {
            List<Person> liste = new List<Person>();


            String date = DateTime.Now.ToString("yyyy-mm-dd");



            try
            {
                var config = new CsvConfiguration(CultureInfo.CurrentUICulture)
                {

                    HeaderValidated = null,
                    MissingFieldFound = null,
                    HasHeaderRecord = true,
                    Delimiter = ",",
                    BadDataFound = null

                };
                // Quelle
                using (var reader = new StreamReader("C:\\Users\\r.myumyunova\\Documents\\Personen_CSV\\Q1.Person.2021-05-27.csv", Encoding.GetEncoding("iso-8859-1")))


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
            catch (Exception ex)
            {
                sendMail("Die Datei Personal.csv konnte nicht in die Datenbank importiert werden.");
                Console.WriteLine("Fehler die Datei ist beschädigt oder nicht da");
            }
            return liste;

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

    }



}



    

