using CsvHelper.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatenAktualisierung.Models
{
    public class Csv_Model_Map: ClassMap<Models.Csv_Model_Map>
    {
        public String Personalnummer { get; set; }

        public String Vorname { get; set; }

        public String Fehlgrund { get; set; }

        public String Name { get; set; }

        public Nullable<decimal> Krank { get; set; }

        public Nullable<decimal> Anwesend { get; set; }

        public String Abteilung { get; set; }


        public DateTime Startdatum { get; set; }

        public DateTime Startdatum_Tag { get; set; }

        public DateTime Startdatum_Monat { get; set; }

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


        public int Mandant { get; set; }
    }
}
