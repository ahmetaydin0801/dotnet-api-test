using DotnetAPI.Models;

namespace DotnetAPI.Data
{
    // ASK what is repository pattern and how does it work
    public interface IUserRepository
    {
        public bool SaveChanges();
        public void AddEntity<T>(T entityToAdd);
        public void RemoveEntity<T>(T entityToRemove);

        public IEnumerable<User> GetUsers();

        public User GetSingleUser(int userId);

        public UserSalary GetUserSalary(int userId);

        public UserJobInfo GetUserJobInfo(int userId);

    }
}