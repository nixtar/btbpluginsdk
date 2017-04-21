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

namespace btbplugin
{
   public class betPluginCommand : ICommand
   {
      private int count = 1;
      private int betStatus = 0;
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
            "EG: '!Bet No 1337'..." +
            "Type '!Bet' to show if a bet is currently underway..." +
            "Author: Nixtar " +
            "Version: 0.0.1";
         }
      }

      public string[] Command
      {
         get
         {
            string[] commands = { "!Bet" };
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
         if (args[0].Equals("Create"))
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

         if (args[0].Equals("End"))
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
               message = "Bet Details: " betMessage + " " + "Options: " + betOptions;
            }
         }

         if (args[0].Equals("Create"))
         {
            string betMessage = args[0];
            string[] betOptions 
         }

         if (args[0].Equals("Finish"))
         {
            //TODO get the result and pay the winners
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
