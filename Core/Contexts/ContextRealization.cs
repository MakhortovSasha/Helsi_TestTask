using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Helsi_TestTask.Core.Abstractions;
using Helsi_TestTask.Core.Models;

namespace Helsi_TestTask.Models
{
    public partial class MongoContext : DbContext, IDefaultContext
    {
        public DbSet<TasksList> TasksLists { get; set; }
        public DbSet<Sequence> Sequences { get; set; }
        public DbSet<Relation> Relations { get; set; }


        public MongoContext(DbContextOptions<MongoContext> options)
            : base(options)
        {
            Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TasksListConfiguration());
            modelBuilder.ApplyConfiguration(new SequenceConfiguration());
            modelBuilder.ApplyConfiguration(new RelationConfiguration());
            base.OnModelCreating(modelBuilder);
        }



        public class TasksListConfiguration : IEntityTypeConfiguration<TasksList>
        {
            public void Configure(EntityTypeBuilder<TasksList> builder)
            {
                builder.HasKey(p => p.Id);
            }
        }

        public class SequenceConfiguration : IEntityTypeConfiguration<Sequence>
        {
            public void Configure(EntityTypeBuilder<Sequence> builder)
            {
                builder.HasKey(p => p.SequenceName);
            }
        }

        public class RelationConfiguration : IEntityTypeConfiguration<Relation>
        {
            public void Configure(EntityTypeBuilder<Relation> builder)
            {
                builder.HasKey(p => p.Id);
            }
        }

    }




    public partial class MongoContext : DbContext, IDefaultContext
    {

        private long GetNewTasksListID()
        {
            var sequenceForTLs = GetSequence("TaskLists");
            if (sequenceForTLs == null)
                throw new Exception("Unable to get iterator for new tasklist. Sequence unawailable");

            sequenceForTLs.SequenceValue++;

            //might be a conflict if more then one instance is started
            //it is better to place there < SaveChanges(); > call.


            return sequenceForTLs.SequenceValue;

        }

        private Sequence GetSequence(string name)
        {
            var existingSequenceForTLs = Sequences.Find(name);
            if (existingSequenceForTLs == null)
            {
                existingSequenceForTLs = new Sequence()
                {
                    SequenceName = name,
                    SequenceValue = 0
                };

                Sequences.Add(existingSequenceForTLs);
                SaveChanges();
            }
            return existingSequenceForTLs;
        }

        public async Task<Relation> CreateRelation(long id)
        {
            if (Relations.Find(id) != null)
                DeleteRelation(id);

            var returnable = new Relation() { Id = id };
            await Relations.AddAsync(returnable);
            SaveChanges();
            return returnable;
        }
        public async Task<Relation> CreateRelation(long id, IEnumerable<string> recipients)
        {
            if (Relations.Find(id) != null)
                DeleteRelation(id);

            var returnable = new Relation() { Id = id, Recepients = recipients.ToList() };
            await Relations.AddAsync(returnable);
            SaveChanges();
            return returnable;
        }

        public async Task<Relation?> GetRelation(long id)
        {
            return await Relations.FindAsync(id);
        }

        public IQueryable<Relation> GetRelations()
        {
            return Relations.AsQueryable();
        }

        public async Task<bool> UpdateRelation(Relation newRelation)
        {
            var existingItem = GetRelation(newRelation.Id);
            if (existingItem == null)
                return false;

            Relations.Update(newRelation);
            try
            {
                await SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Relations.Any(t => t.Id == newRelation.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;

        }

        public async void DeleteRelation(long id)
        {
            var toDeleteRel = Relations.Find(id);
            if (toDeleteRel != null)
                Relations.Remove(toDeleteRel);
            await SaveChangesAsync();
        }

        public async Task<bool> CreateTasksList(TasksList tasksList)
        {
            tasksList.Id = GetNewTasksListID();

            DeleteRelation(tasksList.Id);///remove orphan 

            tasksList.LastUpdate = tasksList.Created = DateTime.Now;
            TasksLists.Add(tasksList);
            await SaveChangesAsync();

            return true;
        }

        public async Task<TasksList?> GetOneTasksList(long id)
        {
            return await TasksLists.FindAsync(id);
        }

        public IQueryable<TasksList> GetTasksLists()
        {
            return TasksLists.AsQueryable();
        }

        public async Task<bool> UpdateTasksList(TasksList newTasksList)
        {
            var existingItem = TasksLists.Find(newTasksList.Id);
            if (existingItem == null)
                return false;

            existingItem.CopyFromAnother(newTasksList);
            existingItem.LastUpdate = DateTime.UtcNow;
            try
            {
                await SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TasksLists.Any(t => t.Id == newTasksList.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

        public async Task<bool> Delete(TasksList tasksList)
        {

            if (tasksList == null)
                return false;


            DeleteRelation(tasksList.Id);
            TasksLists.Remove(tasksList);


            await SaveChangesAsync();
            return true;

        }
    }
}