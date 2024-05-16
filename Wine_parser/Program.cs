using System;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.VisualBasic;
using Wine_parser;
using static System.Net.Mime.MediaTypeNames;



class MyClass
{
    static SemaphoreSlim semaphore = new SemaphoreSlim(3, 3);
    static string substring = "/catalog/product/";
    static string substring1 = "/reviews/";
    static string filter = "/filter/";

    static string[] link_list;

    static async Task Main(string[] args)
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        var url = "https://simplewine.ru/catalog/shampanskoe_i_igristoe_vino/";

        //var test = new ParsingResult { ParsedUrl = url };

        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);

        var document = await context.OpenAsync(url);

        var divs = document.QuerySelectorAll(".catalog-grid__item");

        List<string> link_list = new List<string>();

        if (divs != null)
        {
            foreach (var div in divs)
            {
                var links = div.QuerySelectorAll("a").Select(a => a.GetAttribute("href"));
                int count = links.Count();

                foreach (var (link, index) in links.Select((link, index) => (link, index)))
                {
                    if (link.Contains(substring) && !link.Contains(substring1) && !link.Contains(filter))
                    {
                        link_list.Add(link);
                    }

                }

            }
        }
        else
        {
            Console.WriteLine("null");
        }

        int maxDegreeOfParallelism = 3;

        // int delayBetweenThreads = 1000;

        List<ParsingResult> parsingResults = new List<ParsingResult>();

        for (int i = 0; i < link_list.Count; i += maxDegreeOfParallelism)
        {
            // Запускаем не более maxDegreeOfParallelism задач
            int count = Math.Min(maxDegreeOfParallelism, link_list.Count - i);
            List<Task<ParsingResult>> tasks = new List<Task<ParsingResult>>();

            for (int j = 0; j < count; j++)
            {
                string link = link_list[i + j];
                Task<ParsingResult> task = Task.Run(() => Parser(link));
                tasks.Add(task);
            }

            // Ждем завершения всех запущенных задач и добавляем результаты в общий список
            ParsingResult[] results = await Task.WhenAll(tasks);
            parsingResults.AddRange(results);

            /*
            // Добавляем задержку между запусками потоков
            if (i + maxDegreeOfParallelism < link_list.Count)
            {
                await Task.Delay(delayBetweenThreads);
            }
            */
        }

        JsonHelper.JsonCreat(parsingResults);
    }


    static async Task<ParsingResult> Parser(string url_pars)
    {
        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

        var parUrl = "https://simplewine.ru" + $"{url_pars}";
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(parUrl);


        var pictures = document.QuerySelectorAll(".product-slider__slide-picture");

        var Name = document.QuerySelector("div.product-page__head");
        var location = document.QuerySelector("button.location__current.dropdown__toggler").TextContent.Trim();
        var _rating = document.QuerySelector("p.rating-stars__value").TextContent.Trim();
        var volum = document.QuerySelectorAll("dd.product-brief__value");

        var name = Name.QuerySelector("h1").TextContent.Trim();
        var arcicul = Name.QuerySelector("span.product-page__article.js-copy-article").TextContent.Trim();

        var oldPriceElement = document.QuerySelector("div.product-buy__old-price.product-buy__with-one");

        var priceElement = document.QuerySelector(".product-buy__price");

        var volum1 = volum[5].TextContent.Trim();

        var rating_bool = double.TryParse(_rating, NumberStyles.Any, culture, out double rating);
        Regex regex = new Regex(@"\d+");

        Match match = regex.Match(arcicul);

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


        var discountElement = priceElement?.QuerySelector(".product-buy__discount");
        discountElement?.Remove();

        string Price = priceElement?.TextContent.Trim();

        string cleaned_price = Price.Replace(" ₽", "");
        string price1 = cleaned_price.Replace(" ", "");

        if (oldPriceElement != null)
        {
            var oldPrice = oldPriceElement.TextContent;

            string cleaned_old_price = oldPrice.Replace(" ₽", "");
            string price2 = cleaned_old_price.Replace(" ", "");

            return new ParsingResult
            {
                Name = name,
                Price = int.Parse(price1),
                OldPrice = int.Parse(price2),
                Rating = rating,
                Volume = volum1,
                Articul = int.Parse(match.Value),
                Region = location,
                Url = parUrl,
                Pictures = cleanLinks,
            };
        }
        else
        {
            return new ParsingResult
            {
                Name = name,
                Price = int.Parse(price1),
                OldPrice = 0,
                Rating = rating,
                Volume = volum1,
                Articul = int.Parse(match.Value),
                Region = location,
                Url = parUrl,
                Pictures = cleanLinks,
            };
        }
    }
}




    