// namespace btbcomm
// {
//   public interface ICommand
//   {
//      string Name { get; }
//      string Help { get; }
//      string[] Command { get; }

//      // Return true if the paremeters seem OK to execute. Help is displayed if you return false.
//      bool CheckParameters(string[] args);

//      // return true for success
//      bool Execute(out string message, btb.User usr, string[] args);

//      byte[] Save();
//      void Load(byte[] data);
//   }
// }

// !bet Create {Bet Message} {Space separated possible outcomes} min=1 max=100
// !bet End {Result}
// !bet {Choice} {Points}
// Every 2 minute message spam with the current bet when one is running.
// EG:
// MOD: !newbet “Will Bear Win?” (Y,N) 100
// User: !bet Y 500

using System;
using System.IO;
using btbcomm;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

namespace btbplugin
{
   [Serializable()]
   public class betPluginCommand : ICommand
   {
      private int count = 1;
      private int betStatus = 0;
      private string betMessage = "";
      private List<string> betOptions = new List<string>();
      private int maxBet = "";
      private int minBet = "";

      public string Name
      {
         get
         {
            return "betPlugin";
         }
      }

      public string Help
      {
         get
         {
            return "While a bet is under way type '!Bet {Choice} {Points}'... " +
            "EG: '!Bet Yes 1337'..." +
            "Type '!Bet' to show if a bet is currently underway..." +
            "Author: Nixtar " +
            "Version: 0.0.1";
         }
      }

      public string[] Command
      {
         get
         {
            string[] commands = { "!bet" };
            return commands;
         }
      }

      private bool CheckPointsRespond(btb.User usr, UInt32 pointsNeeded, out string message)
      {
         if (usr.points < pointsNeeded)
         {
            message = usr.displayName + ": You don't have enough points to do that. You have only " + usr.points + "points.";
            return false;
         }
         return true;
      }


      public bool CheckParameters(string[] args)
      {
         if (args[0].Equals("create"))
            {
               if (args.length < 3)
               {
                  return false;
               }
               else
               {
                  return true;
               }
            }

         if (args[0].Equals("end"))
         {
            return true;
         }

         if (betStatus == 1)
         {
            if (betOptions.Contains(args[0]) & (args.length < 2) & (Double.TryParse(args[1], 0)))
            {
               return true;
            }
            else
            {
               return false;
            }
         }
         else
         {
            return True;
         }
      }

      public bool Execute(out string message, btb.User usr, string[] args)
      {

         if (args.length == 0)
         {
            if (betStatus == 0)
            {
               message = "Sorry, There is currently no bets running...";
            }
            else
            {
               message = "Bet Details: " + betMessage + " Options: " + string.Join(", ", betOptions);
            }
         }

         if (args.length == 2)
         {
            if (betStatus == 1)
            {
               if(betOptions.Contains(args[0]))
               {
                  //TODO Do something with the users Choice value..
               }
               else
               {
                  message = "Invalid option, please choose from the following options: " + string.Join(", ", betOptions);
               }

               if(int.TryParse(args[1], out value))
               {
                  //TODO Do something with the users bet value..
               }
            }
            else
            {
               message = "Sorry, There is currently no bets running...";
            }

         }

         if (args[0].Equals("Create")) 
         {
            betMessage = args[0];

            // //Set the max bet
            // string maxArg = "max=";
            // int maxPos = Array.IndexOf(args, maxArg);
            // if (maxPos > -1)
            // {
            //    int maxBet = UInt32.Parse(args[maxPos].split('=')[1]);
            // }

            // //Set the min bet
            // string minArg = "min=";
            // int minPos = Array.IndexOf(args, minArg);
            // if (minPos > -1)
            // {
            //    int minBet = UInt32.Parse(args[minPos].split('=')[1]);
            // }

            foreach (var arg in args)
            {
               if (arg == args[0]) continue;

               maxBet = arg.Where(str => str.StartsWith("max="));
               if (maxBet = null)
               {
               continue;
               }
               else
               {
                  int maxBet = UInt32.Parse(maxBet.split('=')[1]); //Set the max bet
                  continue;
               }

               string minBet = arg.Where(str => str.Contains("min=*"));
               if (minBet = null) 
               {
               continue;
               }
               else
               {
                  int minBet = UInt32.Parse(minBet.split('=')[1]); //Set the min bet
                  continue;
               }
               betOptions.add(arg);
            }
            betStatus = 1;
         }

         if (args[0].Equals("Finish"))
         {
            if (betStatus == 0)
            {
               message = "Sorry, There is currently no bets running...";
            }
         }
         message = usr.displayName + ": Here is an example response, you have " + usr.points + "points.";
         message += "This command has been ran a total of " + count++ + " times.";
         return true;
      }

      public byte[] Save()
      {
         MemoryStream exampleStream = new MemoryStream();
         BinaryFormatter serialiser = new BinaryFormatter();
         serialiser.Serialize(exampleStream, this);
         return exampleStream.GetBuffer();
      }

      public void Load(byte[] data)
      {
         MemoryStream exampleStream = new MemoryStream(data, false);
         BinaryFormatter deserialiser = new BinaryFormatter();
         count = (deserialiser.Deserialize(exampleStream) as betPluginCommand).count;
      }
   }
}
