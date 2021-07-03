namespace AssignmentService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    public enum PriorityOrder
    {
        Desc = 1,
        Asc = 2
    }
    
    public class AssignmentService
    {
        #region DBQueries

        private readonly string _getAssignmetnsQuery = 
                @"
[
    {
        ""$lookup"": {
            ""from"": ""Groups"",
            ""localField"": ""GroupIds"",
            ""foreignField"": ""_id"",
            ""as"": ""Groups""
        }
    },
    { ""$unwind"": { path: ""$FlowAssignments"", preserveNullAndEmptyArrays: true } },
    {
        ""$lookup"": {
            ""from"": ""Flows"",
            ""localField"": ""FlowAssignments.FlowId"",
            ""foreignField"": ""_id"",
            ""as"": ""FlowAssignments.Flow""
        }
    },
    { ""$unwind"": { path: ""$Groups"", preserveNullAndEmptyArrays: true } },
    { ""$unwind"": { path: ""$Groups.FlowAssignments"", preserveNullAndEmptyArrays: true } },
    {
        ""$lookup"": {
            ""from"": ""Flows"",
            ""localField"": ""Groups.FlowAssignments.FlowId"",
            ""foreignField"": ""_id"",
            ""as"": ""Groups.FlowAssignments.Flow""
        }
    },
    { ""$unwind"": { path: ""$Groups.FlowAssignments.Flow"", preserveNullAndEmptyArrays: true } },
    { ""$unwind"": { path: ""$Groups.Assignments"", preserveNullAndEmptyArrays: true } },
    { ""$unwind"": { path: ""$Assignments"", preserveNullAndEmptyArrays: true } },
    {
        ""$lookup"": {
            ""from"": ""Videos"",
            ""localField"": ""Assignments.VideoId"",
            ""foreignField"": ""_id"",
            ""as"": ""Assignments.Video""
        }
    },
    { ""$unwind"": { path: ""$Assignments.Video"", preserveNullAndEmptyArrays: true } },
    { ""$unwind"": { path: ""$FlowAssignments.Flow"", preserveNullAndEmptyArrays: true } },
    { ""$unwind"": { path: ""$FlowAssignments.Flow.VideoIds"", preserveNullAndEmptyArrays: true } },
    {
        ""$lookup"": {
            ""from"": ""Videos"",
            ""localField"": ""FlowAssignments.Flow.VideoIds"",
            ""foreignField"": ""_id"",
            ""as"": ""FlowAssignments.Flow.Video""
        }
    },
    { ""$unwind"": { path: ""$FlowAssignments.Flow.Video"", preserveNullAndEmptyArrays: true } },
    { ""$unwind"": { path: ""$Groups.FlowAssignments.Flow.VideoIds"", preserveNullAndEmptyArrays: true } },
    {
        ""$lookup"": {
            ""from"": ""Videos"",
            ""localField"": ""Groups.FlowAssignments.Flow.VideoIds"",
            ""foreignField"": ""_id"",
            ""as"": ""Groups.FlowAssignments.Flow.Video""
        }
    },
    { ""$unwind"": { path: ""$Groups.FlowAssignments.Flow.Video"", preserveNullAndEmptyArrays: true } },
    {
        ""$lookup"": {
            ""from"": ""Videos"",
            ""localField"": ""Groups.Assignments.VideoId"",
            ""foreignField"": ""_id"",
            ""as"": ""Groups.Assignments.Video""
        }
    },
    { ""$unwind"": { path: ""$Groups.Assignments.Video"", preserveNullAndEmptyArrays: true } },
    {
        ""$project"":
        {
            ""_id"" : ""$_id"",
            ""Assignments"" : { ""$cond"": { ""if"": { ""$eq"": ['$Assignments', {}] }, ""then"": null, ""else"": '$Assignments' } },
            ""FlowAssignments"" : { ""$cond"": { ""if"": { ""$eq"": ['$FlowAssignments', {}] }, ""then"": null, ""else"": '$FlowAssignments' } },
            ""Groups.Assignments"" : { ""$cond"": { ""if"": { ""$eq"": ['$Groups.Assignments', {}] }, ""then"": null, ""else"": '$Groups.Assignments' } },
            ""Groups.FlowAssignments"" : { ""$cond"": { ""if"": { ""$eq"": ['$Groups.FlowAssignments.Flow', {}] }, ""then"": null, ""else"": '$Groups.FlowAssignments' } }
        }
    },
    {
        ""$group"": {
            ""_id"": ""$_id"",
            ""Assignments"": {""$addToSet"" : ""$Assignments""},
            ""FlowAssignments"": {""$addToSet"": ""$FlowAssignments""},
            ""GroupAssignments"": {""$addToSet"": ""$Groups.Assignments""},
            ""GroupFlowAssignments"": {""$addToSet"": ""$Groups.FlowAssignments""},
        }
    },
    {
        ""$project"": {
            ""_id"": ""$_id"",
            ""Assignments"": { ""$setDifference"": [ ""$Assignments"", [null] ] },
            ""FlowAssignments"": { ""$setDifference"": [ ""$FlowAssignments"", [null] ] },
            ""GroupAssignments"": { ""$setDifference"": [ ""$GroupAssignments"", [null] ] },
            ""GroupFlowAssignments"": { ""$setDifference"": [ ""$GroupFlowAssignments"", [null] ] }

        }
    }
]
";
        
        

        #endregion
        
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Group> _groupsCollection;
        private readonly IMongoCollection<Flow> _flowsCollection;
        private readonly IMongoCollection<Video> _videosCollection;

        public AssignmentService(IMongoDatabase db)
        {
            _usersCollection = db.GetCollection<User>(Constants.UsersCollectionName);
            _groupsCollection = db.GetCollection<Group>(Constants.GroupsCollectionName);
            _flowsCollection = db.GetCollection<Flow>(Constants.FlowsCollectionName);
            _videosCollection = db.GetCollection<Video>(Constants.VideosCollectionName);
        }
        
        public async Task<IReadOnlyList<AssignedVideoResponseModel>> GetAllAssignedContent(int userId, string priorityOrder = "desc")
        {
            var order = Mapper.ToPriorityOrder(priorityOrder);

            var query = BsonSerializer.Deserialize<BsonDocument[]>(_getAssignmetnsQuery)
                .Append(new BsonDocument { { "$match", new BsonDocument("_id", userId) } })
                .ToList();
            using var cursor = _usersCollection.Aggregate<GetAssignmentsReadModel>(query);
            var getAssignmentsResult = await cursor.FirstAsync();


            var assignments = new List<(Priority Priority, string Video)>();

            foreach (var item in getAssignmentsResult.Assignments)
            {
                assignments.Add((item.Priority, item.Video.Name));
            }
            foreach (var item in getAssignmentsResult.FlowAssignments)
            {
                assignments.Add((item.Priority, item.Flow.Video.Name));
            }
            foreach (var item in getAssignmentsResult.GroupAssignments)
            {
                assignments.Add((item.Priority, item.Video.Name));
            }
            foreach (var item in getAssignmentsResult.GroupFlowAssignments)
            {
                assignments.Add((item.Priority, item.Flow.Video.Name));
            }

            var result = assignments.GroupBy(x => x.Video)
                .Select(group => (Video: group.Key, Priority: group.Max(x => x.Priority)));

            result = order switch
            {
                PriorityOrder.Asc => result.OrderBy(x => x.Priority),
                _ => result.OrderByDescending(x => x.Priority)
            };

            return result.Select(x => new AssignedVideoResponseModel()
            {
                Priority = Mapper.ToPriorityViewModel(x.Priority),
                Video = x.Video
            }).ToList();
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