using System;

namespace VRShooting.Target
{
    public interface IScorable
    {
        public abstract int Score { get; }
        public abstract string Name { get; }
        public abstract Tuple<int, string> ScoreAndName { get;}
    }
}
