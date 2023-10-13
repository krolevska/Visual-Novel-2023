using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public class Character_Text : Character
    {
        public Character_Text(string name) : base(name)
        {
            Debug.Log($"Creates character {name}");
        }
    }
}