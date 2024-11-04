namespace AlpimiAPI.Responses
{
    public class ApiGetAllResponse<Type>
    {
        public Type Content { get; set; }
        public Pagination Pagination { get; set; }
        public DateTime Timestamp { get; set; }

        public ApiGetAllResponse(Type content, Pagination pagination, DateTime timeStamp)
        {
            Content = content;
            Pagination = pagination;
            Timestamp = timeStamp;
        }
    }
}
