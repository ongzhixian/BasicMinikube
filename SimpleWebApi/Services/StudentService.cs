using MongoDB.Driver;
using SimpleWebApi.Requests;
using SimpleWebApi.DataEntities;

namespace SimpleWebApi.Services;

public class StudentService
{
    private readonly ILogger<StudentService> logger;
    private readonly IMongoCollection<Student> studentCollection;

    public StudentService(ILogger<StudentService> logger, [FromKeyedServices("student")] IMongoCollection<Student> studentCollection)
    {
        this.logger = logger;
        this.studentCollection = studentCollection;
    }
    public async Task RegisterNewStudentAsync(RegisterNewStudentRequest request)
    {
        await studentCollection.InsertOneAsync(new Student
        {
            IdentityCode = request.IdentityCode,
            EducationLevel = request.EducationLevel,
            Name = request.Name,
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

