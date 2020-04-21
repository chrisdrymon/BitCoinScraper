using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;

namespace BitCoinScraper
{
    class TitlesOnly
    {
        static async Task Main(string[] args)
        {
            // This is the URL of a Google Search.
            string originalURL = "https://www.google.com/search?q=bitcoin+site:forbes.com&sxsrf=ALeKk00hRJnxNYQ2jMNZJtSsMFszejMH5Q:1587155620466&ei=pBKaXqaKHNvKtQbUvbmICg&start=0&sa=N&ved=2ahUKEwim7Ne3p_DoAhVbZc0KHdReDqEQ8tMDegQIEhAt&biw=1707&bih=931";
            Console.WriteLine("This is the original URL:");
            Console.WriteLine(originalURL);
            Console.WriteLine("");
            var httpClient = new HttpClient();

            // Alter URL to receive more pages and their links.
            for (int i = 1; i < 3; i++)
            {
                int num = i * 10;
                string stringNum = num.ToString();
                string nextURL = originalURL.Replace("start=0&", String.Format("start={0}&", stringNum));
                Console.WriteLine(String.Format("Article URLs for page {0}:", i));
                await GetTargetLinksAsync(nextURL, httpClient);
                Console.WriteLine("");
            }
            Console.ReadLine();
        }

        private static async Task GetTargetLinksAsync(string thisurl, HttpClient httpSession)
        {
            var html = await httpSession.GetStringAsync(thisurl);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            Console.WriteLine(htmlDocument.DocumentNode.OuterHtml);
            Console.ReadLine();
            //foreach (var child in htmlDocument.DocumentNode.Descendants("a"))
            //{
            //    if (child.GetAttributeValue("href", "no hyperlinks").StartsWith("/url?q=https://www.forbes.com/sites"))
            //    {
            //        int stopNum = child.GetAttributeValue("href", "").IndexOf('&');
            //        string articleURL = child.GetAttributeValue("href", "").Substring(7, stopNum - 7);
            //        Console.WriteLine(String.Format("{0} {1}", articleURL.Length, articleURL));
            //        var arthtml = await httpSession.GetStringAsync(articleURL);
            //        var arthtmldoc = new HtmlDocument();
            //        arthtmldoc.LoadHtml(arthtml);
            //        Console.WriteLine(arthtmldoc.DocumentNode.OuterHtml);
            //        Console.ReadLine();
            //        //await GetArticleInfoAsync(articleURL, httpSession);
            //    }
            //}
        }
        //private static async Task GetArticleInfoAsync(string arturl, HttpClient httpSession)
        //{
        //    var html = await httpSession.GetStringAsync(arturl);

        //    var htmlDocument = new HtmlDocument();
        //    htmlDocument.LoadHtml(html);
        //    Console.WriteLine(htmlDocument.DocumentNode.OuterHtml);
        //    Console.ReadLine();
        //}
    }
}