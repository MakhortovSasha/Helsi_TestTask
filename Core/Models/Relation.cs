﻿using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Helsi_TestTask.Core.Models
{
    public class Relation
    {
        public long Id { get; set; }
        public List<string>? Recepients { get; set; }
    }
}
