using Newsfeed.Models.Enums;

namespace Newsfeed.Models.Response;

public class BaseResponse<T> : IBaseResponse<T>
{
    public bool Success { get; set; }

    public StatusCode StatusCode { get; set; }

    public object Data { get; set; }
    
    public int statusCode { get; set; }

    public DateTime TimeStamp { get; set; } = DateTime.Now;

    public int ErrorCode { get; internal set; }

    public string token { get; }

    public int id { get; }

    public object content { get; set; }

    public int numberOfElements { get; set; }
}

public interface IBaseResponse<T>
{
    bool Success { get; }

    StatusCode StatusCode { get; }
}

public class DataResponse<T>
{
    public List<T> Content { get; set; }

    public int numberOfElements { get; set; }

}

public class ObjectResponse<T>
{
    public object Content { get; set; }
}
