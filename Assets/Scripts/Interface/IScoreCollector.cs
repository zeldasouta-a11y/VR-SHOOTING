using System;
using UnityEngine;

namespace VRShooting.Player
{
    public interface IScoreCollector
    {
        public abstract void GetScoreAndName(Tuple<int, string> score);
    };
}

