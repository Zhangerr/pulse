using System;
using ChatterBotAPI;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
//using ChatterBotAPI;
//multi threaded bots, fml, fix up faq system
namespace ircbot
{
    /// <summary>
    /// consider making the dictionary of users only add permissions if NOT a normal user, otherwise just assume normal and don't add (so when checking just assume normal if key not found for user/channel. would make everything much much easier and reduce memory usage since there will always be many more normal users than ops, but when doing whois would have to clear list first so no permissions linger
    /// add faqs
    /// </summary>
    public class IrcBot
    {
        // Irc server to connect
        public static string SERVER = "91.121.201.125"; //exp.ulse.net
        // Irc server's port (6667 is default port)
        public static int PORT = 6667;
        // User information defined in RFC 2812 (Internet Relay Chat: Client Protocol) is sent to irc server
        public static string USER = "USER pulsar pulse pulsing :impulsing pulsation";
        // Bot's nickname
        public static string NICK = "pulsar";
        // Channel to join
        public static string CHANNEL = "#pulse";
        public static List<string> channels = new List<string>();
        static Dictionary<string, string> dic = new Dictionary<string, string>();
        public static Dictionary<string, Dictionary<string, UserLevel>> users = new Dictionary<string, Dictionary<string, UserLevel>>();
        static Dictionary<string, ChatterBotSession> pms = new Dictionary<string, ChatterBotSession>();
        public static List<string> noJoin = new List<string>(); //channels NOT to join
        public static void msg(string channel, string msg)
        {
            writer.WriteLine("PRIVMSG " + channel + " :" + msg);
        }
        static void oper()
        {

            writer.WriteLine("identify pulse4life96Zhangergetintheringfeltallthistime");
            writer.WriteLine("oper pulsar pulse4life96Zhangergetintheringfeltallthistime");
            // Utils.admin(CHANNEL);
            writer.WriteLine("LIST >0,<10000");
            Thread t = new Thread(new ThreadStart(listChannels));
            t.Start();
            t.IsBackground = true;
        }

