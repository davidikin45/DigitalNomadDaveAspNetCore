using System.Collections.Generic;

namespace DND.Common.Tasks
{
    public class TaskRunner
    {
  
        public IEnumerable<IRunAfterApplicationConfiguration> RunAfterApplicationConfiguration { get; set; }
        public IEnumerable<IRunOnWebHostStartup> RunOnWebHostStartup { get; set; }
        public IEnumerable<IRunOnEachRequest> RunOnEachRequest { get; set; }   
        public IEnumerable<IRunAfterEachRequest> RunAfterEachRequest { get; set; }     
        public IEnumerable<IRunOnError> RunOnError { get; set; }

        public TaskRunner()
        {
        }

        public void RunTasksAfterApplicationConfiguration()
        {
            foreach (IRunAfterApplicationConfiguration task in RunAfterApplicationConfiguration)
            {
                task.Execute();
            }
        }

        public void RunTasksOnWebHostStartup()
        {
            foreach (IRunOnWebHostStartup task in RunOnWebHostStartup)
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
