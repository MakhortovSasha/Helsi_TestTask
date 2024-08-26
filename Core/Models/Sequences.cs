using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Helsi_TestTask.Core.Models
{
    public class Sequence
    {

        [BsonElement("_id")]
        public string SequenceName { get; set; }

        public long SequenceValue { get; set; }
    }
}
