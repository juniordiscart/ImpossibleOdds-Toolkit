using System;

namespace ImpossibleOdds
{
    public class ImpossibleOddsException : Exception
    {
        public ImpossibleOddsException()
        {
        }

        public ImpossibleOddsException(string errMsg)
            : base(errMsg)
        {
        }

        public ImpossibleOddsException(string errMsg, Exception innerException)
            : base(errMsg, innerException)
        {
        }

        public ImpossibleOddsException(string errMsg, params object[] format)
            : base(string.Format(errMsg, format))
        {
        }

        public ImpossibleOddsException(Exception innerException, string errMsg)
            : base(errMsg, innerException)
        {
        }
    }
}