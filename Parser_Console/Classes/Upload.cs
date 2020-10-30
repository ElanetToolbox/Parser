using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Parser_Console.Classes
{
    public class Upload
    {
        public string ProjectFileId { get; set; }

        //E3
        public string f102E32019  { get; set; } //102
        public string f202E32019  { get; set; } //202
        public string f181E32019  { get; set; } //181
        public string f281E32019  { get; set; } //281
        public string f481E32019  { get; set; } //481
        public string f185E32019  { get; set; } //185
        public string f285E32019  { get; set; } //285
        public string f485E32019  { get; set; } //485
        public string Turnover2019 { get; set; } //500
        public string EBITDA2019  { get; set; } //524
        public KadUpload KadSuggestBiggest  { get; set; } //kyrios kad
        public KadUpload KadSuggestMain  { get; set; } //kad megalitera esoda

        //Taxis
        public string TaxCode { get; set; }
        public string LegalName { get; set; }
        public string LegalType { get; set; }
        public string FoundingDate { get; set; }
        public string TaxOffice { get; set; }
        public string CountryOfResidence { get; set; }
        public string CivicCompartment  { get; set; }
        public string PostCode { get; set; }
        public List<KadUpload> KadImplementationPlaces { get; set; }

        public string Log { get; set; }

        public string ToJsonString()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            var obj = JsonConvert.SerializeObject(this, Formatting.Indented, settings);
            string result = obj.ToString();
            return result;
        }

        public void UploadToCloud(string code)
        {
            var client = new RestClient("https://api.elanet.gr/wp-json/covid-app/v1/projects/data/" + code);
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", "__cfduid=d701eff1105ff2e9f0494fcc62073c6131601277950; LOhNClQmXjeGsv=eWZzKu2DNihQrV; xBowmpAyJ_=hJEAfOvB3G; wlDxodLWRmQ=%5Bqr%5DnMAdlb.");
            string json = ToJsonString();
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if(response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception();
            }
        }
    }

}
