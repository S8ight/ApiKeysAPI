using ApiKeysApi.DTOs.Request;
using ApiKeysApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiKeysApi.Controllers;

[ApiController]
[Route("/api/v1/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var user = await _userService.UserLogin(loginRequest);
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(user);
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] UserRequest userRequest)
    {
        await _userService.CreateUser(userRequest);
        
        return Ok();
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserById(id);
        
        return Ok(user);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] PaginationRequest paginationRequest)
    {
        var users = await _userService.GetUsers(paginationRequest);
        
        return Ok(users);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userService.DeleteUser(id);
        
        return Ok();
    }
}