namespace DND.Common.Infrastructure.Validation.Errors
{
    public interface IError
    {
        string PropertyName { get; }
        string PropertyExceptionMessage { get; }
    }
}
