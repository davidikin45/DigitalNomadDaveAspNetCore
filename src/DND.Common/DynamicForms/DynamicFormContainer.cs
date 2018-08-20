using System.Collections.Generic;

namespace DND.Common.DynamicForms
{
    public class DynamicFormContainer
    {
        public int PercentageCompleted { get; set; }
        public DynamicFormNavigation Navigation { get; set; } = new DynamicFormNavigation();
        public List<DynamicForm> Forms { get; set; } = new List<DynamicForm>();
    }
}
