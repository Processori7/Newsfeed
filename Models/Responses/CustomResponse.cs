using Newsfeed.Models.Enums;

namespace Newsfeed.Models.Response;

public class CustomSuccesResponce<T> : ICustomResponce<T>
{
    public bool Success { get; }

    public StatusCode StatusCode { get; }

    public T Data { get; }
}

public class CustomSuccessResponse<T> : ICustomResponce<T>
{
    public bool Success { get; set; }

    public StatusCode StatusCode { get; set; }

    public T Data { get; set; }

    public int ErrorCode { get; set; }

    public int numberOfElements { get; }

    public int statusCode { get; set; }
}

public interface ICustomResponce<T>
{
    bool Success { get; }

    StatusCode StatusCode { get; }
    
    T Data { get; }
}
