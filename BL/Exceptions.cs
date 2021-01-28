using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BlExceptions
{
    #region An Error Occur Exception
    public class AnErrorOccurredException : Exception
    {
        public object Argument { get; set; }
        public AnErrorOccurredException() : base() { }
        public AnErrorOccurredException(string message) : base(message) { }
        public AnErrorOccurredException(string message, Exception innerException)
            : base(message, innerException) { }
        public AnErrorOccurredException(SerializationInfo info, StreamingContext context)
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

    public class InvalidUsernameException : Exception
    {
        public InvalidUsernameException() : base() { }
        public InvalidUsernameException(string message) : base(message) { }
        public InvalidUsernameException(string message, Exception innerException)
            : base(message, innerException) { }
        public InvalidUsernameException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException() : base() { }
        public InvalidPasswordException(string message) : base(message) { }
        public InvalidPasswordException(string message, Exception innerException)
            : base(message, innerException) { }
        public InvalidPasswordException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    public class InvalidEmailException : Exception
    {
        public InvalidEmailException() : base() { }
        public InvalidEmailException(string message) : base(message) { }
        public InvalidEmailException(string message, Exception innerException)
            : base(message, innerException) { }
        public InvalidEmailException(SerializationInfo info, StreamingContext context)
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

    #region LineTrip Exceptions
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