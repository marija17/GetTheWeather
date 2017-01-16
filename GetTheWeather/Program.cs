using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using RestSharp;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace GetTheWeather
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the rest client and make a request for the weather based on zip code.
            var client = new RestClient("http://api.openweathermap.org/");
            var request = new RestRequest("data/2.5/weather", Method.GET);
            request.AddParameter("zip", "60607");
            request.AddParameter("APPID", "6f9420f0c2be13ab911010437ca9d8c7");
            IRestResponse response = client.Execute(request);

            // Get the temperature: String Parsing
            double temp = GetTempFromResponseByStringParsing(response);
            Console.WriteLine("String Parse Temp: " + ConvertKelvinsToFahrenheit(temp) + " F");

            // Get the temperature: JSON Deserializer
            temp = GetTempFromResponseByJSONDeserializer(response);
            Console.WriteLine("JSON Deserializer Temp: " + ConvertKelvinsToFahrenheit(temp) + " F");

            // Get the temperature: Regular Expressions
            temp = GetTempFromResponseByRegularExpressions(response);
            Console.WriteLine("Regular Expressions Temp: " + ConvertKelvinsToFahrenheit(temp) + " F");

            Console.ReadKey();
        }

        /// <summary>
        /// Get the temperature by parsing the response as string.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static double GetTempFromResponseByStringParsing(IRestResponse response)
        {
            string responseString = response.Content.ToString();
            string stringTemp = "\"temp\":";
            int stringStart = responseString.IndexOf("\"temp\":", 0) + stringTemp.Length;
            int End = responseString.IndexOf(",\"pressure\"", stringStart);
            string temp = responseString.Substring(stringStart, End - stringStart);
            return Convert.ToDouble(temp);
        }

        /// <summary>
        /// Get the temperature using a JSON parser.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static double GetTempFromResponseByJSONDeserializer(IRestResponse response)
        {
            string responseString = response.Content.ToString();

            var jsonObject = (JObject)JsonConvert.DeserializeObject(responseString);
            string temp = (string)jsonObject["main"]["temp"];

            return Convert.ToDouble(temp);
        }

        /// <summary>
        /// Get the temperature using regular expressions.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static double GetTempFromResponseByRegularExpressions(IRestResponse response)
        {
            string responseString = response.Content.ToString();

            Regex tempFinder = new Regex("\\{\\s*\"temp\":\\s*(?<Temperature>.*?)\\s*(,)");
            var match = tempFinder.Match(responseString);
            var value = match.Groups["Temperature"].Captures[0];

            return Convert.ToDouble(value.ToString());
        }

        /// <summary>
        /// Given a temperature in Kelvins, convert it to Fahrenheit.
        /// </summary>
        /// <param name="kelvins"></param>
        /// <returns></returns>
        private static double ConvertKelvinsToFahrenheit(double kelvins)
        {
            return kelvins * (9.0 / 5.0) - 459.67;
        }
    }
    
}
