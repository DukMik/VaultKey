using TheApiDto;

namespace TheBlazorVault.Components.Pages
{
    public partial class Home
    {
        public UserDto? TheUser { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var toto = await authStateProvider.GetAuthenticationStateAsync();
            TheUser = await UserService.GetCurrentUserAsync();
        }
    }
}
