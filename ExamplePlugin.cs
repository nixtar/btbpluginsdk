﻿using System;
using System.IO;
using btbcomm;
using System.Runtime.Serialization.Formatters.Binary;

namespace btbplugin
{
   [Serializable()]
   public class btbCommand : ICommand
   {
      private int count = 1;
      public string Name
      {
         get
         {
            return "Example";
         }
      }

      public string Help
      {
         get
         {
            return "Type \"!Example param1\" to try";
         }
      }

      public string[] Command
      {
         get
         {
            string[] commands = { "!example" };
            return commands;
         }
      }

      public bool CheckParameters(string[] args)
      {
         if (args.Length < 1)
            return false;
         return true;
      }

      public bool Execute(out string message, btb.User usr, string[] args)
      {
         message = usr.displayName + ": Here is an example response, you have " + usr.points + "points.";
         message += " This command has been ran a total of " + count++ + " times.";
         message += " Args passed are: ";

         foreach (string arg in args)
         {
            message += '"' + arg + "\", ";
         }

         return true;
      }

      public byte[] Save()
      {
         // Example
         MemoryStream exampleStream = new MemoryStream();
         BinaryFormatter serialiser = new BinaryFormatter();
         serialiser.Serialize(exampleStream, this);
         return exampleStream.GetBuffer();
      }

      public void Load(byte[] data)
      {
         // Example
         MemoryStream exampleStream = new MemoryStream(data, false);
         BinaryFormatter deserialiser = new BinaryFormatter();
         count = (deserialiser.Deserialize(exampleStream) as btbCommand).count;
      }
   }
}
