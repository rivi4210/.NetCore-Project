﻿using Entities;
using Repositories;
using DTO;

namespace Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        async Task<User> IUserService.Update(int id, User user)
        {
            var checkStrength = Check(user.Password);
            if (checkStrength < 2)
                return null;
            User u = await _userRepository.Update(id, user);
            return u;
        }
        async Task<User> IUserService.Login(LoginDTO userLogin)
        {
            User u = await _userRepository.Login(userLogin);
            return u;
        }
        async Task<User> IUserService.Register(User user)
        {
            var checkStrength = Check(user.Password);
            if (checkStrength < 2)
                return null;
            User u = await _userRepository.Register(user);
            return u;
        }

        public async Task<User> Get(int id)
        {
            return await _userRepository.Get(id);
        }
        public int Check(object password)
        {
            var result = Zxcvbn.Core.EvaluatePassword(password.ToString());
            return result.Score;
        }

        public async Task<UserDTO> ReturnPrev(int id, UserDTO user)
        {
            User prevUser = await Get(id);

            user.UserName = user.UserName == null ? prevUser.UserName : user.UserName;
            user.Password = user.Password == null ? prevUser.Password : user.Password;
            user.FirstName = user.FirstName == null ? prevUser.FirstName : user.FirstName;
            user.LastName = user.LastName == null ? prevUser.LastName : user.LastName;
            user.Email = user.Email == null ? prevUser.Email : user.Email;

            return user;
        }
    }
}
