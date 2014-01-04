using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ircbot
{
   public interface ICommands
    {
       Dictionary<string, CommandPair> Commands
       {
           get;
           set;
       }
    }
}
