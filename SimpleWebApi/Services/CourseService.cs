using MongoDB.Driver;
using SimpleWebApi.Requests;
using SimpleWebApi.DataEntities;

namespace SimpleWebApi.Services;

public class CourseService
{
    private readonly ILogger<CourseService> logger;
    //private readonly IMongoDatabase mongoDatabase;
    private readonly IMongoCollection<Course> courseCollection;

    public CourseService(ILogger<CourseService> logger, [FromKeyedServices("course")] IMongoCollection<Course> courseCollection)
    {
        this.logger = logger;
        this.courseCollection = courseCollection;
        //this.mongoDatabase = mongoDatabase;

        //var collection = client.GetDatabase("sample_mflix").GetCollection<BsonDocument>("movies");

        //IMongoCollection<BsonDocument> coll = mongoDatabase.GetCollection<BsonDocument>("user");
        //var filter = Builders<BsonDocument>.Filter.Eq("title", "Back to the Future");
        //var document = collection.Find(filter).First();
    }

    public async Task RegisterCourseAsync(NewCourseRequest request)
    {
        //var collection =
        //.GetCollection<BsonDocument>("movies");

        //db.members.createIndex( { groupNumber: 1, lastname: 1, firstname: 1 }, { unique: true } )

        //var indexOptions = new CreateIndexOptions { Unique = true };
        //var indexModel = new CreateIndexModel<Course>(
        //    Builders<Course>.IndexKeys
        //        .Ascending(m => m.SubjectCode)
        //        .Ascending(m => m.Level)
        //    , indexOptions);
        //courseCollection.Indexes.CreateOne(indexModel);

        //var indexModel = new CreateIndexModel<Course>(
        //    Builders<Course>.IndexKeys
        //        .Ascending(m => m.Name));
        //courseCollection.Indexes.CreateOne(indexModel);

        await courseCollection.InsertOneAsync(new Course
        {
            SubjectCode = request.SubjectCode,
            Level = request.Level,
            Name = request.Name,
        });
    }

    public async Task UpdateCourseAsync(UpdateCourseRequest request)
    {
        var filter = Builders<Course>.Filter.Eq(course => course.Id, request.CourseId);

        var update = Builders<Course>.Update
            .Set(r => r.SubjectCode, request.SubjectCode)
            .Set(r => r.Level, request.Level)
            .Set(r => r.Name, request.Name);
        
        var result = await courseCollection.UpdateManyAsync(filter, update);
    }

    public async Task RemoveCourseAsync(string courseId)
    {
        var filter = Builders<Course>.Filter.Eq(course => course.Id, courseId);
        
        var result = await courseCollection.DeleteManyAsync(filter);

        
    }

    public async Task<List<Course>> GetAllCoursesAsync()
    {
        var filter = Builders<Course>.Filter.Empty;

        List<Course> result = await courseCollection.Find(filter).ToListAsync();

        return result;
    }
}

