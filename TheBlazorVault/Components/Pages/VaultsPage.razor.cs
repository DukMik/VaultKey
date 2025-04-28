using Microsoft.AspNetCore.Components;
using System.Security.Cryptography;
using System.Text;
using TheApiDto;



namespace TheBlazorVault.Components.Pages
{
    public partial class VaultsPage
    {
        [Parameter]
        public int UserId { get; set; }
        private List<VaultDto> Vaults { get; set; } = new ();
        
        private string newVaultName;
        private string? creationError;
        
        private Guid _entraIdUser = Guid.Parse("a64e2dda-b127-4219-8225-7ea10fdc0e0e") ;
        protected override async Task OnInitializedAsync()
        {
           var authState = await AuthStateProvider.GetAuthenticationStateAsync();
           var user = authState.User;
           if (user.Identity.IsAuthenticated)
           {
               //var contextDebug = await CallServices.GetContextDebugAsync();
               //Console.WriteLine("Contexte Debug reçu depuis l'API : " + contextDebug);
               
               
               // Vous pouvez logger ou afficher le nom de l'utilisateur par exemple
               Console.WriteLine("Utilisateur authentifié : " + user.Identity.Name);
               
               UserId = 2;
               // Appel du service pour récupérer la liste des vaults de l'utilisateur connecté
               Vaults = await CallServices.GetVaultsAsync(UserId);
           }
           else
           {
               NavigationManager.NavigateTo("authentication/login");
           }
        }
        
        private async Task CreateVault()
        {
            creationError = null;

            // Génération du salt
            var saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            var salt = Convert.ToBase64String(saltBytes);

            // Hash du nom de vault + salt
            var hashInput = Encoding.UTF8.GetBytes(newVaultName + salt);
            byte[] hashBytes;
            using (var sha = SHA256.Create())
            {
                hashBytes = sha.ComputeHash(hashInput);
            }
            var keyHash = Convert.ToBase64String(hashBytes);

            //try
            //{
            //    var created = await CallServices.CreateVaultAsync(UserId, newVaultName, keyHash, salt);
            //    Vaults.Add(created);
            //    newVaultName = string.Empty;
            //}
            //catch (Exception ex)
            //{
            //    creationError = "Erreur lors de la création : " + ex.Message;
            //}
        }//
    }

    
}

