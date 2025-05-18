// using Microsoft.AspNetCore.Components;
// using Microsoft.AspNetCore.Components.Authorization;
// using TheApiDto;
// using TheBlazorVault.Service;
//
// namespace TheBlazorVault.Components.Pages;
//
// public partial class EntriePage : ComponentBase
// {
//     [Inject] private CallServices CallServices { get; set; } = default!;
//     [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
//     [Inject] private NavigationManager Navigation { get; set; } = default!;
//
//     [Parameter] public int CurrentVaultId { get; set; }
//
//     private List<EntrieDto>? _entries;
//     private EntrieDtoCreation _newEntrie = new();
//     private string? _errorMessage;
//
//
//     private bool _isLoading = true;
//
//     protected override async Task OnAfterRenderAsync(bool firstRender)
//     {
//         try
//         {
//             if (firstRender)
//             {
//                 _isLoading = true;
//                 // Récupère l'IdUser depuis l'API (base de données)
//                 var userId = await CallServices.GetCurrentUserIdAsync();
//                 if (userId == null)
//                 {
//                     Navigation.NavigateTo("/authentication/login");
//                     return;
//                 }
//
//                 // Récupère les vaults de l'utilisateur depuis la base de données
//                 var vaults = await CallServices.GetVaultsAsync(userId.Value);
//
//                 // Vérifie que le vault demandé appartient bien à l'utilisateur (en base)
//                 var vault = vaults.FirstOrDefault(v => v.IdVault == CurrentVaultId && v.UserId == userId.Value);
//                 if (vault == null)
//                 {
//                     Navigation.NavigateTo("/unauthorized");
//                     return;
//                 }
//
//                 // Charge les entrées du vault
//                 _entries = await CallServices.GetEntriesAsync(CurrentVaultId);
//           
//
//                 _isLoading = false;
//
//                 StateHasChanged();
//             }
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             throw;
//         }
//     }
//
//     protected override async Task OnParametersSetAsync()
//     {
//         
//     }
//     
//      
//       private async Task CreateEntrieAsync()
//       {
//           _errorMessage = null;
//           try
//           {
//               if (_newEntrie.NameData == null || _newEntrie.UserNameData == null)
//               {
//                   _errorMessage = "Tous les champs obligatoires doivent être remplis.";
//                   return;
//               }
//      
//               //await CallServices.GetVaultsAsync(CurrentVaultId);
//      
//              //Recharge les entrées depuis la base pour l'utilisateur connecté
//               var userId = await CallServices.GetCurrentUserIdAsync();
//               if (userId == null)
//               {
//                   Navigation.NavigateTo("/authentication/login");
//                   return;
//               }
//               var vaults = await CallServices.GetVaultsAsync(userId.Value);
//               var vault = vaults.FirstOrDefault(v => v.IdVault == CurrentVaultId && v.UserId == userId.Value);
//               _entries = vault?.Entries ?? new List<EntrieDto>();
//               _newEntrie = new EntrieDtoCreation();
//           }
//           catch (Exception ex)
//           {
//               _errorMessage = ex.Message;
//           }
//       }
// }
//
// // using System;
// // using System.Net.Http;
// // using System.Net.Http.Json;
// // using System.Threading.Tasks;
// // using Microsoft.AspNetCore.Components;
// // using System.Collections.Generic;
// // using System.ComponentModel.DataAnnotations;
// // using TheApiDto;
// //
// //
// // namespace TheBlazorVault.Components.Pages;
// // public partial class EntriePage: ComponentBase
// // {
// //     [Parameter]
// //     public int VaultId { get; set; }
// //
// //     [Inject]
// //     public HttpClient Http { get; set; }
// //
// //     protected List<EntrieDto> _entries = new();
// //     protected EntrieDto _newEntry;
// //     protected EntrieDto _editingEntry;
// //
// //     protected override async Task OnInitializedAsync()
// //     {
// //         await LoadEntries();
// //         _newEntry = CreateEmptyEntry();
// //     }
// //
// //     private async Task LoadEntries()
// //     {
// //         _entries = await Http.GetFromJsonAsync<List<EntrieDto>>($"api/entries/vault/{VaultId}");
// //     }
// //
// //     protected EntrieDto CreateEmptyEntry() => new EntrieDto
// //     {
// //         VaultId = VaultId,
// //         NameDataId = 0,
// //         UserNameDataId = 0,
// //         UrlDataId = 0,
// //         CommentDataId = 0,
// //         IsDesactivated = false
// //     };
// //
// //     protected async Task AddEntry()
// //     {
// //         var response = await Http.PostAsJsonAsync("api/entries", _newEntry);
// //         if (response.IsSuccessStatusCode)
// //         {
// //             await LoadEntries();
// //             _newEntry = CreateEmptyEntry();
// //         }
// //         else
// //         {
// //             // gérer erreurs
// //         }
// //     }
// //
// //     protected void StartEdit(EntrieDto entry)
// //     {
// //         _editingEntry = new EntrieDto
// //         {
// //             IdEntrie = entry.IdEntrie,
// //             VaultId = entry.VaultId,
// //             NameDataId = entry.NameDataId,
// //             UserNameDataId = entry.UserNameDataId,
// //             UrlDataId = entry.UrlDataId,
// //             CommentDataId = entry.CommentDataId,
// //             CreatedDate = entry.CreatedDate,
// //             UpdatedDate = entry.UpdatedDate,
// //             IsDesactivated = entry.IsDesactivated
// //         };
// //     }
// //
// //     protected async Task SaveEdit()
// //     {
// //         var response = await Http.PutAsJsonAsync($"api/entries/{_editingEntry.IdEntrie}", _editingEntry);
// //         if (response.IsSuccessStatusCode)
// //         {
// //             _editingEntry = null;
// //             await LoadEntries();
// //         }
// //         else
// //         {
// //             // gestion erreur
// //         }
// //     }
// //
// //     protected void CancelEdit()
// //     {
// //         _editingEntry = null;
// //     }
// //
// //     // Ajout d'attributs de validation pour la page (DTO non annoté)
// //     public class EntryFormValidator
// //     {
// //         [Required(ErrorMessage = "Le nom (Id) est requis")] public int NameDataId { get; set; }
// //         [Required(ErrorMessage = "Le nom d'utilisateur (Id) est requis")] public int UserNameDataId { get; set; }
// //         [Required(ErrorMessage = "L'URL (Id) est requis")] public int UrlDataId { get; set; }
// //         [Required(ErrorMessage = "Le commentaire (Id) est requis")] public int CommentDataId { get; set; }
// //     }
// // }