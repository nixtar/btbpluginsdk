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
using System.Collections.Generic;

namespace btbplugin
{
   [Serializable()]
   public class betPluginCommand : ICommand
   {
      private int count = 1;
      private int betStatus = 0;
      private string betMessage = "";
      private List<string> betOptions = new List<string>();
      private uint maxBet = 0;
      private uint minBet = 0;

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

      private bool CheckPointsRespond(User usr, UInt32 pointsNeeded, out string message)
      {
         if (usr.points < pointsNeeded)
         {
            message = usr.displayName + ": You don't have enough points to do that. You have only " + usr.points + "points.";
            return false;
         }
         message = "";
         return true;
      }


      public PluginResponse ValidateParameters(string[] args)
      {
         if (args.Length == 0)
         {
            PluginResponse.Accept;
         }
         if (args[0].Equals("create"))
            {
               if (args.Length < 3)
               {
                  return PluginResponse.Reject;
               }
               else
               {
                  return PluginResponse.Accept;
               }
            }

         if (args[0].Equals("end"))
         {
            return PluginResponse.Accept;
         }

         if (betStatus == 1)
         {  
            string derp = "";
            if (betOptions.Contains(args[0]) && (args.Length < 2) && (UInt32.TryParse(args[1], derp)))
            {
               return PluginResponse.Accept;
            }
            else
            {
               return PluginResponse.Help;
            }
         }
         else
         {
            return PluginResponse.Accept;
         }
      }

      public bool Execute(out string message, User usr, string[] args)
      {

         if (args.Length == 0)
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

         if (args.Length == 2)
         {
            if (betStatus == 1)
            {
               if (betOptions.Contains(args[0]))
               {
                  //TODO Do something with the users Choice value..
               }
               else
               {
                  message = "Invalid option, please choose from the following options: " + string.Join(", ", betOptions);
                  continue;
               }

               int value = 0;
               if (UInt32.TryParse(args[1], out value))
               {
                  //TODO Do something with the users bet value..
               }
               else
               {
                  message = "Invalid points option, please type the command as '!Bet {Choice} {Points}'";
                  continue;
               }
            }
            else
            {
               message = "Sorry, There is currently no bets running...";
            }

         }

         if (args[0].Equals("Create")) 
         {
            if (usr.admin = false)
            {
               message = "Sorry, This is a Mod only command...";
               continue;
            }

            betMessage = args[0];

            foreach (var arg in args)
            {
               if (arg == args[0]) continue;

               if (arg.StartsWith("max="))
               {
                  maxBet = UInt32.Parse(arg.Split('=')[1]); //Set the max bet
                  continue;
               }

               if (arg.StartsWith("min="))
               {
                  minBet = UInt32.Parse(arg.Split('=')[1]); //Set the max bet
                  continue;
               }

               betOptions.Add(arg);
            }
            betStatus = 1;
         }

         if (args[0].Equals("end"))
         {
            if (usr.admin = false)
            {
               message = "Sorry, This is a Mod only command...";
               continue;
            }
            if (betStatus == 0)
            {
               message = "Sorry, There is currently no bets running...";
            }
            else
            {
               message = "Betting has finished..."
               //TODO actually do something when the bet has completed
               betStatus = 0
            }
         }
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
