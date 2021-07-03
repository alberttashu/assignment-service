namespace AssignmentService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class AssignmentService
    {

        public async Task<IReadOnlyList<AssignedVideoResponseModel>> GetAllAssignedContent(int userId, string priorityOrder = "desc")
        {
            return new List<AssignedVideoResponseModel>()
            {
                new() {Video = "video 5", Priority = "high"},
                new() {Video = "video 6", Priority = "high"},
                new() {Video = "video 7", Priority = "high"},
                new() {Video = "video 8", Priority = "medium"}
            };
        }
    }

    public class AssignedVideoResponseModel
    {
        // video name
        public string Video { get; set; }

        // priority
        public string Priority { get; set; }
    }
}