using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestProject.WebAPI.Data;

namespace TestProject.WebAPI.Services
{
    public class UsersService : IUsersService
    {
        private readonly TestProjectContext _testProjectContext;

        public UsersService(TestProjectContext testProjectContext)
        {
            _testProjectContext = testProjectContext;
        }


        public Task<IEnumerable<User>> Get(int[] ids, Filters filters)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> Add(User user)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<User>> AddRange(IEnumerable<User> users)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> Update(User user)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Delete(User user)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IUsersService
    {
        Task<IEnumerable<User>> Get(int[] ids, Filters filters);

        Task<User> Add(User user);

        Task<IEnumerable<User>> AddRange(IEnumerable<User> users);

        Task<User> Update(User user);

        Task<bool> Delete(User user);
    }

    public class Filters
    {
        public uint[] Ages { get; set; }
        public string[] FirstNames { get; set; }
        public string[] LastNames { get; set; }
    }
}
