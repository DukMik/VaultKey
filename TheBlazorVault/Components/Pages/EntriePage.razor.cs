using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using TheApiDto;


namespace TheBlazorVault.Components.Pages
{
    public partial class EntriePage
    {
        [Parameter] public int CurrentVaultId { get; set; }

        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime IjsRuntime { get; set; } = default!;

        private List<EntrieDto> _entries = new();

        private bool _errorVisible = false;
        private string _errorMessage = "";

        private bool _showEntryForm = false;
        private bool _isNewEntry = true;
        private MudForm _form = new MudForm();
        private bool isEdit;
        private string typemodal = "";

        private EntrieDtoCreation _currentEntrieCreation = new();
        private EntrieDto _currentEntrie = new EntrieDto
        {
            NameData = new EncryptedDataDto(),
            UserNameData = new EncryptedDataDto(),
            UrlData = new EncryptedDataDto(),
            CommentData = new EncryptedDataDto()
        };

        private byte[] _globalIv = Array.Empty<byte>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    _entries = await CallServices.GetEntriesAsync(CurrentVaultId);
                
                    var isValid = await CallServices.IsConnectionValidAsync(CurrentVaultId);
                    var isValidBool = Convert.ToBoolean(isValid);

                    if (!isValidBool)
                    {
                        Navigation.NavigateTo("/vaults");
                    }
                    
                    await IjsRuntime.InvokeVoidAsync("decryptAndDisplay", _entries);
                    StateHasChanged();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        private async void OpenDialog(string typeButton, EntrieDto entrie)
        {
            var isValid = await CallServices.IsConnectionValidAsync(CurrentVaultId);
            var isValidBool = Convert.ToBoolean(isValid);
            
            if (isValidBool)
            {
                isEdit = true;
                typemodal = typeButton;
                _currentEntrie = entrie;
                StateHasChanged();
            } 
            else 
                Navigation.NavigateTo("/vaults", true);
        } 

        private async void CreateNewEntrieToApi(EntrieDtoCreation entrie)
        {
            try
            {        
                //je convertis les string en EncryptedDataDtoCreation
                EntrieDtoCreation entrieDtoCreation = new EntrieDtoCreation
                {
                    IsDesactivated = false,
                    NameData = entrie.NameData,
                    UserNameData = entrie.UserNameData,
                    PasswordData = entrie.PasswordData,
                    UrlData = entrie.UrlData,
                    CommentData = entrie.CommentData   
                };
                isEdit = true;

                var response = await CallServices.AddEntryAsync(CurrentVaultId, entrieDtoCreation);
                if (response.IsSuccessStatusCode)
                {
                    _showEntryForm = false;
                    _entries = await CallServices.GetEntriesAsync(CurrentVaultId);
                }
                else
                {
                    _errorVisible = true;
                    _errorMessage = "Erreur lors de la création de l'entrée.";
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                _errorVisible = true;
                _errorMessage = $"Erreur: {ex.Message}";
            }   
        }
        
        /// <summary>
        /// dans un projet de modification d'entries cela pourrait etre utile 
        /// </summary>
        /// <param name="entrie"></param>
        private void UpdateCurrentEntrieToApi(EntrieDto entrie)
        {
           
            
            _isNewEntry = false;
           // _currentEntrieCreation = new EntrieDtoCreation
           // {
           //     NameData = new EncryptedDataDtoCreation { CryptedData = entrie.NameData?.CryptedData },
           //     UserNameData = new EncryptedDataDtoCreation { CryptedData = entrie.UserNameData?.CryptedData },
           //     UrlData = new EncryptedDataDtoCreation { CryptedData = entrie.UrlData?.CryptedData },
           //     CommentData = new EncryptedDataDtoCreation { CryptedData = entrie.CommentData?.CryptedData }
           // };
            _showEntryForm = true;
        }
        
        private void Close()
        {
            isEdit = false;
            StateHasChanged();
        }
    }
}