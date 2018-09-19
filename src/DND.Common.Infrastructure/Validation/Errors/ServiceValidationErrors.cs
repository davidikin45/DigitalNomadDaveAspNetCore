namespace DND.Common.Infrastructure.Validation.Errors
{
    public class ServiceValidationErrors : ValidationErrors
    {
        public ServiceValidationErrors() : base()
        {
           
        }

        public ServiceValidationErrors(IError error) : base(error)
        {
           
        }
    }
}
