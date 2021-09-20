using System;

namespace ApiCoverageTool.Exceptions
{
    public class InvalidSwaggerJsonException : Exception
    {
        public string InvalidJson { get; init; }

        public InvalidSwaggerJsonException(string message = null, string invalidJson = null) :
            this(message, invalidJson, null)
        { }

        public InvalidSwaggerJsonException(string message, Exception innerException) :
            this(message, null, innerException)
        { }

        public InvalidSwaggerJsonException(string message, string invalidJson, Exception innerException) :
            base(message, innerException)
        {
            InvalidJson = invalidJson;
        }
    }
}
