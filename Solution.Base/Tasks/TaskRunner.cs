using System.Collections.Generic;

namespace Solution.Base.Tasks
{
    public class TaskRunner
    {
  
        public IEnumerable<IRunAtStartup> RunAtStartup { get; set; }
      
        public IEnumerable<IRunOnEachRequest> RunOnEachRequest { get; set; }
    
        public IEnumerable<IRunAfterEachRequest> RunAfterEachRequest { get; set; }
       
        public IEnumerable<IRunOnError> RunOnError { get; set; }

        public TaskRunner()
        {
        }

        public void RunTasksAtStartup()
        {
            foreach (IRunAtStartup task in RunAtStartup)
            {
                task.Execute();
            }
        }

        public void RunTasksOnEachRequest()
        {
            foreach (IRunOnEachRequest task in RunOnEachRequest)
            {
                task.Execute();
            }
        }

        public void RunTasksAfterEachRequest()
        {
            foreach (IRunAfterEachRequest task in RunAfterEachRequest)
            {
                task.Execute();
            }
        }

        public void RunTasksOnError()
        {
            foreach (IRunOnError task in RunOnError)
            {
                task.Execute();
            }
        }

    }
}
