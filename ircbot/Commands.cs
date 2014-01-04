using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using System.IO;
using System.Threading;
namespace ircbot
{
    public class Commands
    {
        public delegate void execute(CommandParams cp);
        public static Dictionary<string, CommandPair> commands = new Dictionary<string, CommandPair>();
        public static void init()
        {
            Utils.loadPlugin();
        }
        public static void init1()
        {
            commands.Clear();
          
            commands.Add("!clever", new CommandPair(new CommandInfo("Talk to cleverbot!", UserLevel.VOICE, 2), delegate(CommandParams cp)
            {
                Utils.initThink(IrcBot.ses, cp.message, cp.channel);
            }));
            commands.Add("!panda", new CommandPair(new CommandInfo("Talk to pandabot!", UserLevel.VOICE, 2), delegate(CommandParams cp)
            {
                Utils.initThink(IrcBot.pan, cp.message, cp.channel);
            }));
            commands.Add("!jabber", new CommandPair(new CommandInfo("Talk to jabberbot!", UserLevel.VOICE, 2), delegate(CommandParams cp)
            {
                Utils.initThink(IrcBot.jab, cp.message, cp.channel);
            }));
            commands.Add("!fml", new CommandPair(new CommandInfo("Feeling down? View an fml!", UserLevel.VOICE), delegate(CommandParams cp)
            {
                Utils.initFML(cp.channel);
            }));
            commands.Add("!bash", new CommandPair(new CommandInfo("those im quotes you don't care about", UserLevel.VOICE), delegate(CommandParams cp)
            {
                Utils.initBash(cp.channel);
            }));
            commands.Add("!join", new CommandPair(new CommandInfo("Join a channel", UserLevel.OP, 2), delegate(CommandParams cp)
            {
                if (!IrcBot.noJoin.Contains(cp.splitted[1].ToLower()))
                {
                    Utils.join(cp.splitted[1]);
                    IrcBot.msg(cp.channel, "Joined " + cp.splitted[1]);
                }
                else
                {
                 IrcBot.msg(cp.channel, cp.splitted[1] + " is on the no join list.");
                }
            }));
            commands.Add("!part", new CommandPair(new CommandInfo("Part a channel", UserLevel.OP, 2), delegate(CommandParams cp)
            {
                Utils.part(cp.splitted[1]);
                IrcBot.msg(cp.channel, "Parted " + cp.splitted[1]);
            }));
            commands.Add("!list", new CommandPair(new CommandInfo("Lists available commands."), delegate(CommandParams cp)
            {
                var e = from element in commands where cp.invoker >= element.Value.ci.level select element;
                string result = "";
                foreach (var i in e)
                {
                    result += i.Key + ", ";
                }
                IrcBot.msg(cp.channel, result);
            }));
            commands.Add("!help", new CommandPair(new CommandInfo("Gives more info about a command.", UserLevel.NORMAL, 2), delegate(CommandParams cp)
            {
                string cm = "!" + cp.splitted[1];
                if (commands.ContainsKey(cm))
                {
                    if (cp.invoker >= commands[cm].ci.level)
                    {
                        IrcBot.msg(cp.channel, commands[cm].ci.desc);
                    }
                }
            }));
            commands.Add("!google", new CommandPair(new CommandInfo("Returns google link for term(s).", UserLevel.VOICE, 2), delegate(CommandParams cp)
            {
                string repl = cp.message.Replace(" ", "%20");
                IrcBot.msg(cp.channel, "https://www.google.com/search?q=" + repl);
            }));
            commands.Add("!wiki", new CommandPair(new CommandInfo("Returns wikipedia link for term(s).", UserLevel.VOICE, 2), delegate(CommandParams cp)
            {
                string repl = cp.message.Replace(" ", "%20");
                IrcBot.msg(cp.channel, "http://en.wikipedia.org/wiki/" + repl);
            }));
            commands.Add("!report", new CommandPair(new CommandInfo("Report something/someone for a good reason.", UserLevel.NORMAL, 2), delegate(CommandParams cp)
            {
                IrcBot.msg(cp.user, "You have issued a report for the reason '" + cp.message + "'. Opers have been alerted.");
                IrcBot.msg("#lounge", cp.user + " is in need of help for reason '" + cp.message + "'.");                           
            }));
            commands.Add("!quit", new CommandPair(new CommandInfo("Quit from the server, and close the bot. Requires manual restart, so use with care", UserLevel.ADMIN, 1), delegate(CommandParams cp)
            {
                IrcBot.writer.WriteLine("QUIT");
                Environment.Exit(0);
            }));
            commands.Add("!anjoin", new CommandPair(new CommandInfo("Adds a channel to the no join list. If the bot is currently on the channel, it will part.", UserLevel.ADMIN, 2), delegate(CommandParams cp)
            {
                string chan = cp.splitted[1].ToLower();
                if (chan.StartsWith("#"))
                {
                    IrcBot.noJoin.Add(chan);
                    IrcBot.msg(cp.channel, "Added " + cp.splitted[1] + " to no join list.");
                    Utils.part(chan);
                }

            }));
            commands.Add("!rnjoin", new CommandPair(new CommandInfo("Removes a channel from the no join list. The bot will not auto-join.", UserLevel.ADMIN, 2), delegate(CommandParams cp)
            {
                string chan = cp.splitted[1].ToLower();
                if (chan.StartsWith("#"))
                {
                    IrcBot.noJoin.Remove(chan);
                    IrcBot.msg(cp.channel, "Remove " + cp.splitted[1] + " to no join list.");
                }

            }));
            commands.Add("!lnjoin", new CommandPair(new CommandInfo("Lists current no join channels.", UserLevel.ADMIN), delegate(CommandParams cp)
            {
                string res = "List of no join channels: ";
                foreach(string s in IrcBot.noJoin) {
                    res += s + ", ";
                }
                IrcBot.msg(cp.channel, res);
            }));
            commands.Add("!kick", new CommandPair(new CommandInfo("Kicks a user from a channel.", UserLevel.OP, 2), delegate(CommandParams cp)
            {
                string user = cp.splitted[1];
                string chan = cp.channel;
                cp.sr.WriteLine("os KICK " + chan + " " + user + " bot kick");

            }));
            commands.Add("!silence", new CommandPair(new CommandInfo("Silences a user. Syntax is !silence <user> <number of minutes>, if the minutes is not specified it defaults to 5.", UserLevel.OP, 2), delegate(CommandParams cp)
            {
                string user = cp.splitted[1];
                int time = 5;
                if (cp.splitted.Length > 2)
                {
                    Int32.TryParse(cp.splitted[2], out time);
                }
                IrcBot.msg(user, "You have been silenced for " + time + " minutes. You will not recieve messages or be able to chat until the silence expires.");
                cp.sr.WriteLine("shun " + user + " " + time + "m1s :botsilence");
                Thread t = new Thread(new ParameterizedThreadStart(unshun));
                t.IsBackground = true;
                t.Start(new string[] { user, (1000 * 60 * time).ToString()});
            }));
        }
       public static void unshun(object o)
        {
            string[] s = o as string[];
            string user = s[0];
            int sleeptime = Int32.Parse(s[1]);
            System.Threading.Thread.Sleep(sleeptime);
            IrcBot.msg(user, "You are free to speak now.");
        }
    }
    public class CommandPair
    {
        public Commands.execute exe;
        public CommandInfo ci;
        public CommandPair(CommandInfo i, Commands.execute e)
        {
            exe = e;
            ci = i;
        }
    }
    public class CommandParams
    {
        //channel
        public string channel;
        //user sending
        public string user;
        /// <summary>
        /// the actual message, if applicable (no command included)
        /// </summary>

