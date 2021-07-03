namespace AssignmentService
{
    using System;

    public static class Mapper
    {
        public static PriorityOrder ToPriorityOrder(string priorityOrder)
        {
            return priorityOrder switch
            {
                "desc" => PriorityOrder.Desc,
                "asc" => PriorityOrder.Asc,
                null => PriorityOrder.Desc, 
                _ => throw new Exception("Unexpected priority order")
            };
        }
        
        public static Priority ToPriority(string priority)
        {
            return priority switch
            {
                "low" => Priority.Low,
                "medium" => Priority.Medium,
                "high" => Priority.High,
                "critical" => Priority.Critical,
                _ => throw new Exception("Unexpected priority")
            };
        }
        
        public static string ToPriorityViewModel(Priority priority)
        {
            return priority switch
            {
                Priority.Low => "low",
                Priority.Medium => "medium",
                Priority.High => "high",
                Priority.Critical => "critical",
                _ => throw new Exception("Unexpected priority")
            };
        }
    }
}