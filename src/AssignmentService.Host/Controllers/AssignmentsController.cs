namespace AssignmentService.Host.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly AssignmentService _service;

        public AssignmentsController(AssignmentService service)
        {
            _service = service;
        }

        [Route("/users/{userId:int}/videos")]
        public async Task<IReadOnlyList<AssignedVideoResponseModel>> GetAllAssignedContent(int userId, [FromQuery(Name = "priority")]string? priorityOrder = null)
        {
            return await _service.GetAllAssignedContent(userId, priorityOrder);
        }


    }
}