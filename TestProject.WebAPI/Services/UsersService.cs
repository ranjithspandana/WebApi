﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestProject.WebAPI.Data;
using TestProject.WebAPI.Helpers;

namespace TestProject.WebAPI.Services
{
    public class UsersService : IUsersService
    {
        private readonly TestProjectContext _testProjectContext;

        public UsersService(TestProjectContext testProjectContext)
        {
            _testProjectContext = testProjectContext;
        }

        /*public Task<IEnumerable<User>> Get(int[] ids, Filters filters)
        {
            throw new System.NotImplementedException();
        }*/

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _testProjectContext.Users.SingleOrDefault(x => x.Email == username);

            // check if username exists
            if (user == null)
                return null;

            #region Password Hashing Commented
            /*
            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;
            # endregion
            */
            #endregion
            // authentication successful
            if (user.Password == password)
                return user;

            return null;
        }

        public async Task<User> Add(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_testProjectContext.Users.Any(x => x.Email == user.Email))
                throw new AppException("Username \"" + user.Email + "\" is already taken");

            #region Password Hashing Commented

            /*
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            */

            #endregion

            _testProjectContext.Users.Add(user);
            await _testProjectContext.SaveChangesAsync();

            return user;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _testProjectContext.Users.ToListAsync();
        }       

        public async Task<User> GetUserById(int id)
        {
            var user = await _testProjectContext.Users.FindAsync(id);
            return user;
            
        }
       
        public Task<IEnumerable<User>> AddRange(IEnumerable<User> users)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<User>> GetByFirstNames(string[] firstNames)
        {
            //We can implement same as GetUser By ID just filter by firstNames
            throw new System.NotImplementedException();
        }

        public async Task<User> Update(User userParam,string password)
        {
            var user = _testProjectContext.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            // update username if it has changed
            if (!string.IsNullOrWhiteSpace(userParam.Email) && userParam.Email != user.Email)
            {
                // throw error if the new username is already taken
                if (_testProjectContext.Users.Any(x => x.Email == userParam.Email))
                    throw new AppException("Username " + userParam.Email + " is already taken");

                user.Email = userParam.Email;
            }

            // update user properties if provided
            if (!string.IsNullOrWhiteSpace(userParam.FirstName))
                user.FirstName = userParam.FirstName;

            if (!string.IsNullOrWhiteSpace(userParam.LastName))
                user.LastName = userParam.LastName;

            user.Age = userParam.Age;
            user.Password = password;

            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                user.Password = password;
                //Hashing can be done  
                /*byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;*/
            }

            _testProjectContext.Users.Update(user);
            await _testProjectContext.SaveChangesAsync();

            return user;
        }

        public async Task<bool> Delete(int id)
        {
            var user = _testProjectContext.Users.Find(id);
            if (user != null)
            {
                _testProjectContext.Users.Remove(user);
                await _testProjectContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }

    public interface IUsersService
    {
        User Authenticate(string username, string password);

        Task<IEnumerable<User>> GetByFirstNames(string[] firstNames);

        Task<User> Add(User user,string password);

        Task<IEnumerable<User>> AddRange(IEnumerable<User> users);

        Task<User> Update(User user,string password =null);        

        Task<bool> Delete(int id);

        Task<User> GetUserById(int id);

        Task<IEnumerable<User>> GetAll();

    }

    public class Filters
    {
        public uint[] Ages { get; set; }
        public string[] FirstNames { get; set; }
        public string[] LastNames { get; set; }
    }
}
