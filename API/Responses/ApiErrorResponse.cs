namespace AlpimiAPI.Responses
{
    public class ApiErrorResponse
    {
        public int status { get; set; }
        public IEnumerable<ErrorObject> errors { get; set; }
        public DateTime timestamp { get; set; }

        public ApiErrorResponse(int Status, IEnumerable<ErrorObject> Errors)
        {
            status = Status;
            errors = Errors;
            timestamp = DateTime.UtcNow;
        }
    }

    public class ApiErrorException : Exception
    {
        public IEnumerable<ErrorObject> errors { get; set; }

        public ApiErrorException(IEnumerable<ErrorObject> Errors)
        {
            errors = Errors;
        }
    }

    public class ErrorObject
    {
        public string message { get; set; }

        public ErrorObject(string Message)
        {
            message = Message;
        }
    }
}
