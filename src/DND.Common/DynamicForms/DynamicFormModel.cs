using DND.Common.Dynamic;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace DND.Common.DynamicForms
{
    public class DynamicFormModel : DynamicTypeDescriptorWrapper, ICustomTypeDescriptor
    {
        public DynamicFormModel()
        {

        }

        public DynamicFormModel(int formId, int sectionId)
        {
            Add("FormId", formId);
            AddAttribute("FormId", new HiddenInputAttribute());

            Add("SectionId", sectionId);
            AddAttribute("SectionId", new HiddenInputAttribute());
        }
    }
}
