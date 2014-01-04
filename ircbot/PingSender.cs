using System;
using System.Threading;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
/*
* Class that sends PING to irc server every 15 seconds
*/
namespace ircbot
{
    class PingSender
    {
   //     static WebClient client = new WebClient();
        static PingSender() { }
        static string PING = "PING :";
        private Thread pingSender;
        // Empty constructor makes instance of Thread
        public PingSender()
        {
            pingSender = new Thread(new ThreadStart(this.Run));
        }
        // Starts the thread
        public void Start()
        {
            pingSender.Start();
        }
        // Send PING to irc server every 15 seconds
        public static string getBash()
        {
            using(WebClient client = new WebClient()) {
                client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            client.Proxy = null;
            Console.WriteLine("d/ling string");
            string data = client.DownloadString("http://bash.org/?random");
            string result_ = "";
            try
            {
                Regex regexObj = new Regex("<p class=\"qt\">(.*?)</p>", RegexOptions.Singleline);
                Match matchResults = regexObj.Match(data);
                while (matchResults.Success)
                {
                    // matched text: matchResults.Value
                    // match start: matchResults.Index
                    // match length: matchResults.Length
                    //matchResults = matchResults.NextMatch();

                    result_ = HttpUtility.HtmlDecode(matchResults.Groups[1].Value).Replace("<br />", "\n");
                  //  Console.WriteLine(result_);
                   // Console.WriteLine("next");
                    matchResults = matchResults.NextMatch();
                }
                return result_;
            }
            catch (ArgumentException ex)
            {
                // Syntax error in the regular expression
            }
            return "failed to get bash";
        }
        }
        public static string getFML()
        {
            using (WebClient client = new WebClient())
            {
                            
                client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                client.Proxy = null;
             //   Console.WriteLine("d/ling string");
                Stream data = client.OpenRead("http://fmylife.com/random");

            //    Console.WriteLine("finish d/l");
                using (StreamReader reader = new StreamReader(data))
                {
                    String thisLine = "";
                    List<String> fmls = new List<String>();
                  //  Console.WriteLine("init read");
                    while ((thisLine = reader.ReadLine()) != null)
                    {
                        try
                        {
                            Regex regexObj = new Regex("class=\"fmllink\">(.+?)</a>", RegexOptions.Multiline);
                            Match matchResults = regexObj.Match(thisLine);
                            string fml = "";
                            while (matchResults.Success)
                            {
                                fml += matchResults.Groups[1].Value;
                                //     Console.WriteLine(matchResults.Groups[1].Value);
                                matchResults = matchResults.NextMatch();
                            }
                            if (!string.IsNullOrEmpty(fml))
                            {

                                fmls.Add(fml);
                                Console.WriteLine("found fml, breaking. " + fmls.Count);
                                reader.Dispose();
                                break;
                                //    Console.WriteLine(); //to separate fmls
                            }
                        }

                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                    Console.WriteLine("unescaping");
                    try
                    {
                        return Uri.UnescapeDataString(fmls[new Random().Next(fmls.Count)]);
                    }
                    catch (Exception e)
                    {
                        return "STOP SPAMMING !FML PLEASE";
                    }
                }
            }
        }
        int counter;
        public void Run()
        {

            while (true)
            {
               /* counter++;
                if (counter == 3)
                {
              //      IrcBot.writer.Write("JOIN #pulse");
                }*/
                IrcBot.writer.WriteLine(PING + IrcBot.SERVER);
                IrcBot.writer.Flush();
                //   ConsoleKeyInfo ki = Console.ReadKey();
                //   if(ki.KeyChar == 'c') {}
                // if(ki.KeyChar == 'c')

                //     IrcBot.writer.WriteLine("PRIVMSG #pulse :");
                //   IrcBot.writer.Flush();
                Console.WriteLine("pinged");

                Thread.Sleep(15000);
                //    }
            }
        }
    }
}