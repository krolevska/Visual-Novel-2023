using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Dialoque;

namespace Commands
{
    public class CommandDataBase
    {
        private Dictionary<string, Delegate> database = new Dictionary<string, Delegate>();
        public bool HasCommand(string commandName) => database.ContainsKey(commandName);

        public void AddCommand(string commandName, Delegate command)
        {

            if (!database.ContainsKey(commandName))
            {
                database.Add(commandName, command);
            }
            //else
                //Debug.LogError($"Command already exists in the database '{commandName}'");

        }

        public Delegate GetCommand(string commandName)
        {

            if (!database.ContainsKey(commandName))
            {
                //Debug.LogError($"Command '{commandName}' doesn't exint in the database!");
                return null;
            }
            return database[commandName];

        }
    }
}