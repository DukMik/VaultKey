using TheApiDto;

namespace TheBlazorVault.Components.Pages
{
    public partial class Home
    {
        public UserDto? TheUser { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TheUser = await UserService.GetCurrentUserAsync();
        }
    }
}
