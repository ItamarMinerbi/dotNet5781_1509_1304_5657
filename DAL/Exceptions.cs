using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DalExceptions
{
    #region Bus Exceptions
    class BusDoesNotExistException : Exception
    {
        public BusDoesNotExistException() : base() { }
        public BusDoesNotExistException(string message) : base(message) { }
        public BusDoesNotExistException(string message, Exception innerException) 
            : base(message, innerException) { }
        public BusDoesNotExistException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }

    class BusAlreadyExistException : Exception
    {
        public BusAlreadyExistException() : base() { }
        public BusAlreadyExistException(string message) : base(message) { }
        public BusAlreadyExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public BusAlreadyExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
    #endregion

    #region Station Exceptions
    public class StationDoesNotExistException : Exception
    {
        public StationDoesNotExistException() : base() { }
        public StationDoesNotExistException(string message) : base(message) { }
        public StationDoesNotExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public StationDoesNotExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    public class StationAlreadyExistException : Exception
    {
        public StationAlreadyExistException() : base() { }
        public StationAlreadyExistException(string message) : base(message) { }
        public StationAlreadyExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public StationAlreadyExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
    #endregion

    #region StationLine Exceptions
    public class StationLineDoesNotExistException : Exception
    {
        public StationLineDoesNotExistException() : base() { }
        public StationLineDoesNotExistException(string message) : base(message) { }
        public StationLineDoesNotExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public StationLineDoesNotExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    public class StationLineAlreadyExistException : Exception
    {
        public StationLineAlreadyExistException() : base() { }
        public StationLineAlreadyExistException(string message) : base(message) { }
        public StationLineAlreadyExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public StationLineAlreadyExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
    #endregion

    #region Line Exceptions
    public class LineDoesNotExistException : Exception
    {
        public LineDoesNotExistException() : base() { }
        public LineDoesNotExistException(string message) : base(message) { }
        public LineDoesNotExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public LineDoesNotExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    public class LineAlreadyExistException : Exception
    {
        public LineAlreadyExistException() : base() { }
        public LineAlreadyExistException(string message) : base(message) { }
        public LineAlreadyExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public LineAlreadyExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
    #endregion

    #region User Exceptions
    public class UserDoesNotExistException : Exception
    {
        public UserDoesNotExistException() : base() { }
        public UserDoesNotExistException(string message) : base(message) { }
        public UserDoesNotExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public UserDoesNotExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    public class UserAlreadyExistException : Exception
    {
        public UserAlreadyExistException() : base() { }
        public UserAlreadyExistException(string message) : base(message) { }
        public UserAlreadyExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public UserAlreadyExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
    #endregion

    #region AdjStations Exceptions
    public class AdjStationsDoesNotExistException : Exception
    {
        public AdjStationsDoesNotExistException() : base() { }
        public AdjStationsDoesNotExistException(string message) : base(message) { }
        public AdjStationsDoesNotExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public AdjStationsDoesNotExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    public class AdjStationsAlreadyExistException : Exception
    {
        public AdjStationsAlreadyExistException() : base() { }
        public AdjStationsAlreadyExistException(string message) : base(message) { }
        public AdjStationsAlreadyExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public AdjStationsAlreadyExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
    #endregion

    #region LineTrip Exceptions
    public class LineTripDoesNotExistException : Exception
    {
        public LineTripDoesNotExistException() : base() { }
        public LineTripDoesNotExistException(string message) : base(message) { }
        public LineTripDoesNotExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public LineTripDoesNotExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    public class LineTripAlreadyExistException : Exception
    {
        public LineTripAlreadyExistException() : base() { }
        public LineTripAlreadyExistException(string message) : base(message) { }
        public LineTripAlreadyExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public LineTripAlreadyExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
    #endregion
}