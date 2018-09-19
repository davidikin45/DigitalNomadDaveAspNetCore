namespace DND.Common.Infrastructure.Validation.Errors
{
    public class PropertyError : IError
    {
        public string PropertyName { get; set; }
        public string PropertyExceptionMessage { get; set; }
        public PropertyError(string propertyName, string errorMessage)
        {
            this.PropertyName = propertyName;
            this.PropertyExceptionMessage = errorMessage;
        }
    }
}
