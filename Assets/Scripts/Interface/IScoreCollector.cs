using System;
using UnityEngine;

namespace VRShooting.Player
{
    public interface IScoreCollector
    {
        public String Name { get; } 
        public abstract void GetScoreAndName(Tuple<int, string> score);
    };
}

