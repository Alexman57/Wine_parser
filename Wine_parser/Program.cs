using System;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Wine_parser;
using static System.Net.Mime.MediaTypeNames;



class MyClass
{

    static async Task Main(string[] args)
    {

        var parUrl = "https://simplewine.ru/catalog/product/bruni_prosecco_brut_2021_075_138416/";

        var test = new ParsingResult { ParsedUrl = parUrl };

        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);

        var document = await context.OpenAsync(parUrl);

        CancellationTokenSource cts = new CancellationTokenSource();

        Console.WriteLine("Нажмите любую кнопку для остановки");
        while (!Console.KeyAvailable)
        {
            Task<ParsingResult> task1 = Task.Run(async () => await GetPicture(1, cts.Token, document));
            Task<ParsingResult> task2 = Task.Run(async () => await GetData(2, cts.Token, document));
            Task<ParsingResult> task3 = Task.Run(async () => await GetPrice(3, cts.Token, document));

            ParsingResult result1 = await task1;
            ParsingResult result2 = await task2;
            ParsingResult result3 = await task3;

            ParsingResult combinedResult = new ParsingResult
            {
                ParsedName = result2.ParsedName,
                ParsedPrice = result3.ParsedPrice,
                ParsedOldPrice = result3.ParsedOldPrice,
                ParsedRating = result2.ParsedRating,
                ParsedVolume = result2.ParsedVolume,
                ParsedArticul = result2.ParsedArticul,
                ParsedRegion = result2.ParsedRegion,
                ParsedUrl = parUrl,
                ParsedPictures = result1.ParsedPictures,
            };

            JsonHelper.JsonCreat(combinedResult);
            await Task.Delay(30000);
        }

        cts.Cancel();

        Console.WriteLine("Парсинг остановлен");

    }


    static async Task<ParsingResult> GetPicture(int taskId, CancellationToken cancellationToken, IDocument document)
    {

        var pictures = document.QuerySelectorAll(".product-slider__slide-picture");

        var cleanLinks = pictures.Select(picture =>
        {
            var img = picture.QuerySelector("img");
            var dataSrcset = img?.GetAttribute("data-srcset");
            if (string.IsNullOrEmpty(dataSrcset))
            {
                return null;
            }

            var parts = dataSrcset.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var firstPart = parts[0].Trim();
            var spaceIndex = firstPart.IndexOf(' ');
            var firstLink = spaceIndex > 0 ? firstPart.Substring(0, spaceIndex) : firstPart;

            return firstLink.Replace("@x226", "");
        }).Where(link => !string.IsNullOrEmpty(link)).ToArray();

        return new ParsingResult
        {
            ParsedPictures = cleanLinks,
        };
    }


    static async Task<ParsingResult> GetData(int taskId, CancellationToken cancellationToken, IDocument document)
    {

        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

        var Name = document.QuerySelector("div.product-page__head");
        var location = document.QuerySelector("button.location__current.dropdown__toggler").TextContent.Trim();
        var _rating = document.QuerySelector("p.rating-stars__value").TextContent.Trim();
        var volum = document.QuerySelectorAll("dd.product-brief__value");

        var name = Name.QuerySelector("h1").TextContent.Trim();
        var arcicul = Name.QuerySelector("span.product-page__article.js-copy-article").TextContent.Trim();

        var volum1 = volum[5].TextContent.Trim();

        var rating_bool = double.TryParse(_rating, NumberStyles.Any, culture, out double rating);
        Regex regex = new Regex(@"\d+");

        Match match = regex.Match(arcicul);
        
        return new ParsingResult
        {
            ParsedName = name,
            ParsedRating = rating,
            ParsedVolume = volum1, 
            ParsedArticul = int.Parse(match.Value),
            ParsedRegion = location, 
        };
        
    }


    static async Task<ParsingResult> GetPrice(int taskId, CancellationToken cancellationToken, IDocument document)
    {

        var oldPrice = document.QuerySelector("div.product-buy__old-price.product-buy__with-one").TextContent;
        var priceElement = document.QuerySelector(".product-buy__price");

        var discountElement = priceElement?.QuerySelector(".product-buy__discount");
        discountElement?.Remove();

        string Price = priceElement?.TextContent.Trim();

        string cleaned_price = Price.Replace(" ₽", "");
        string price1 = cleaned_price.Replace(" ", "");

        if (oldPrice != null) {

            string cleaned_old_price = oldPrice.Replace(" ₽", "");
            string price2 = cleaned_old_price.Replace(" ", "");

            return new ParsingResult
            {
                ParsedPrice = int.Parse(price1),
                ParsedOldPrice = int.Parse(price2)
            };
        }
        else
        {
            return new ParsingResult
            {
                ParsedPrice = int.Parse(price1),
                ParsedOldPrice = 0
            };
        }





    }
}