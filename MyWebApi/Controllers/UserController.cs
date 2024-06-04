﻿using Microsoft.AspNetCore.Mvc;
using Entities;
using Services;
using AutoMapper;
using DTO;
using Azure.Identity;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService userService;
        private IMapper mapper;
        private readonly ILogger<UserController> logger;

        public UserController(IUserService userService,IMapper mapper, ILogger<UserController> logger)
        {
            this.userService = userService;
            this.mapper = mapper;
            this.logger = logger;
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<UserAfterLoginDTO>> Login([FromBody] LoginDTO userLogin)
        {
            User u = await userService.Login(userLogin);

            UserAfterLoginDTO userAfter = mapper.Map<User, UserAfterLoginDTO>(u);
            if (userAfter != null)
            {
                logger.LogInformation($"login attempted with UserName {userAfter.UserName}");
                return Ok(userAfter);
            }   
            return NoContent();
        }


        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterDTO userDto)
        {
            var user = mapper.Map<RegisterDTO, User>(userDto);

            User u =await userService.Register(user);
            if (u!=null)
                return CreatedAtAction(nameof(Get), new { id = u.UserId }, u);
            return NoContent();
        }

        [HttpPost("check")]
        public ActionResult Check([FromBody] object password)
        {
            
            var result = userService.Check(password);
            if (result >= 2)
                return Ok(result);
            return Accepted(result);
        }

        [HttpGet("{id}")]
        public async Task<User> Get(int id)
        {
            var user = await userService.Get(id);
            return user;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update(int id, [FromBody] UserDTO user)
        {
            UserDTO prevUser=await userService.returnPrev(id,user);

            User userAfter = mapper.Map<UserDTO, User>(prevUser);

            User u =await userService.Update(id, userAfter);
            if (u != null)
            {
                UserDTO uu = mapper.Map<User, UserDTO>(u);
                return Ok(uu);
            }     
            return NoContent();
        } 
    }
}
