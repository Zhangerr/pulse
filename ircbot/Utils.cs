using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using ChatterBotAPI;
using System.Reflection;

namespace ircbot
{
    public class Utils
    {
        public static void threadThink(object o)
        {
            try
            {
                IrcBot.writer.WriteLine("PRIVMSG " + ((string)((List<object>)o)[0]) + " :" + ((ChatterBotSession)((List<object>)o)[1]).Think(((string)((List<object>)o)[2])));
            }
            catch (Exception e)
            {
                IrcBot.msg("#lounge", "Exception at threadthink " + e);
            }
        }
        public static void join(string channel)
        {
            if (!IrcBot.channels.Contains(channel.ToLower()))
            {
                IrcBot.writer.WriteLine("JOIN " + channel);
            }
            //admin(channel);
            /*   if (!IrcBot.channels.Contains(channel))
               {
                   IrcBot.channels.Add(channel);
               }*/

        }
        public static void loadPlugin()
        {
            try
            {
                AppDomainSetup ads = new AppDomainSetup();
                ads.ShadowCopyFiles = "true";
                AppDomain.CurrentDomain.SetShadowCopyFiles();
               // AppDomain ad = AppDomain.CreateDomain("temp");
            //    ad.DoCallBack(new CrossAppDomainDelegate(delegate()
             //   {
                    Assembly ass = Assembly.Load("CommandLib");
                     
                    Type t = ass.GetType("CommandLib.CommandImplementation");
                    foreach (Type tt in ass.GetTypes())
                    {
                     //   Console.WriteLine(tt);
                    }
                    //make watch dog program, use waitforexit infinite loop
                 //   Console.WriteLine(t.ToString());
                    ICommands ic = (ICommands)Activator.CreateInstance(t);
                    Commands.commands.Clear();
                    foreach (var i in ic.Commands)
                    {
                        Commands.commands.Add(i.Key, i.Value);
                    }
                 //   Console.WriteLine("s");
              //  }));
                
           //     AppDomain.Unload(ad);
            //   var p = new System.Diagnostics.Process();
               
            }
            catch (Exception e)
            {
                Console.WriteLine("Reverting, " + e);
                Commands.init1();
            }

        }
        public static UserLevel getUserLevel(string user, string channel)
        {
            if (IrcBot.users.ContainsKey(user))
            {
                if (IrcBot.users[user].ContainsKey(channel))
                {
                    return IrcBot.users[user][channel];
                }
            }
            return UserLevel.NORMAL;
        }
        public static void admin(string channel)
        {
            IrcBot.writer.WriteLine("os mode " + channel + " +a " + IrcBot.NICK);
        }
        public static void part(string channel)
        {
            if (IrcBot.channels.Contains(channel.ToLower()))
            {
                IrcBot.writer.WriteLine("PART " + channel);
            }
            /*   if (IrcBot.channels.Contains(channel))
               {
                   IrcBot.channels.Remove(channel);
               }*/

        }
        public static void initThink(ChatterBotSession s, string m, string target)
        {
            List<object> o = new List<object>();
            o.Add(target);
            o.Add(s);
            o.Add(m);
            new Thread(new ParameterizedThreadStart(threadThink)).Start(o);
        }
        public static void threadFML(object o)
        {
            IrcBot.msg((string)o, PingSender.getFML());
        }
        public static void threadBash(object o)
        {
            using (StringReader sr = new StringReader(PingSender.getBash()))
            {
                string result = "";
                string thisLine = "";
                while ((thisLine = sr.ReadLine()) != null)
                {
                    result += thisLine + " ";
                }
                IrcBot.msg((string)o, result);
            }

        }
        public static void initFML(string channel)
        {
            new Thread(new ParameterizedThreadStart(threadFML)).Start(channel);
        }
        public static void initBash(string channel)
        {
            new Thread(new ParameterizedThreadStart(threadBash)).Start(channel);
        }
    }
}
