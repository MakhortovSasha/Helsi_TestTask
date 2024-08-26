using Helsi_TestTask.Models.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Helsi_TestTask.Models
{
    public class APIRealization : IDefaultAPI
    {
        public IDefaultContext Context { get; }
        public APIRealization(IDefaultContext context)
        {
            Context = context;
        }
        //{
        //    return _context.TasksLists.Any(e => e.Id == id);
        //}
        private bool CheckReadAccess(string requester, long id)
        {

            if(! CheckModifyAccess(requester, id))
            {
                var exists = Context.GetRelations().Any(r => r.Id == id || r.Recepients != null && r.Recepients.Contains(requester));
                if(!exists) { return false; }
            }
            return true;
        }

        private bool CheckModifyAccess(string requester, long id)
        {
            if (!CheckDeleteAccess(requester, id))
            {
                var exists = Context.GetRelations().Any(r => r.Id == id || r.Recepients != null && r.Recepients.Contains(requester));
                if (!exists) { return false; }
            }
            return true;
        }

        private bool CheckModifyAccess(string requester, TasksList tasksList)
        {
            if (!CheckDeleteAccess(requester, tasksList))
            {
                var exists = Context.GetRelations().Any(r => r.Id == tasksList.Id || r.Recepients != null && r.Recepients.Contains(requester));
                if (!exists) { return false; }
            }
            return true;
        }

        private bool CheckDeleteAccess(string requester, long id)
        {
            Task<TasksList?> task = Context.GetOneTasksList(id);
            task.Wait();
            var tasklist = task.Result;
            if (tasklist!=null && tasklist.Owner == requester)
                return true;
            return false;
        }

        private bool CheckDeleteAccess(string requester, TasksList tasksList)
        {
            if (tasksList != null && tasksList.Owner == requester)
                return true;
            return false;
        }

        public async Task<bool> CreateTaskList(string owner, TasksList tasksList)
        {
            tasksList.Owner = owner;
            return await Context.CreateTasksList(tasksList);
        }

        public async Task<TasksList?> GetTasksList(string requester, long id)
        {
            // Check access
            if (CheckReadAccess(requester, id))

                return await Context.GetOneTasksList(id);
            return null;

        }

        public async Task<List<TasksList>> GetOwnedTaskLists(string requester, int? page, string? orderby)
        {

            int position = page ?? 1;
            if (position <= 0)
                position = 1;
            orderby = orderby ?? string.Empty;

          //var readableTLsIDs = await Context.GetRelations().Where(r => r.Recepients != null && r.Recepients.Contains(requester)).AsNoTracking().Select(r => r.Id).ToListAsync(); doesn't need asnotracking in case of <Select>

            var readableTLsIDs = await Context.GetRelations().Where(r => r.Recepients != null && r.Recepients.Contains(requester)).Select(r => r.Id).ToListAsync();

            var everyEnabledTL = Context.GetTasksLists().Where(tl => tl.Owner == requester || readableTLsIDs.Contains(tl.Id));
            List<TasksList> returnableCollection;

            switch (orderby.ToLower().Trim())
            {
                case "created":
                    everyEnabledTL =  everyEnabledTL.OrderBy(b => b.Created);
                    break;
                case "updated":
                    everyEnabledTL =  everyEnabledTL.OrderBy(b => b.LastUpdate);
                    break;
                case "id":
                    everyEnabledTL =  everyEnabledTL.OrderBy(b => b.Id);
                    break;
                default:
                    break;
            }

            returnableCollection = await everyEnabledTL.Skip((position - 1) * 10).Take(10).AsNoTracking().ToListAsync();
            return returnableCollection;
        }

        public async Task<bool> UpdateTaskList(string requester, long id, TasksList newTasksList)
        {
            var existingTask = await Context.GetOneTasksList(id);
            if (existingTask == null || !CheckModifyAccess(requester, existingTask))
                return false;
            return await Context.UpdateTasksList(newTasksList);


        }

        public async Task<bool> DeleteTaskList(string requester, long id)
        {
            var toDelete = await Context.GetOneTasksList(id);

            if(toDelete== null || !CheckDeleteAccess(requester, id)) return false;
            return await Context.Delete(toDelete);

        }

        public async Task<bool> AddRecepient(string requester, long id, string newRecepient)
        {
            return await AddRecepientsBulk(requester, new List<string>() { newRecepient }, id);
        }

        public async Task<bool> AddRecepientsBulk(string requester, IEnumerable<string> newRecepients, long id)
        {
            if (!CheckModifyAccess(requester, id)) return false;

            var relation = await Context.GetRelation(id);
            if (relation == null) relation = await Context.CreateRelation(id, newRecepients);

            if (relation.Recepients == null)
                relation.Recepients = new List<string>();
            foreach(var r  in newRecepients)
                if(!relation.Recepients.Contains(r))
                    relation.Recepients.Add(r);
            
            return await Context.UpdateRelation(relation);
        }

        public async Task<bool> RemoveRecepient(string requester, long id, string toDeleteRecepient)
        {
            return await RemoveRecepientsBulk(requester, new List<string>() { toDeleteRecepient }, id);
        }

        public async Task<bool> RemoveRecepientsBulk(string requester, IEnumerable<string> toDeleteRecepients, long id)
        {
            if (CheckModifyAccess(requester, id))
            {
                var relation = await Context.GetRelation(id);
                if (relation != null && relation.Recepients != null && relation.Recepients.Count > 0)
                {
                    for (int i = relation.Recepients.Count - 1; i >= 0; i--)
                        if (toDeleteRecepients.Contains(relation.Recepients[i]))
                            relation.Recepients.RemoveAt(i);
                    await Context.UpdateRelation(relation);
                }
                return true;
            }
            return false;
        }
        public async Task<Relation?> GetRelation(string requester, long id)
        {
            if (CheckReadAccess(requester, id))
                return await Context.GetRelation(id);

            return null;
        }


        public async Task<bool> TasksListExists(string userid, string Name)
        {
            var existing =await Context.GetTasksLists().FirstAsync(x => x.Owner == userid && x.Name == Name);
            if (existing==null)return false;

            return true;
        }
    }
}
