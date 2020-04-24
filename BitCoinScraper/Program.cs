using System;
using System.Collections.Generic;
using System.Collections;
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

            // Creates a hash table that will be used later to convert time zones
            Hashtable tzabbrevs = GetTimeZoneAbbreviationLookup();

            // Alter URL to receive more pages and their links. [Note to self: Before finishing, turn this into a while loop.]
            for (int i = 1; i < 2; i++)
            {
                int num = i * 10;
                string stringNum = num.ToString();
                string nextURL = originalURL.Replace("start=0&", String.Format("start={0}&", stringNum));
                Console.WriteLine(String.Format("Article URLs for page {0}:", i));
                await GetTargetLinksAsync(nextURL, httpClient, client, tzabbrevs);
                Console.WriteLine("");
            }
            Console.ReadLine();
        }
        private static async Task GetTargetLinksAsync(string thisurl, HttpClient httpSession, WebClientExtended oclient, Hashtable tzabbrevs)
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
                    GetArticleInfo(articleURL, oclient, tzabbrevs);
                }
            }
        }
        private static void GetArticleInfo(string arturl, WebClientExtended headerclient, Hashtable tzabbrevs)
        {
            var articleDoc = new HtmlDocument();
            articleDoc.LoadHtml(System.Text.Encoding.UTF8.GetString(headerclient.DownloadData(arturl)));
            
            // This writes out the title of the article
            var titlenode = articleDoc.DocumentNode.SelectSingleNode("//title");
            string title = titlenode.InnerText;
            Console.WriteLine(title);

            //Finds article's time info and also converts it to unixtime
            var times = articleDoc.DocumentNode.SelectNodes("//time");
            string date = times[0].InnerText.Remove(times[0].InnerText.Length - 1, 1);
            string time = times[1].InnerText;
            string timestring = date + ", " + time;
            Console.WriteLine(String.Format("Publish Date: {0}", timestring));
            string tz = timestring.Substring(timestring.Length - 3);
            string newtz = tzabbrevs[tz].ToString();
            string newdate = timestring.Remove(timestring.Length - 3) + newtz;
            DateTime dtversion = DateTime.Parse(newdate);
            long unixtime = ((DateTimeOffset)dtversion).ToUnixTimeSeconds();
            Console.WriteLine(String.Format("Publish Unix Time: {0}", unixtime)); //1586108400

            //This gets the article's author
            var author = articleDoc.DocumentNode.SelectSingleNode("//meta[@name='author']").GetAttributeValue("Content", "No Author");
            Console.WriteLine(String.Format("Author: {0}", author));

            //Removes unwanted paragraphs and returns text
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

            DataAccess.InsertRow(date, time, unixtime, arturl, title, author, articletext);
            Console.WriteLine(articletext);
        }
        static Hashtable GetTimeZoneAbbreviationLookup()
        {
            Hashtable hashtable = new Hashtable();
            String sInput = "ACDT;UTC+10:30|ACST;UTC+09:30|ACT;UTC+08|ADT;UTC-03|AEDT;UTC+11|AEST;UTC+10|AFT;UTC+04:30|AKDT;UTC-08|AKST;UTC-09|AMST;UTC-03|AMT;UTC-04|ART;UTC-03|AST;UTC-04|AWDT;UTC+09|AWST;UTC+08|AZOST;UTC-01|AZT;UTC+04|BDT;UTC+08|BIOT;UTC+06|BIT;UTC-12|BOT;UTC-04|BRT;UTC-03|BST;UTC+01|BTT;UTC+06|CAT;UTC+02|CCT;UTC+06:30|CDT;UTC-05|CEDT;UTC+02|CEST;UTC+02|CET;UTC+01|CHADT;UTC+13:45|CHAST;UTC+12:45|CHOT;UTC+08|ChST;UTC+10|CHUT;UTC+10|CIST;UTC-08|CIT;UTC+08|CKT;UTC-10|CLST;UTC-03|CLT;UTC-04|COST;UTC-04|COT;UTC-05|CST;UTC-06|CT;UTC+08|CVT;UTC-01|CWST;UTC+08:45|CXT;UTC+07|DAVT;UTC+07|DDUT;UTC+10|DFT;UTC+01|EASST;UTC-05|EAST;UTC-06|EAT;UTC+03|ECT;UTC-04|EDT;UTC-04|EEDT;UTC+03|EEST;UTC+03|EET;UTC+02|EGST;UTC+00|EGT;UTC-01|EIT;UTC+09|EST;UTC-05|FET;UTC+03|FJT;UTC+12|FKST;UTC-03|FKT;UTC-04|FNT;UTC-02|GALT;UTC-06|GAMT;UTC-09|GET;UTC+04|GFT;UTC-03|GILT;UTC+12|GIT;UTC-09|GMT;UTC|GST;UTC+04|GYT;UTC-04|HADT;UTC-09|HAEC;UTC+02|HAST;UTC-10|HKT;UTC+08|HMT;UTC+05|HOVT;UTC+07|HST;UTC-10|ICT;UTC+07|IDT;UTC+03|IOT;UTC+03|IRDT;UTC+04:30|IRKT;UTC+08|IRST;UTC+03:30|IST;UTC+05:30|JST;UTC+09|KGT;UTC+06|KOST;UTC+11|KRAT;UTC+07|KST;UTC+09|LHST;UTC+11|LINT;UTC+14|MAGT;UTC+12|MART;UTC-09:30|MAWT;UTC+05|MDT;UTC-06|MET;UTC+01|MEST;UTC+02|MHT;UTC+12|MIST;UTC+11|MIT;UTC-09:30|MMT;UTC+06:30|MSK;UTC+03|MST;UTC-07|MUT;UTC+04|MVT;UTC+05|MYT;UTC+08|NCT;UTC+11|NDT;UTC-02:30|NFT;UTC+11:30|NPT;UTC+05:45|NST;UTC-03:30|NT;UTC-03:30|NUT;UTC-11|NZDT;UTC+13|NZST;UTC+12|OMST;UTC+06|ORAT;UTC+05|PDT;UTC-07|PET;UTC-05|PETT;UTC+12|PGT;UTC+10|PHOT;UTC+13|PKT;UTC+05|PMDT;UTC-02|PMST;UTC-03|PONT;UTC+11|PST;UTC-08|PYST;UTC-03|PYT;UTC-04|RET;UTC+04|ROTT;UTC-03|SAKT;UTC+11|SAMT;UTC+04|SAST;UTC+02|SBT;UTC+11|SCT;UTC+04|SGT;UTC+08|SLST;UTC+05:30|SRET;UTC+11|SRT;UTC-03|SST;UTC-11|SYOT;UTC+03|TAHT;UTC-10|THA;UTC+07|TFT;UTC+05|TJT;UTC+05|TKT;UTC+13|TLT;UTC+09|TMT;UTC+05|TOT;UTC+13|TVT;UTC+12|UCT;UTC|ULAT;UTC+08|USZ1;UTC+02|UTC;UTC|UYST;UTC-02|UYT;UTC-03|UZT;UTC+05|VET;UTC-04:30|VLAT;UTC+10|VOLT;UTC+04|VOST;UTC+06|VUT;UTC+11|WAKT;UTC+12|WAST;UTC+02|WAT;UTC+01|WEDT;UTC+01|WEST;UTC+01|WET;UTC|WIT;UTC+07|WST;UTC+08|YAKT;UTC+09|YEKT;UTC+05|Z;UTC";
            string[] timezones = sInput.Split('|');
            string[] tzValuePair;
            foreach (string timezone in timezones)
            {
                // Value pair example: ACDT;UTC+10:30
                tzValuePair = timezone.Split(';');
                // Console.WriteLine (timezone);
                // Remove UTC from the value
                hashtable.Add(tzValuePair[0], tzValuePair[1].Substring(3));
            }
            return hashtable;
        }
    }
}