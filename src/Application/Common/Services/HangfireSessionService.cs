using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Domain.Sessions;
using Domain.Statuses;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Services;

public class HangfireSessionService(IBaseQuery<Session> query, IBaseQuery<Status> statusQuery, IBaseRepository<Session> repository) : IHangfireSessionService
{
    public async Task ChangeSessionState()
    {
        var sessions = await query.GetMany(default, x => x.StartAt <= DateTime.UtcNow && x.EndAt > DateTime.UtcNow && x.Status!.Name == Defaults.StatusPending, include: x => x.Include(x => x.Status)!);

        if (sessions.Any())
        {
            var result = await statusQuery.Get(default, x => x.Name == Defaults.StatusActive);

            await result.Match(async status =>
            {
                foreach (var session in sessions)
                {
                    session.UpdateStatus(status.Id);
                    await repository.Update(session, default);
                }
            }, () => Task.CompletedTask);
        }

        sessions = await query.GetMany(default, x => x.EndAt < DateTime.UtcNow && x.Status!.Name != Defaults.StatusInactive, include: x => x.Include(x => x.Status)!);

        if (sessions.Any())
        {
            var result = await statusQuery.Get(default, x => x.Name == Defaults.StatusInactive);

            await result.Match(async status =>
            {
                foreach (var session in sessions)
                {
                    session.UpdateStatus(status.Id);
                    await repository.Update(session, default);
                }
            }, () => Task.CompletedTask);
        }
    }
}
