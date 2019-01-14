using System.Threading.Tasks;

namespace API.Infrastructure.Identity
{
    public interface IUserService
    {
        Task<TokenObject> Authenticate(string uniqueId, int expireHours = 24);
    }
}
