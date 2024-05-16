using AngleSharp;
using AngleSharp.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Wine_parser
{
    public class JsonHelper
    {

        public static async void JsonCreat(List<ParsingResult> result)
        {
            string json = JsonConvert.SerializeObject(result, Formatting.Indented);

            string path = Path.Combine("D:\\VisualStudio Projects\\Wine_parser", "wine.json");

            await File.WriteAllTextAsync(path, json);

            Console.WriteLine("JSON данные записаны в файл wine.json");

        }
    }
}
