namespace DND.Common.Infrastructure.Validation
{
    public enum ErrorType
    {
        UnknownError,
        ObjectDoesNotExist,
        ObjectValidationFailed,
        ConcurrencyConflict,
        EmailSendFailed,
        Unauthorized
    }
}
