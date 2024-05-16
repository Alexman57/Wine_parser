using AngleSharp;
using AngleSharp.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Wine_parser
{
    public class JsonHelper
    {
        public static async void JsonCreat(ParsingResult result)
        {

            //Regex regex = new Regex(@"\d+");

            // Match match = regex.Match(Arcicul);
            // string cleaned_price = Price.Replace(" ₽", "");
            // string price1 = cleaned_price.Replace(" ", "");

            // string cleaned_old_price = OldPrice.Replace(" ₽", "");
            // string price2 = cleaned_old_price.Replace(" ", "");

            //  CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

            // var Rating1 = double.TryParse(Rating, NumberStyles.Any, culture, out double number);


            // ParsingResult result = new;

            //var result = ParsingResult;

           // Console.WriteLine(ParsingResult.);

            var data = new
            {
                name = result.ParsedName,
                price = result.ParsedPrice,
                oldPrice = result.ParsedOldPrice,
                rating = result.ParsedRating,
                volume = result.ParsedVolume,
                arcicul = result.ParsedArticul,
                region = result.ParsedRegion,
                url = result.ParsedUrl,
                pictures = result.ParsedPictures,
            };

            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);

            File.WriteAllText("wine.json", jsonData);

            Console.WriteLine("JSON данные записаны в файл person.json");
        }
    }
}
