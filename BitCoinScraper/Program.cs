using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;

namespace BitCoinScraper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // This is the URL of a Google Search.
            string originalURL = "https://www.google.com/search?q=bitcoin+site:forbes.com&sxsrf=ALeKk00hRJnxNYQ2jMNZJtSsMFszejMH5Q:1587155620466&ei=pBKaXqaKHNvKtQbUvbmICg&start=0&sa=N&ved=2ahUKEwim7Ne3p_DoAhVbZc0KHdReDqEQ8tMDegQIEhAt&biw=1707&bih=931";
            Console.WriteLine("This is the original URL:");
            Console.WriteLine(originalURL);
            Console.WriteLine("");

            // Start the httpclient for Google and the webclient for Forbes
            var httpClient = new HttpClient();
            WebClientExtended client = new WebClientExtended();

            // Alter URL to receive more pages and their links.
            for (int i = 1; i < 2; i++)
            {
                int num = i * 10;
                string stringNum = num.ToString();
                string nextURL = originalURL.Replace("start=0&", String.Format("start={0}&", stringNum));
                Console.WriteLine(String.Format("Article URLs for page {0}:", i));
                await GetTargetLinksAsync(nextURL, httpClient, client);
                Console.WriteLine("");
            }
            Console.ReadLine();
        }

        private static async Task GetTargetLinksAsync(string thisurl, HttpClient httpSession, WebClientExtended oclient)
        {
            var html = await httpSession.GetStringAsync(thisurl);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            foreach (var child in htmlDocument.DocumentNode.Descendants("a"))
            {
                if (child.GetAttributeValue("href", "no hyperlinks").StartsWith("/url?q=https://www.forbes.com/sites"))
                {
                    int stopNum = child.GetAttributeValue("href", "").IndexOf('&');
                    string articleURL = child.GetAttributeValue("href", "").Substring(7, stopNum - 7);
                    Console.WriteLine(String.Format("{0} {1}", articleURL.Length, articleURL));
                    GetArticleInfo(articleURL, oclient);
                }
            }
        }
        private static void GetArticleInfo(string arturl, WebClientExtended headerclient)
        {
            var articleDoc = new HtmlDocument();
            articleDoc.LoadHtml(System.Text.Encoding.UTF8.GetString(headerclient.DownloadData(arturl)));
            
            // This writes out the title of the article
            var title = articleDoc.DocumentNode.SelectSingleNode("//title");
            Console.WriteLine(title.InnerText);
            var times = articleDoc.DocumentNode.SelectNodes("//time");
            Console.WriteLine(String.Format("Publish Date: {0}", times[0].InnerText.Remove(times[0].InnerText.Length-1, 1)));
            Console.WriteLine(String.Format("Publish Time: {0}", times[1].InnerText));
            var author = articleDoc.DocumentNode.SelectSingleNode("//meta[@name='author']").GetAttributeValue("Content", "No Author");
            Console.WriteLine(String.Format("Author: {0}", author));
            var figs = articleDoc.DocumentNode.SelectSingleNode("//figure");
            figs.Remove();
            var paragraphs = articleDoc.DocumentNode.SelectNodes("//div[@class='article-body fs-article fs-responsive-text current-article']//p");
            string articletext = "";
            foreach (var paragraph in paragraphs)
            {
                articletext = string.Concat(articletext, paragraph.InnerText);
                //articletext = articletext + paragraph;
                //<div class="article-body">
            }
            Console.WriteLine(articletext);
        }
    }
}