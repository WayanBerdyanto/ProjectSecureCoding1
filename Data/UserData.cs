using System;
using System.Linq;
using ProjectSecureCoding1.Models;
using BC = BCrypt.Net.BCrypt;

namespace ProjectSecureCoding1.Data
{
    public class UserData : IUser
    {
        private readonly ApplicationDbContext _db;

        public UserData(ApplicationDbContext db)
        {
            _db = db;
        }

        // Login method
        public Users Login(Users users)
        {
            var _user = _db.Users.FirstOrDefault(u => u.Username == users.Username);

            if (_user == null)
            {
                throw new Exception("User not found");
            }

            if (!BC.Verify(users.Password, _user.Password))
            {
                throw new Exception("Incorrect password");
            }

            return _user;
        }

        // Registration method
        public Users Registration(Users users)
        {
            try
            {
                users.Password = BC.HashPassword(users.Password);

                _db.Users.Add(users);
                _db.SaveChanges();
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception("Registration failed: " + ex.Message);
            }
        }

        public Users GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username is required");
            }
            else
            {
                return _db.Users.FirstOrDefault(u => u.Username == username);
            }
        }

        public Users UpdateUser(Users user)
        {
            var existingUser = _db.Users.FirstOrDefault(u => u.Username == user.Username); // Assuming you have an Id property
            if (existingUser == null)
            {
                throw new Exception("User not found.");
            }

            existingUser.Username = user.Username;
            existingUser.Password = BC.HashPassword(user.Password); // Hash the new password
            existingUser.Role = user.Role; // Update role if needed

            _db.SaveChanges();
            return existingUser;
        }
    }
}
