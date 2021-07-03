namespace AssignmentService
{
    using System.Collections.Generic;

    public class MongoEntity
    {
        //[BsonId()]
        //public ObjectId _id { get; set; }
    }

    public class User : MongoEntity
    {
        public int Id { get; set; }
        public ICollection<int> GroupIds { get; set; }
        public ICollection<Assignment> Assignments { get; set; }
        public ICollection<FlowAssignment> FlowAssignments { get; set; }
    }

    public class Group : MongoEntity
    {
        public int Id { get; set; }
        public ICollection<Assignment> Assignments { get; set; }
        public ICollection<FlowAssignment> FlowAssignments { get; set; }
    }

    public class Flow : MongoEntity
    {
        public int Id { get; set; }
        public ICollection<int> VideoIds { get; set; }
    }

    public class Video : MongoEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Assignment
    {
        public int VideoId { get; set; }
        public Priority Priority { get; set; }
    }

    public class FlowAssignment
    {
        public int FlowId { get; set; }
        public Priority Priority { get; set; }
    }

    public enum Priority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }
}