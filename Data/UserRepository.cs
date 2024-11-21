using DotnetAPI.Models;

namespace DotnetAPI.Data
{
    public class UserRepository : IUserRepository
    {
        private DataContextEF _entityFramework;

        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Add(entityToAdd);
            }
        }

        public void RemoveEntity<T>(T entityToRemove)
        {
            _entityFramework.Remove(entityToRemove);
        }

        public IEnumerable<User> GetUsers()
        {
            IEnumerable<User> users = _entityFramework.Users.ToList();
            return users;
        }

        public User GetSingleUser(int userId)
        {
            User user = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault();
            if (user != null)
            {
                return user;
            }

            throw new Exception("Failed to get user");
        }

        public UserSalary GetUserSalary(int userId)
        {
            var userSalary = _entityFramework.UserSalary.FirstOrDefault(s => s.UserId == userId);
            if (userSalary != null)
            {
                return userSalary;
            }

            throw new Exception("Failed to get salary");
        }

        public UserJobInfo GetUserJobInfo(int userId)
        {
            var jobInfo = _entityFramework.UserJobInfo.FirstOrDefault(j => j.UserId == userId);
            if (jobInfo != null)
            {
                return jobInfo;
            }

            throw new Exception("Failed to get job info");
        }
    }
}