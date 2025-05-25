using Microsoft.AspNetCore.Components;
using System;

namespace TheBlazorVault.Components.Pages
{
    public partial class Home
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private void Logout()
        {
            NavigationManager.NavigateTo(
                "MicrosoftIdentity/Account/SignOut?postLogoutRedirectUri=/",
                forceLoad: true
            );
        }

        private void Navigue()
        {
            try
            {
                NavigationManager.NavigateTo("/vaults");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}


