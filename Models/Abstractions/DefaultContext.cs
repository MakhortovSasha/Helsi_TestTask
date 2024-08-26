using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helsi_TestTask.Models.Abstractions
{
    public interface IDefaultContext
    {

        public Task<bool> CreateTasksList(TasksList tasksList);
        public Task<bool> UpdateTasksList(TasksList newTasksList);
        public Task<bool> Delete(TasksList tasksList);
        public IQueryable<TasksList> GetTasksLists();
        public Task<TasksList?> GetOneTasksList(long id);


        public Task<Relation> CreateRelation(long id);
        public Task<Relation> CreateRelation(long id, IEnumerable<string> recepients);
        public Task<bool> UpdateRelation(Relation newRelation);
        public void DeleteRelation(long id);
        public IQueryable<Relation> GetRelations();
        public Task<Relation?> GetRelation(long id);
    }
}
