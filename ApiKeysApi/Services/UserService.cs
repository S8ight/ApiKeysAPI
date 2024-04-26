using ApiKeysApi.DataAccess.Entities;
using ApiKeysApi.DTOs.Request;
using ApiKeysApi.DTOs.Response;
using ApiKeysApi.Interfaces;
using AutoMapper;
using Gridify;

namespace ApiKeysApi.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public UserService(ILogger<UserService> logger, IMapper mapper, ITokenService tokenService, IUserRepository userRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    public async Task<UserResponse?> UserLogin(LoginRequest request)
    {
        var user = await _userRepository.GetUserByUserNameAsync(request.Username);

        if (user == null)
        {
            return null;
        }

        var passwordCheck = VerifyPassword(request.Password, user.Password);
        if (!passwordCheck) return null;

        var accessToken = _tokenService.CreateToken(user);

        user.AccessToken = accessToken;
        await _userRepository.UpdateUserAsync(user);

        var userResponse = _mapper.Map<User, UserResponse>(user);

        return userResponse;
    }

    public async Task CreateUser(UserRequest request)
    {
        var userCheck = await _userRepository.GetUserByUserNameAsync(request.UserName);
        if (userCheck != null)
        {
            throw new ArgumentException("User already exists");
        }

        var user = new User
        {
            UserName = request.UserName,
            Password = PasswordHashing(request.Password),
            Role = request.Role
        };

        await _userRepository.AddUserAsync(user);
        _logger.LogInformation("Created new user({UserRole}) with username: {UserName}", user.Role, user.UserName);
    }


    public async Task<PaginationResponse<UsersListResponse>> GetUsers(PaginationRequest request)
    {
        var users = _userRepository.GetUsers();
        if (users.Any())
        {
            var paginatedUsers = users.ApplyPaging(request.PageNumber, request.PageSize).ToList();
            var mappedUsers = _mapper.Map<List<User>, List<UsersListResponse>>(paginatedUsers);

            return new PaginationResponse<UsersListResponse>
            {
                PagesCount = (int)Math.Ceiling((double)users.Count() / request.PageSize),
                Items = mappedUsers
            };
        }

        return new PaginationResponse<UsersListResponse>();
    }

    public async Task<UserResponse> GetUserById(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with provided id: {id} not found");
        }
        
        var userResponse = _mapper.Map<User, UserResponse>(user);
        
        return userResponse;
    }
    
    public async Task DeleteUser(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        
        if(user == null)
        {
            throw new KeyNotFoundException("User not found");
        }
        
        await _userRepository.DeleteUserAsync(user);
        _logger.LogInformation("User: {UserName} deleted", user.UserName);
    }
    
    private string PasswordHashing(string password)
    {
        string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        return hashedPassword;
    }

    private bool VerifyPassword(string inputPassword, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
    }
}