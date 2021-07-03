namespace AssignmentService.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Mongo2Go;
    using MongoDB.Bson.Serialization.Conventions;
    using MongoDB.Driver;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    [TestFixture]
    public class AssignmentServiceTests
    {
        private MongoDbRunner _runner;
        private MongoClient _client;
        private IMongoDatabase _db;
        private AssignmentService _service;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _runner = MongoDbRunner.Start();
            _client = new MongoClient(_runner.ConnectionString);
            _db = _client.GetDatabase(Constants.AssignmentsDbName);

            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);
            
            await DataSeeder.SeedAsync(_db, "./seed/Data.json");

            _service = new AssignmentService(_db);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _runner.Dispose();
        }

        [Test]
        public async Task GetAllAssignedContent_ForUser5_ShouldReturnCorrectItems()
        {
            // arrange
            var userId = 5;
            var expected = new List<AssignedVideoResponseModel>()
            {
                new() {Video = "Video 5", Priority = "high"},
                new() {Video = "Video 6", Priority = "high"},
                new() {Video = "Video 7", Priority = "high"},
                new() {Video = "Video 8", Priority = "medium"}
            };

            // act 
            var actual = await _service.GetAllAssignedContent(userId);

            // assert
            AssertCollectionEquivalent(expected, actual, (ex, ac) =>
                ex.Video == ac.Video && ex.Priority == ac.Priority);
        }

        private static void AssertCollectionEquivalent<TElement>(IEnumerable<TElement> expected,
            IEnumerable<TElement> actual, Func<TElement, TElement, bool> comparer)
        {
            Assert.That(actual,
                new CollectionEquivalentConstraint(expected)
                    .Using(comparer));
        }
    }
}