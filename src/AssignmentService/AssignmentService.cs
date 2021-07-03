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
                new AssignedVideoResponseModel()
                {
                    Video = "video 6",
                    Priority = "high"
                }
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