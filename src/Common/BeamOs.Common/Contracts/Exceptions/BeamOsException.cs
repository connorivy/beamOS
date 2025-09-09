namespace BeamOs.Common.Contracts.Exceptions;

public class BeamOsException(string message, Exception? innerException = null)
    : Exception(message, innerException) { }
