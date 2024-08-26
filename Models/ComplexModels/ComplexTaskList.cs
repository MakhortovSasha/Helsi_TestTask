using System.ComponentModel;

namespace Helsi_TestTask.Models.ComplexModels
{
    public class ComplexTaskList
    {

        public TasksList TaskList { get; set; }


        /// <summary>
        /// cannot be retrieved from the request in case of auth lack
        /// </summary>
        public string userid { get; set; } 
    }
}
