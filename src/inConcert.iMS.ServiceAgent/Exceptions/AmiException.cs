using System;

namespace inConcert.iMS.ServiceAgent.Exceptions
{
    public class AmiException : Exception
    {
        public AmiException(string message)
            : base(message)
        {
        }
    }
}
