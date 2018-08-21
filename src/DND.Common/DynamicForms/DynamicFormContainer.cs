using System.Collections.Generic;

namespace DND.Common.DynamicForms
{
    public class DynamicFormContainer
    {
        public DynamicFormNavigation Navigation { get; set; } = new DynamicFormNavigation();
        public List<DynamicForm> Forms { get; set; } = new List<DynamicForm>();
    }
}
