using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;
using System.Net;

namespace Scratch
{
    class Scratch
    {
        static async Task Main(string[] args)
        {
            //This one with the web client gives me 2015 results. It also works with httpClient if I use the same useragent header.
            //Uri originalURL = new Uri("https://www.google.com/search?q=bitcoin+site%3Aforbes.com&source=lnt&tbs=cdr%3A1%2Ccd_min%3A1%2F1%2F2015%2Ccd_max%3A2%2F1%2F2015&tbm=");
            Uri originalURL = new Uri("https://www.google.com/search?q=bitcoin+site:forbes.com&tbs=cdr:1,cd_min:3/1/2017,cd_max:4/1/2017&sxsrf=ALeKk024LulKuHDl6svGxyr5qSBaa-hThA:1588108834118&ei=Ip6oXtXaBpTWtAbHzrbwAQ&start=30&sa=N&ved=2ahUKEwjV86-3hozpAhUUK80KHUenDR4Q8NMDegQIDBAx&biw=1707&bih=931");

            HtmlDocument firstHTML = new HtmlDocument();

            //This is all for using the first HttpClient
            HttpClientHandler handler = new HttpClientHandler();

            CookieContainer cookies = new CookieContainer();
            handler.CookieContainer = cookies;
            handler.UseCookies = true;
            var httpClient = new HttpClient(handler);

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");//IE
            //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Feedly/1.0 (+http://www.feedly.com/fetcher.html; like FeedFetcher-Google)");
            //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0)");
            //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (iPhone; CPU iPhone OS 10_15_4 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) FxiOS/24.0 Mobile/15E148 Safari/605.1.15");
            //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:75.0) Gecko/20100101 Firefox/75.0");

            var html = await httpClient.GetStringAsync(originalURL);
            firstHTML.LoadHtml(html);

            Console.WriteLine("URLs for Original Search:");
            ListURLs(firstHTML);

            //Console.WriteLine(firstHTML.DocumentNode.OuterHtml);

            Console.ReadLine();
        }

        static void ListURLs(HtmlDocument html)
        {
            foreach (var child in html.DocumentNode.SelectNodes("//a[@class='fl']"))
            {
                child.Remove();
            }

            foreach (var child in html.DocumentNode.Descendants('a'))
            {
                if (child.GetAttributeValue("href", "no hyperlinks").StartsWith("https://www.forbes.com/sites") && !child.InnerText.StartsWith("404 - Forbes"))
                {
                    //int stopNum = child.GetAttributeValue("href", "").IndexOf('&');
                    string articleURL = child.GetAttributeValue("href", "");
                    //.Substring(7, stopNum - 7);
                    Console.WriteLine("Link:");
                    Console.WriteLine(articleURL);
                    Console.WriteLine(child.InnerText);
                }
            }
        }
    }
}