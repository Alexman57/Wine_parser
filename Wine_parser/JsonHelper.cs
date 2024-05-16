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

            string path = Path.Combine("D:\\VisualStudio Projects\\Wine_parser", "wine.json");

            File.WriteAllText(path, jsonData);

            Console.WriteLine("JSON данные записаны в файл wine.json");
        }
    }
}
