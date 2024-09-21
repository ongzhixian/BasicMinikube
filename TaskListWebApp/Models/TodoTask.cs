namespace TaskListWebApp.Models;

//public class TodoTask2
//{
//    public int Id { get; set; }

//    public bool IsCompleted { get; set; }

//    public int Priority { get; set; }
//}


public record TodoTask
{
    public int Id { get; set; }

    public string Description { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("is_completed")]
    public int IsCompleted { get; set; }

    public int Priority { get; set; }

    public bool IsCompleteFlag { 
        get
        {
            return IsCompleted == 1 ? true : false;
        } 
    }

    //public TodoTask(TodoTask todoTask)
    //{

    //}

    //public object Clone() => new TodoTask { Id = Id, Description = Description, IsCompleted = IsCompleted, Priority = Priority };
}

//public class Rootobject
//{
//    public Record[] records { get; set; }
//}

//public class Record
//{
//    public int id { get; set; }
//    public string is_completed { get; set; }
//    public int priority { get; set; }
//}


//public class Rootobject
//{
//    public Class1[] Property1 { get; set; }
//}

//public class Class1
//{
//    public int id { get; set; }
//    public string is_completed { get; set; }
//    public int priority { get; set; }
//}

