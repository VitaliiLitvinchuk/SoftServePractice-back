using Application.Common.Interfaces.Queries;
using Domain.Actors;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ActorsController(IBaseQuery<Actor> query) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IEnumerable<Actor>> GetAll(CancellationToken cancellation)
        {
            return await query.GetMany(cancellation);
        }
    }
}
