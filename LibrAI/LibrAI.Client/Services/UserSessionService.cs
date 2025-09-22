using Blazored.LocalStorage;
using LibrAI.Shared;

public class UserSessionService
{
    private readonly ILocalStorageService _localStorage;
    private const string Key = "user";

    public UserSessionService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task SaveUserAsync(UserDto user)
    {
        await _localStorage.SetItemAsync(Key, user);
    }

    public async Task<UserDto?> GetUserAsync()
    {
        return await _localStorage.GetItemAsync<UserDto>(Key);
    }

    public async Task ClearAsync()
    {
        await _localStorage.RemoveItemAsync(Key);
    }

    public async Task<bool> IsLoggedInAsync()
    {
        var user = await GetUserAsync();
        return user != null;
    }
}