        public string message;
        /// <summary>
        /// the entire line
        /// </summary>

        public string entire;
        /// <summary>
        /// the message splitted (includes command)
        /// </summary>

        public string[] splitted;

        public UserLevel invoker;
        public StreamWriter sr;
        public CommandParams(string c, string u, string m, string e, string[] s, StreamWriter r, UserLevel inv)
        {
            channel = c;
            user = u;
            message = m;
            entire = e;
            splitted = s;
            sr = r;
            invoker = inv;
        }
    }

    public class CommandInfo
    {
        /// <summary>
        /// description
        /// </summary>
        public string desc;
        //permission level required to use, 0 is normal, 1 is voice, 2 is hop, 3 is op, 4 is +q, 5 is +a
        public UserLevel level;
        //number of args required (at least)
        public int args = 1;
        public CommandInfo(string d)
            : this(d, UserLevel.NORMAL)
        {

        }
        public CommandInfo(string d, UserLevel level)
            : this(d, level, 1)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d">description of the command</param>
        /// <param name="level">UserLevel required to use the command</param>
        /// <param name="arg">Number of arguments</param>
        public CommandInfo(string d, UserLevel level, int arg)
        {
            desc = d;
            this.level = level;
            args = arg;
        }
    }
    public enum UserLevel : int
    {
        NORMAL = 0,
        VOICE = 1,
        HOP = 2,
        OP = 3,
        OWNER = 4,
        ADMIN = 5
    }
}
