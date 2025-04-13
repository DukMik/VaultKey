using Microsoft.AspNetCore.Components;
using TheApiDto;



namespace TheBlazorVault.Components.Pages
{
    public partial class VaultsPage
    {
        [Parameter]
        public Guid entraIdUser { get; set; }
        public List<VaultDto> _vaults { get; set; } = new ();
       
        private Guid _entraIdUser = Guid.Parse("a64e2dda-b127-4219-8225-7ea10fdc0e0e") ;
        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                // Vous pouvez logger ou afficher le nom de l'utilisateur par exemple
                Console.WriteLine("Utilisateur authentifié : " + user.Identity.Name);
            }
            else
            {
                NavigationManager.NavigateTo("authentication/login");
            }
            
            
            // Appel du service pour récupérer la liste des vaults de l'utilisateur connecté
            _vaults = await CallServices.GetVaultsAsync(_entraIdUser);
                
        
        }
    }

    
}