        static IrcBot()
        {

            dic.Add("pulse", "a rhythm game coded by matt and alex; it is currently in alpha! feedback is much appreciated");
            dic.Add("irc", "irc is a place for socializing, but remember there are rules as well. use common sense and be respectful to others");
            dic.Add("notechart", "feel free to contribute as many notecharts as you would like on the forums!");
            dic.Add("editor", "use the editor to design and create notecharts");
            dic.Add("graphics", "we are in desparate need of graphics. please, contribute a skin if you would be so kind.");
            dic.Add("abuse", "please do not abuse me!");
            dic.Add("fml", "fml is a site to lighten up your day");
            dic.Add("clever", "cleverbot is a human");
            dic.Add("troll", "TBTE.");
            string help = "list of commands: ";
            foreach (var i in dic)
            {
                help += i.Key + ", ";
            }
            dic.Add("help", help + "help");
        }
        public static void parseUsers(string channel, string inputLine)
        {
            //      string channel = param[4];//.Equals(CHANNEL)

            string[] nicks = inputLine.Substring(1).Split(':')[1].Split(' ');
            foreach (string nick in nicks)
            {
                if (!string.IsNullOrWhiteSpace(nick))
                {
                    string rnick = nick;
                    UserLevel toAdd = UserLevel.NORMAL;
                    switch (nick[0])
                    {
                        case '+':
                            toAdd = UserLevel.VOICE;
                            break;
                        case '%':
                            toAdd = UserLevel.HOP;
                            break;
                        case '@':
                            toAdd = UserLevel.OP;
                            break;
                        case '~':
                            toAdd = UserLevel.OWNER;
                            break;
                        case '&':
                            toAdd = UserLevel.ADMIN;
                            break;
                    }

                    if (toAdd != UserLevel.NORMAL)
                    {
                        rnick = nick.Substring(1);

                        if (users.ContainsKey(rnick))
                        {
                            if (users[rnick].ContainsKey(channel))
                            {
                                users[rnick][channel] = toAdd;
                            }
                            else
                            {
                                users[rnick].Add(channel, toAdd);
                            }
                        }
                        else
                        {
                            users.Add(rnick, new Dictionary<string, UserLevel>());
                            users[rnick].Add(channel, toAdd);
                        }
                    }
                    else
                    {
                        if (users.ContainsKey(rnick))
                        {
                            if (users[rnick].ContainsKey(channel))
                            {
                                users[rnick].Remove(channel);
                                if (users[rnick].Count == 0)
                                {
                                    users.Remove(rnick);
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void listChannels()
        {
            while (true)
            {
                Thread.Sleep(1000 * 60 * 3); //every 3 mins
                writer.WriteLine("LIST >0,<10000");
            }
        }
        public static ChatterBotSession jab;
        public static ChatterBotSession pan;
        public static ChatterBotSession ses;
        // StreamWriter is declared here so that PingSender can access it
        public static StreamWriter writer;
        static bool opered;
        static void Main(string[] args)
        {
            try
            {
                Utils.loadPlugin();
                /*Console.WriteLine("try");
                Console.WriteLine(Commands.commands.Count);
                Console.ReadKey();
                Utils.loadPlugin();
                Console.WriteLine("loading again");
                Console.WriteLine(Commands.commands.Count);
                Console.ReadKey();*/
                NetworkStream stream;
                TcpClient irc;
                string inputLine;
                StreamReader reader;
                //   try
                //  {
                irc = new TcpClient(SERVER, PORT);
                stream = irc.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);
                writer.AutoFlush = true;
                writer.WriteLine(USER);
                writer.WriteLine("NICK " + NICK);
                //   Utils.join(CHANNEL);            
                ChatterBotFactory factory = new ChatterBotFactory();
                ChatterBot clever = factory.Create(ChatterBotType.CLEVERBOT);
                ChatterBot jabber = factory.Create(ChatterBotType.JABBERWACKY);
                ChatterBot panda = factory.Create(ChatterBotType.PANDORABOTS, "d689f7b8de347251");
                jab = jabber.CreateSession();
                pan = panda.CreateSession();
                ses = clever.CreateSession();
                //Thread t = new Thread(new ThreadStart(oper));
                //   t.Start();
                Commands.init();
                opered = false;
                while (true)
                {
                    while ((inputLine = reader.ReadLine()) != null)
                    {

                        int numeric;
                        string noinit = inputLine.Substring(1);
                        string text = noinit.Substring(noinit.IndexOf(':') + 1);
                        string[] splitted = text.Split(' ');
                        string[] param = inputLine.Split(' ');
                        if (param.Length > 3 && (param[3].Equals("=") || param[3].Equals("@")))
                        {
                            parseUsers(param[4], inputLine);
                        }
                        // :alex!alex@imp.ulse.net MODE #pulse +v Sync
                        else if (param.Length > 4 && param[1] == "MODE")
                        {
                            writer.WriteLine("WHOIS " + param[4]);
                        }
                        else if (param.Length > 1 && param[1] == "NICK")
                        {
                            string user = inputLine.Substring(1, inputLine.IndexOf('!') - 1);
                            if (users.ContainsKey(user))
                            {
                                var temp = users[user];
                                users.Remove(user);
                                users.Add(text, temp);
                                //  users.Add()
                            }
                        }
                        else if (param.Length > 3 && param[1] == "KICK")
                        {
                            string chan = param[2];
                            string user = param[3];
                            channels.Remove(chan);
                            if (user == NICK)
                            {
                                Utils.join(chan);
                            }
                            else
                            {
                                if (users.ContainsKey(user))
                                {
                                    if (users[user].ContainsKey(chan))
                                    {
                                        users[user].Remove(chan);
                                    }
                                }
                            }
                        }
                        else if (inputLine.ToLower().Contains("privmsg"))
                        {

                            string channel = inputLine.Split(' ')[2];
                            string user = inputLine.Substring(1, inputLine.IndexOf('!') - 1);
                            if (!Directory.Exists("chatlog"))
                            {
                                Directory.CreateDirectory("chatlog");
                            }
                            string date = DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year;
                            File.AppendAllText("chatlog\\" + date + ";" + channel + ".txt", "[" + DateTime.Now.ToString() + "] <" + user + ">: " + text + "\r\n");
                            bool ispm = false;
                            if (inputLine.Split(' ')[2].Equals(NICK)) //private message
                            {
                                if (!splitted[0].StartsWith("!"))
                                {
                                    if (!pms.ContainsKey(user)) //cool bot mode but useless
                                    {
                                        pms.Add(user, panda.CreateSession());
                                    }
                                    //   writer.WriteLine("PRIVMSG " + user + " :" + pms[user].Think(text));
                                    Utils.initThink(pan, text, user);
                                }
                                ispm = true;
                            }
                            string command = splitted[0];
                            if (command.StartsWith("!"))
                            {


                                string rest = "";

                                if (command.Length > 1 && splitted.Length > 1)
                                {
                                    rest = text.Substring(command.Length + 1);
                                }
                                else
                                {
                                    rest = text;
                                }
                                command = command.ToLower();


                                //  else
                                {
                                    var cmds = Commands.commands;
                                    if (cmds.ContainsKey(command))
                                    {
                                        CommandPair cp = cmds[command];
                                        // if (users.ContainsKey(user))
                                        UserLevel lvl = Utils.getUserLevel(user, ispm ? "#pulse" : channel);
                                        if (lvl >= cp.ci.level)
                                        {
                                            if (splitted.Length >= cp.ci.args)
                                            {
                                                cp.exe(new CommandParams(ispm ? user : channel, user, rest, inputLine, splitted, writer, lvl));
                                                msg("#services", command + " executed by " + user + " on " + (ispm ? user : channel));
                                            }
                                            else
                                            {
                                                msg(ispm ? user : channel, "Not enough parameters! " + command + " requires " + cp.ci.args + " arguments");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (inputLine.StartsWith("PING"))
                        {
                            writer.WriteLine("PONG " + inputLine.Split(' ')[1]);
                        }
                        else if (param.Length > 2 && (param[1] == "JOIN" || param[1] == "PART"))
                        {

                            if (inputLine.Split('!')[0].Substring(1).Equals(NICK))
                            {
                                string channel = param[2].Substring(1);
                                if (param[1] == "JOIN")
                                {

                                    Utils.admin(channel);
                                    channels.Add(channel.ToLower());
                                }
                                else
                                {
                                    channels.Remove("#" + channel.ToLower()); //for some reason no leading : when part
                                }
                            }
                        }
                        else if (param.Length > 3 && Int32.TryParse(param[1], out numeric))
                        {
                            if (numeric == 322) //channel listing
                            {
                                if (!noJoin.Contains(param[3].ToLower()))
                                {
                                    Utils.join(param[3]);
                                }
                            }
                            else if (numeric == 376) //end of motd
                            {
                                if (!opered)
                                {
                                    oper();
                                    opered = true;
                                }
                                //>> :exp.ulse.net 319 alex Sync :+#pulse #touhou ~#sync #minecraft 
                            }
                            else if (numeric == 319) //whois channel list
                            {
                                string user = param[3];
                                string[] channels = text.Split(' ');
                                foreach (string s in from i in channels where !string.IsNullOrWhiteSpace(i) select i)
                                {
                                    UserLevel toAdd = UserLevel.NORMAL;
                                    switch (s[0])
                                    {
                                        case '+':
                                            toAdd = UserLevel.VOICE;
                                            break;
                                        case '%':
                                            toAdd = UserLevel.HOP;
                                            break;
                                        case '@':
                                            toAdd = UserLevel.OP;
                                            break;
                                        case '~':
                                            toAdd = UserLevel.OWNER;
                                            break;
                                        case '&':
                                            toAdd = UserLevel.ADMIN;
                                            break;
                                    }
                                    string channame = s;
                                    if (toAdd != UserLevel.NORMAL)
                                    {
                                        channame = channame.Substring(1); //getrid of symbol if  special status
                                        if (users.ContainsKey(user))
                                        {
                                            if (users[user].ContainsKey(channame))
                                            {
                                                users[user][channame] = toAdd;
                                            }
                                            else
                                            {
                                                users[user].Add(channame, toAdd);
                                            }
                                        }
                                        else
                                        {
                                            users.Add(user, new Dictionary<string, UserLevel>());
                                            users[user].Add(channame, toAdd);
                                        }
                                    }
                                    else
                                    {
                                        if (users.ContainsKey(user))
                                        {
                                            if (users[user].ContainsKey(channame))
                                            {
                                                users[user].Remove(channame);
                                                if (users[user].Count == 0)
                                                {
                                                    users.Remove(user);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //if (!(inputLine.EndsWith("JOIN :" + CHANNEL) || inputLine.EndsWith("QUIT")))
                        {
                            Console.WriteLine(inputLine);
                        }
                    }
                    // Close all streams
                    writer.Close();
                    reader.Close();
                    irc.Close();
                }
            }
            catch (Exception e)
            {
                // Show the exception, sleep for a while and try to establish a new connection to irc server
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e);
                Thread.Sleep(5000);
                string[] argv = { };
                Console.WriteLine("restarting");
                Main(argv);
            }
        }



    }
    /*if (users.ContainsKey(user))
                               {
                                   if (true)
                                   {
                                       if (command.Equals("!fml"))
                                       {
                                           initFML(channel);
                                       }
                                       else if (command.Equals("!bash"))
                                       {
                                           initBash(channel);

                                       }
                                       else if (command.Equals("!faq"))
                                       {
                                           if (splitted.Length < 2)
                                           {
                                               writer.WriteLine("PRIVMSG " + channel + " :Please enter more arguments. Type 'help' to see a list");

                                           }
                                           else
                                           {
                                               string entry = splitted[1].ToLower();

                                               if (dic.ContainsKey(entry))
                                               {
                                                   writer.WriteLine("PRIVMSG " + channel + " :" + dic[entry]);

                                               }
                                               else
                                               {
                                                   writer.WriteLine("PRIVMSG " + channel + " :Entry not found!");
                                               }
                                           }
                                       }
                                       else if (command.Equals("!clever"))
                                       {
                                           if (splitted.Length < 2)
                                           {
                                               writer.WriteLine("PRIVMSG " + channel + " :Please enter more arguments.");

                                           }
                                           else
                                           {
                                               //writer.WriteLine("PRIVMSG " + channel + " :" + ses.Think(rest));
                                               initThink(ses, rest, channel);
                                           }
                                       }
                                       else if (command.Equals("!panda"))
                                       {
                                           if (splitted.Length < 2)
                                           {
                                               writer.WriteLine("PRIVMSG " + channel + " :Please enter more arguments.");

                                           }
                                           else
                                           {
                                               //writer.WriteLine("PRIVMSG " + channel + " :" + pan.Think(rest));
                                               initThink(pan, rest, channel);
                                           }
                                       }
                                       else if (command.Equals("!jabber"))
                                       {
                                           if (splitted.Length < 2)
                                           {
                                               writer.WriteLine("PRIVMSG " + channel + " :Please enter more arguments.");

                                           }
                                           else
                                           {
                                               //writer.WriteLine("PRIVMSG " + channel + " :" + jab.Think(rest));
                                               initThink(jab, rest, channel);
                                           }
                                       }
                                       else if (command.Equals("!join"))
                                       {
                                           if (splitted.Length > 1)
                                           {
                                               writer.WriteLine("JOIN " + splitted[1]);
                                           }
                                       }
                                       else if (command.Equals("!oper"))
                                       {
                                           writer.WriteLine("oper Zhanger 96Zhangerfeltallthistime");
                                       } else if (command.Equals("!addfaq")) {
                                           if(splitted.Length > 2) 
                                           {
                                               if(!dic.ContainsKey(splitted[1]))
                                               dic.Add(splitted[1], rest.Substring(splitted[1].Length + 1) );
                                               msg(channel, "added " + splitted[1]);
                                           }
                                       }
                                       else if (command.Equals("!removefaq"))
                                       {
                                           if (splitted.Length > 1)
                                           {
                                               if (dic.ContainsKey(splitted[1]))
                                               {
                                                   dic.Remove(splitted[1]);
                                                   msg(channel, "removed " + splitted[1]);
                                               }
                                           }
                                       }
                                       else if (command.Equals("!list"))
                                       {
                                           string faqs = "";
                                           foreach (var i in dic)
                                           {
                                               faqs += i.Key + ", ";
                                           }
                                           msg(channel, faqs);
                                       }
                                       else if (command.Equals("!savefaqs"))
                                       {
                                           int counter = 0;
                                           while(File.Exists("faqs" + counter + ".txt")) {
                                               counter++;
                                           }
                                           List<string> temp = new List<string>();
                                           foreach (var i in dic)
                                           {
                                               temp.Add(i.Key + "|" + i.Value);
                                           }
                                           File.AppendAllLines("faqs" + counter + ".txt", temp);
                                           msg(channel, "saved " + dic.Count + " faqs to faqs" + counter + ".txt");
                                       }
                                       else if (command.Equals("!clearfaqs"))
                                       {
                                           dic.Clear();
                                           msg(channel, "cleared faqs");
                                       }
                                       else if (command.Equals("!loadfaqs"))
                                       {
                                           if (splitted.Length > 1)
                                           {
                                               string filename = splitted[1];
                                               if (File.Exists(filename) && !filename.Contains("\\")) // \ could mean hack attempt to access unauthorized
                                               {
                                                   dic.Clear();
                                                   foreach (string i in File.ReadAllLines(filename))
                                                   {
                                                       string[] temp = i.Split('|');
                                                       dic.Add(temp[0], temp[1]);
                                                   }
                                                   msg(channel, "loaded " + dic.Count + " faqs");
                                               }
                                               else
                                               {
                                                   msg(channel, "file does not exist");
                                               }
                                           }
                                       }
                                       else if (command.Equals("!google"))
                                       {
                                           string repl = rest.Replace(" ", "%20");
                                           writer.WriteLine("PRIVMSG " + CHANNEL + " :" + "https://www.google.com/search?q=" + repl);
                                       }
                                       else if (command.Equals("!wiki"))
                                       {
                                           string repl = rest.Replace(" ", "%20");
                                           writer.WriteLine("PRIVMSG " + CHANNEL + " :" + "http://en.wikipedia.org/wiki/" + repl);
                                       }
                                   }
                               }
                               if (command.Equals("!report")) //normal user command start
                               {
                                   writer.WriteLine("PRIVMSG " + user + " :You have issued a report for the reason '" + rest + "'. Opers have been alerted.");
                                   writer.WriteLine("PRIVMSG #lounge :" + user + " is in need of help for reason '" + rest + "'.");
                               }
                           }*/

}
