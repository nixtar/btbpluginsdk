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
      private UInt32 maxBet = 0;
      private UInt32 minBet = 1;
      private UInt32 betUsrPoints = 0;
      private string betUsrOption = "";

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
         if (args[0].Equals("help"))
            return PluginResponse.Help;

         if (args.Length == 0)
         {
            return PluginResponse.Accept;
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
            UInt32 derp;
            if (betOptions.Contains(args[0]) && (args.Length == 2) && (UInt32.TryParse(args[1], out derp)))
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
         message = "How did this happen??";
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
         else if (args.Length == 2)
         {
            if (betStatus == 1)
            {
               if (betOptions.Contains(args[0]))
               {
                  betUsrOption = args[0];
               }
               else
               {
                  message = "Invalid option, please choose from the following options: " + string.Join(", ", betOptions);
                  return false;
               }

               UInt32 betUsrPoints;
               UInt32.Parse(args[1], out betUsrPoints);
               if ((betUsrPoints >= minBet) && (betUsrPoints <= maxBet))
               {
                  message = "Congrats, you are betting " + betUsrOption + " against " + betMessage + " with on " + betUsrPoints + " Points!";
               }
               else
               {
                  message = "Sorry, please make sure your bet is within the current limits (Min Bet=" +  minBet + " Max Bet=" + maxBet + ")";
               }
            }
            else
            {
               message = "Sorry, There is currently no bets running...";
            }

         }
         else if (args[0].Equals("create")) 
         {
            if (usr.admin == false)
            {
               message = "Sorry, This is a Mod only command...";
               return false;
            }

            betMessage = args[1];
            betOptions = new List<string>();
            for (int i = 2; i < args.Length; i++)
            {
               var arg = args[i];
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
            message = "A bet has started, '" + betMessage + "' with options: " + string.Join(", ", betOptions);
         }
         else if (args[0].Equals("end"))
         {
            if (usr.admin == false)
            {
               message = "Sorry, This is a Mod only command...";
               return false;
            }
            if (betStatus == 0)
            {
               message = "Sorry, There is currently no bets running...";
            }
            else
            {
               message = "Betting has finished...";
               //TODO actually do something when the bet has completed
               betStatus = 0;
            }
         }
         return true;
      }

      public byte[] Save()
      {
         return new byte[0];
      }

      public void Load(byte[] data)
      {
      }
   }
}
