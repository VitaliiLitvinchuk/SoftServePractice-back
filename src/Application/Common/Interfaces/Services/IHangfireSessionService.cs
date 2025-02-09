namespace Application.Common.Interfaces.Services;

public interface IHangfireSessionService
{
    Task ChangeSessionState();
}
