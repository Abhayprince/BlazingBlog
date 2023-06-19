using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.Json;

namespace BlazingBlog.Authentication
{
    public class AuthenticationService
    {
        private readonly UserService _userService;
        private readonly ProtectedLocalStorage _protectedLocalStorage;

        public AuthenticationService(UserService userService, ProtectedLocalStorage protectedLocalStorage)
        {
            _userService = userService;
            _protectedLocalStorage = protectedLocalStorage;
        }

        public async Task<LoggedInUser?> LoginUserAsync(LoginModel loginModel)
        {
            var loggedInUser = await _userService.LoginAsync(loginModel);
            if (loggedInUser is not null)
            {
                await SaveUserToBrowserStorageAsync(loggedInUser.Value);
            }
            return loggedInUser;
        }
        private const string UserStorageKey = "blg_user";
        private JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {

        };
        public async Task SaveUserToBrowserStorageAsync(LoggedInUser user) =>
            await _protectedLocalStorage.SetAsync(UserStorageKey, JsonSerializer.Serialize(user, _jsonSerializerOptions));

        public async Task<LoggedInUser?> GetUserFromBrowserStorageAsync()
        {
            try
            {
                var result = await _protectedLocalStorage.GetAsync<string>(UserStorageKey);
                if (result.Success && !string.IsNullOrWhiteSpace(result.Value))
                {
                    var loggedInUser = JsonSerializer.Deserialize<LoggedInUser>(result.Value, _jsonSerializerOptions);
                    return loggedInUser;
                }
            }
            catch (InvalidOperationException)
            {
                // Eat out this exception
                // as we know this will occure when this method is being called from server
                // Where there is no Browser and LocalStorage
                // Dont worry about this, as this will be called from client side(Browser's side) after this
                // So we will have the data there
                // So we are good to ignore this exception
            }
            return null;
        }
    
        public async Task RemoveUserFromBroserStorageAsync()=>
            await _protectedLocalStorage.DeleteAsync(UserStorageKey);
    }
}
