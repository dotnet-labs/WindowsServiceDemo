namespace Demo.Services;

public interface IServiceB
{
    void Run();
}

public class ServiceB(ILogger<ServiceB> logger) : IServiceB
{
    public void Run()
    {
        logger.LogInformation("In Service B");
    }
}