using MongoDB.Driver;
using SimpleWebApi.Requests;
using SimpleWebApi.DataEntities;

namespace SimpleWebApi.Services;

public class ApplicationUserService
{
    private readonly ILogger<ApplicationUserService> logger;
    private readonly IMongoCollection<ApplicationUser> applicationUserCollection;

    public ApplicationUserService(ILogger<ApplicationUserService> logger, [FromKeyedServices("application_user")] IMongoCollection<ApplicationUser> applicationUserCollection)
    {
        this.logger = logger;
        this.applicationUserCollection = applicationUserCollection;
    }
    public async Task RegisterNewStudentAsync(RegisterNewApplicationUserRequest request)
    {
        await applicationUserCollection.InsertOneAsync(new ApplicationUser
        {
            
            Username = request.Username,
            PasswordHash = request.Password,
            PasswordSalt = request.Password
        });
    }

    //public async Task UpdateCourseAsync(UpdateCourseRequest request)
    //{
    //    var filter = Builders<Course>.Filter.Eq(course => course.Id, request.CourseId);

    //    var update = Builders<Course>.Update
    //        .Set(r => r.SubjectCode, request.SubjectCode)
    //        .Set(r => r.Level, request.Level)
    //        .Set(r => r.Name, request.Name);

    //    var result = await studentCollection.UpdateManyAsync(filter, update);
    //}

    //public async Task RemoveCourseAsync(string courseId)
    //{
    //    var filter = Builders<Course>.Filter.Eq(course => course.Id, courseId);

    //    var result = await studentCollection.DeleteManyAsync(filter);


    //}

    //public async Task<List<Course>> GetAllCoursesAsync()
    //{
    //    var filter = Builders<Course>.Filter.Empty;

    //    List<Course> result = await studentCollection.Find(filter).ToListAsync();

    //    return result;
    //}

    //internal async Task RegisterNewStudentAsync(RegisterNewStudentRequest request)
    //{
    //    throw new NotImplementedException();
    //}
}

