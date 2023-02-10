namespace AgileBlazor.Client.Auth
{
    public interface IAuthorizeService
    {
        Task LoginSetStorage(string token);
        Task LogoutRemoveStorage();
    }
}
