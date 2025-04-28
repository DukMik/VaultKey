// using TheApiDto;
// 
// namespace TheBlazorVault.Components.Pages
// {
//     public partial class Home
//     {
//         //private UserDto? TheUser { get; set; }
//         
//         // Propriété pour stocker la liste des vaults de l'utilisateur
//         private List<VaultDto> _vaults = new List<VaultDto>();
//         private Guid _entraIdUser = Guid.Parse("a64e2dda-b127-4219-8225-7ea10fdc0e0e") ;
//         protected override async Task OnInitializedAsync()
//         {
//             var authState = await AuthStateProvider.GetAuthenticationStateAsync();
//           // Appel du service pour récupérer la liste des vaults de l'utilisateur connecté
//                     //_vaults = await CallServices.GetVaultsAsync(_entraIdUser);
//                   
//             var user = authState.User;
//             if (!user.Identity.IsAuthenticated)
//             {
//                 // L'utilisateur n'est pas authentifié : vous pouvez le rediriger vers la page de connexion
//                 NavigationManager.NavigateTo("authentication/login");
//             }
//         
//         }
//     }
// }
// 


