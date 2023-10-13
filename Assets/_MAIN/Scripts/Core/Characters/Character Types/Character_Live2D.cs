using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public class Character_Live2D : Character
    {
        public Character_Live2D(string name) : base(name)
        {
            Debug.Log($"Creates character {name}");
        }
    }
}