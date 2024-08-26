using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Helsi_TestTask.Models
{
    public class TasksList
    {
        public long Id { get; set; }

        [MaxLength(255)]        
        public required string Name { get; set; }
              
        public required string Owner { get; set; }

        [JsonIgnore]
        [BsonRequired]
        public DateTime Created {  get; set; }

        [JsonIgnore]
        [BsonRequired]
        public DateTime LastUpdate { get; set; }

        public List<Task>? Tasks { get; set; }




        public void CopyFromAnother(TasksList New)
        {
            if (New == null)
            {
                throw new ArgumentNullException(nameof(New));
            }

            
            Name = New.Name;
            Owner = New.Owner;
            Created = New.Created;
            LastUpdate = New.LastUpdate;

            // Deep copy for the Tasks list

            if (New.Tasks != null)
            {
                Tasks = new List<Task>();
                foreach (var task in New.Tasks)
                {
                    Tasks.Add(task);
                }
            }
            else Tasks = null;
        }
    }


    public class Task
    {
        public required string Title { get; set; } 
        public string? Description { get; set; }
        [DefaultValue(false)]
        public bool IsComplete { get; set; }
    }
}
