using Microsoft.EntityFrameworkCore;

namespace Helsi_TestTask.Models.Abstractions
{

    public interface DefaultAPI
    {


        public Task<bool> CreateTaskList(string owner, TasksList tasksList);
        public Task<TasksList?> GetTasksList(string requester, long id);
        public Task<List<TasksList>> GetOwnedTaskLists(string requester, int? page, string? orderby );
        public Task<bool> UpdateTaskList(string requester, long id, TasksList tasksList);
        public Task<bool> DeleteTaskList(string requester, long id);
        public Task<bool> AddRecepient(string requester, long id, string newRecepient);
        public Task<bool> RemoveRecepient(string requester, long id, string recepient);
        public Task<bool> AddRecepientsBulk(string requester, IEnumerable<string> newRecepients, long id);
        public Task<bool> RemoveRecepientsBulk(string requester, IEnumerable<string> newRecepients, long id);
        public Task<Relation?> GetRelation(string requester, long id);

        public Task<bool> TasksListExists(string userid, string Name);

    }

    
}
