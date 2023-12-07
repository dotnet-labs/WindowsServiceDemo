namespace Demo.Services;

public interface IServiceA
{
    void Run();
}

public class ServiceA(ILogger<ServiceA> logger, IServiceB serviceB) : IServiceA
{
    public void Run()
    {
        logger.LogInformation("In Service A");
        serviceB.Run();
    }
}