namespace APIs.ErrorHandling
{
    public class ApiExceptionResponse : ApiResponce
    {
        public string? Details { get; set; }
        public ApiExceptionResponse(int StatusCode, string? Message = null, string? details = null) : base(StatusCode, Message)
        {
            Details = details;
        }
    }
}
