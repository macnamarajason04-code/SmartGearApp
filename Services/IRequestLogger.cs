namespace SmartGearApp.Services
{
    public interface IRequestLogger
    {
        Task LogAsync(HttpRequest request);
    }
}
