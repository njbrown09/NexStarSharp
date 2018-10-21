using System;

namespace NexStarSharp.Exceptions
{
    public class NexStarException : Exception
    {
        public NexStarException(string message) : base(message)
        {
            message = "NEXSTAR EXCEPTION: " + message;
        }
    }
}