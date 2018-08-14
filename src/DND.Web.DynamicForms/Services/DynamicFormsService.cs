using DND.Interfaces.DynamicForms.ApplicationServices;

namespace DND.Web.DynamicForms.Services
{
    public interface IDynamicFormsSerivce
    {

    }

    public class DynamicFormsService
    {
        private readonly IFormApplicationService _formApplicationSerivce;
        private readonly ILookupTableApplicationService _lookupTableApplicationSerivce;

        private readonly IFormSubmissionApplicationService _formSubmissionApplicationService;
        private readonly IFormSectionSubmissionApplicationService _formSectionSubmissionApplicationService;

        public DynamicFormsService(IFormApplicationService formApplicationSerivce, IFormSubmissionApplicationService formSubmissionApplicationService, IFormSectionSubmissionApplicationService formSectionSubmissionApplicationService, ILookupTableApplicationService lookupTableApplicationService)
        {
            _formApplicationSerivce = formApplicationSerivce;
            _lookupTableApplicationSerivce = lookupTableApplicationService;

            _formSubmissionApplicationService = formSubmissionApplicationService;
            _formSectionSubmissionApplicationService = formSectionSubmissionApplicationService;
        }
    }
}
