using CsvHelper;
using CsvHelper.Configuration;
using DatenAktualisierung.Models;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using DatenAktualisierung.Controllers;
using System.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DatenAktualisierung
{
    public class HangfireErinnerung : IHangfireErinnerung
    {
        private readonly DatenAktualisierenController _controller;
        public HangfireErinnerung(DatenAktualisierenController controller)
        { _controller = controller; }
        //notwendig
        public void methodeAufrufenAsync()
        {
            _controller.schreibe_Personen_in_Db_FTP();
        }

    }


}

