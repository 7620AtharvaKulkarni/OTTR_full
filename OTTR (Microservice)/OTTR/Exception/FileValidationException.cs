using System;

public class FileValidationException: Exception
{
    private Cause causedBy;

    public FileValidationException(Cause causedBy, string message) : base(message)
    {
        this.causedBy = causedBy;
    }

    public FileValidationException(Cause causedBy, string message, Exception innerException) : base(message, innerException)
    {
        this.causedBy = causedBy;
    }

    public enum Cause
    {
        frontend,
        backend
    }
}