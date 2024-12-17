namespace AlpimiAPI.Responses
{
    public class ApiGetResponse<Type>
    {
        public Type Content { get; set; }
        public DateTime Timestamp { get; set; }

        public ApiGetResponse(Type content)
        {
            Content = content;
            Timestamp = DateTime.UtcNow;
        }
    }
}
