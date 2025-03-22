public class ErrorResponse
{
    public string Message { get; set; }
    public string Error { get; set; }

    public ErrorResponse(string message, string error)
    {
        Message = message;
        Error = error;
    }
}