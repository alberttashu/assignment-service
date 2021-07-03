namespace AssignmentService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Newtonsoft.Json;

    public static class DataSeeder
    {
        #region SeedModel

        private class SeedData
        {
            [JsonProperty("users")] public ICollection<SeedUser> Users { get; set; }
            [JsonProperty("videos")] public ICollection<SeedVideo> Videos { get; set; }
            [JsonProperty("groups")] public ICollection<SeedGroup> Groups { get; set; }
            [JsonProperty("flows")] public ICollection<SeedFlow> Flows { get; set; }
            [JsonProperty("usersToVideos")] public ICollection<SeedUserToVideo> UsersToVideos { get; set; }
            [JsonProperty("usersToGroups")] public ICollection<SeedUserToGroup> UsersToGroups { get; set; }
            [JsonProperty("usersToFlows")] public ICollection<SeedUserToFlow> UsersToFlows { get; set; }
            [JsonProperty("groupsToVideos")] public ICollection<SeedGroupToVideo> GroupsToVideos { get; set; }
            [JsonProperty("groupsToFlows")] public ICollection<SeedGroupToFlow> GroupsToFlows { get; set; }
            [JsonProperty("flowsToVideos")] public ICollection<SeedFlowToVideo> FlowsToVideos { get; set; }
        }

        private class SeedUser
        {
            public int Id { get; set; }
        }

        private class SeedVideo
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class SeedGroup
        {
            public int Id { get; set; }
        }

        private class SeedFlow
        {
            public int Id { get; set; }
        }

        private class SeedUserToVideo
        {
            public int UserId { get; set; }
            public int VideoId { get; set; }
            public string Priority { get; set; }
        }

        private class SeedUserToGroup
        {
            public int UserId { get; set; }
            public int GroupId { get; set; }
        }

        private class SeedUserToFlow
        {
            public int UserId { get; set; }
            public int FlowId { get; set; }
            public string Priority { get; set; }
        }

        private class SeedGroupToVideo
        {
            public int GroupId { get; set; }
            public int VideoId { get; set; }
            public string Priority { get; set; }
        }

        private class SeedGroupToFlow
        {
            public int GroupId { get; set; }
            public int FlowId { get; set; }
            public string Priority { get; set; }
        }

        private class SeedFlowToVideo
        {
            public int FlowId { get; set; }
            public int VideoId { get; set; }
        }

        #endregion

        public static async Task SeedAsync(IMongoDatabase db, string configSeedFilePath)
        {
            var seedFileContent = await File.ReadAllTextAsync(configSeedFilePath);
            var seedData = JsonConvert.DeserializeObject<SeedData>(seedFileContent);

            if (seedData == null)
            {
                throw new Exception("Failed to deserialize seed data file");
            }

            var users = new Dictionary<int, User>();
            var flows = new Dictionary<int, Flow>();
            var groups = new Dictionary<int, Group>();
            var videos = new Dictionary<int, Video>();

            foreach (var seedUser in seedData.Users)
            {
                users[seedUser.Id] = new User()
                {
                    Id = seedUser.Id,
                    Assignments = new List<Assignment>(),
                    FlowAssignments = new List<FlowAssignment>(),
                    GroupIds = new List<int>()
                };
            }

            foreach (var seedFlow in seedData.Flows)
            {
                flows[seedFlow.Id] = new Flow()
                {
                    Id = seedFlow.Id,
                    VideoIds = new List<int>()
                };
            }

            foreach (var seedGroup in seedData.Groups)
            {
                groups[seedGroup.Id] = new Group()
                {
                    Id = seedGroup.Id,
                    Assignments = new List<Assignment>(),
                    FlowAssignments = new List<FlowAssignment>()
                };
            }

            foreach (var seedVideo in seedData.Videos)
            {
                videos[seedVideo.Id] = new Video()
                {
                    Id = seedVideo.Id,
                    Name = seedVideo.Name
                };
            }

            foreach (var userToVideo in seedData.UsersToVideos)
            {
                users[userToVideo.UserId].Assignments
                    .Add(new Assignment()
                    {
                        Priority = ParsePriority(userToVideo.Priority),
                        VideoId = userToVideo.VideoId
                    });
            }

            foreach (var userToGroup in seedData.UsersToGroups)
            {
                users[userToGroup.UserId].GroupIds
                    .Add(userToGroup.GroupId);
            }

            foreach (var userToFlow in seedData.UsersToFlows)
            {
                users[userToFlow.UserId].FlowAssignments
                    .Add(new FlowAssignment()
                    {
                        Priority = ParsePriority(userToFlow.Priority),
                        FlowId = userToFlow.FlowId
                    });
            }

            foreach (var groupToVideo in seedData.GroupsToVideos)
            {
                groups[groupToVideo.GroupId].Assignments
                    .Add(new Assignment()
                    {
                        Priority = ParsePriority(groupToVideo.Priority),
                        VideoId = groupToVideo.VideoId
                    });
            }

            foreach (var groupsToFlow in seedData.GroupsToFlows)
            {
                groups[groupsToFlow.GroupId].Assignments
                    .Add(new Assignment()
                    {
                        Priority = ParsePriority(groupsToFlow.Priority),
                        VideoId = groupsToFlow.FlowId
                    });
            }

            foreach (var flowToVideo in seedData.FlowsToVideos)
            {
                flows[flowToVideo.FlowId].VideoIds
                    .Add(flowToVideo.VideoId);
            }
            
            await Task.WhenAll(
                db.DropCollectionAsync(Constants.UsersCollectionName),
                db.DropCollectionAsync(Constants.GroupsCollectionName),
                db.DropCollectionAsync(Constants.FlowsCollectionName),
                db.DropCollectionAsync(Constants.VideosCollectionName)
            );
            
            var usersCollection = db.GetCollection<User>(Constants.UsersCollectionName);
            var groupsCollection = db.GetCollection<Group>(Constants.GroupsCollectionName);
            var flowsCollection = db.GetCollection<Flow>(Constants.FlowsCollectionName);
            var videosCollection = db.GetCollection<Video>(Constants.VideosCollectionName);

            await Task.WhenAll(
                usersCollection.InsertManyAsync(users.Select(x => x.Value).ToList()),
                groupsCollection.InsertManyAsync(groups.Select(x => x.Value).ToList()),
                flowsCollection.InsertManyAsync(flows.Select(x => x.Value).ToList()),
                videosCollection.InsertManyAsync(videos.Select(x => x.Value).ToList())
            );
        }

        private static Priority ParsePriority(string priority)
        {
            return priority switch
            {
                "low" => Priority.Low,
                "medium" => Priority.Medium,
                "high" => Priority.High,
                "critical" => Priority.Critical,
                _ => throw new Exception("Unexpected seed priority")
            };
        }
    }
}