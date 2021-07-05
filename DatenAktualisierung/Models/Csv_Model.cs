using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatenAktualisierung.Models
{
    public class Csv_Model
    {
        [Index(1)]
        public String Personalnummer { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Vorname { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Index(5)]
        public String Fehlgrund { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        [Index(2)]
        public String Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        public Nullable<decimal> Krank { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        [Index(6)]
        public Nullable<decimal> Anwesend { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        [Index(3)]
        public String Abteilung { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Index(4)]
        public DateTime Startdatum { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Startdatum_Tag { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Startdatum_Monat { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Startdatum_Woche { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Geburtsdatum { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Kundenname { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Bereich { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Betrieb { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Alter { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool steht_im_konflikt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Eintritt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Austritt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Datei_id { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Datei_datum { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        [Index(0)]
        public String Mandant { get; set; }
    }
}
