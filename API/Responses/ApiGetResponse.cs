namespace AlpimiAPI.Responses
{
    public class ApiGetResponse<Type>
    {
        public Type Content { get; set; }

        public DateTime Timestamp { get; set; }

        public ApiGetResponse(Type content, DateTime time)
        {
            Content = content;
            Timestamp = time;
        }
    }
}
