using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;

namespace Scratch
{
    class Program
    {
        static void Main(string[] args)
        {
            // This is the URL of a Forbes Article.
            WebClientExtended client = new WebClientExtended();
            string originalURL = "https://www.forbes.com/sites/billybambrough/2020/04/18/its-make-or-break-for-bitcoin/";
            //var httpClient = new HttpClient();
            //var html = await httpClient.GetStringAsync(originalURL);
            var htmlDocument = new HtmlDocument();
            //htmlDocument.LoadHtml(html);
            htmlDocument.LoadHtml(System.Text.Encoding.UTF8.GetString(client.DownloadData(originalURL)));

            Console.WriteLine(htmlDocument.DocumentNode.OuterHtml);
            Console.ReadLine();
            //foreach (var node in nodes)
            //<div class="body-container">
            //    <div class="article-headline-container">
                //  <div class="header-content-container">
                    //  <div class="metrics-channel light-text">
                        //  <div class="content-data light-text">
                            //<span class="hidden pageviews-wrapper">
                            //</span>
                        //    <time>Apr 18, 2020,</time>
                        //    <span class="time"><time>05:55am EDT</time></span>
                         // </div>
                     // </div>
        }
    }
}