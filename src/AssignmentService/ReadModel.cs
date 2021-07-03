namespace AssignmentService
{
    using System.Collections.Generic;

    public class GetAssignmentsReadModel
    {
        public ICollection<AssignmentReadModel> Assignments { get; set; }
        public ICollection<FlowAssignmentReadModel> FlowAssignments { get; set; }
        public ICollection<GroupAssignmentReadModel> GroupAssignments { get; set; }
        public ICollection<GroupFlowAssignmentReadModel> GroupFlowAssignments { get; set; }
    }

    public class GroupFlowAssignmentReadModel
    {
        public Priority Priority { get; set; }
        public FlowReadModel Flow { get; set; }
    }

    public class VideoReadModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class GroupAssignmentReadModel
    {
        public Priority Priority { get; set; }
        public VideoReadModel Video { get; set; }
    }

    public class FlowAssignmentReadModel
    {
        public Priority Priority { get; set; }
        public FlowReadModel Flow { get; set; }
    }

    public class FlowReadModel
    {
        public int Id { get; set; }
        public VideoReadModel Video { get; set; }
    }

    public class AssignmentReadModel
    {
        public Priority Priority { get; set; }
        public VideoReadModel Video { get; set; }
    }
}