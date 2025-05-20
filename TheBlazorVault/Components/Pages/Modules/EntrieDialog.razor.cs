using System.Text;
using Microsoft.AspNetCore.Components;
using TheApiDto;
using System.Security.Cryptography;
using TheBlazorVault.Service;

namespace TheBlazorVault.Components.Pages.Modules;

public partial class EntrieDialog : ComponentBase
{
    [Parameter]
    public EntrieDto? EntrieUpdate { get; set; } //=>passe en EntrieDtoCreation par la suite avec le password en param
    [Parameter]
    public int CurrentVault { get; set; } = default ;
    
    [Parameter]
    public EventCallback<int> CloseCallback { get; set; } = default ;
    
    [Parameter]
    public EventCallback<EntrieUncryptedDto> UpdateCallback { get; set; } = default ;
    
    [Parameter]
    public EventCallback<EntrieUncryptedDto> CreateCallback { get; set; } = default ;
    
    [Parameter]
    public string IsCreateOrIsEdit { get; set; } = default ;
    
    [Inject]
    CallServices CallServices { get; set; } = default!;
    
    public bool desactivated { get; set; } = false;
    public string password { get; set; }
    public string name { get; set; }
    public string username { get; set; }
    public string url { get; set; }
    public string comment { get; set; }
    
    public EntrieDto EntrieDto { get; set; }
    public EntrieUncryptedDto EntrieUncryptedDto { get; set; }
    public EntrieDtoCreation EntrieDtoCreation { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                //password = Encoding.UTF8.GetString(EntrieUpdate.NameData.CryptedData); // c'est ici que je déchiffre les données 
                base.OnAfterRender(firstRender);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public async void Close()
    {
      await CloseCallback.InvokeAsync();
    }
    
    public async void  Update()
    {

        await UpdateCallback.InvokeAsync(EntrieUncryptedDto);
    }



    public async void Create()
    {
        EntrieUncryptedDto = new EntrieUncryptedDto
        {
            NameData = name,
            UserNameData = username,
            UrlData = url,
            PasswordData = password,
            CommentData = comment,
            IsDesactivated = desactivated,
            VaultId = CurrentVault
        };

        // Invoking the CreateCallback with the newly created EntrieUncryptedDto
        await CreateCallback.InvokeAsync(EntrieUncryptedDto);
    }
    
}