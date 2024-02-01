namespace Core;

public class ErrorKind
{
    public const string DownloadFailure = "download_failure";
    public const string NotFound = "not_found";
    public const string InvalidFileContent = "invalid_file_content";
    public const string InvalidArguments = "invalid_arguments";
    public const string ExceptionThrown = "exception_thrown";
    public const string TimedOut = "timed_out";
}