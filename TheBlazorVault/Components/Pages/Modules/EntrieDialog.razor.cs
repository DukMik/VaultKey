using System.Text;
using Microsoft.AspNetCore.Components;
using TheApiDto;

namespace TheBlazorVault.Components.Pages.Modules;

public partial class EntrieDialog : ComponentBase
{
    [Parameter]
    public EntrieDto? EntrieUpdate { get; set; } //=>passe en EntrieDtoCreation par la suite avec le password en param
    [Parameter]
    public int vault { get; set; } = default ;
    
    [Parameter]
    public EventCallback<int> CloseCallback { get; set; } = default ;
    
    [Parameter]
    public EventCallback<EntrieDto> UpdateCallback { get; set; } = default ;
    
    [Parameter]
    public EventCallback<(int vaultId, EntrieDtoCreation entrieCreation)> CreateCallback { get; set; } = default ;
    
    [Parameter]
    public string IsCreateOrIsEdit { get; set; } = default ;
    
    
    
    public bool desactivated { get; set; } = false;
    public string password { get; set; }
    public string name { get; set; }
    public string username { get; set; }
    public string url { get; set; }
    public string comment { get; set; }
    
    public EntrieDto EntrieDto { get; set; }
    public EntrieDtoCreation EntrieDtoCreation { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                password = Encoding.UTF8.GetString(EntrieUpdate.NameData.CryptedData); // c'est ici que je déchiffre les données 
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
        CloseCallback.InvokeAsync();
    }
    
    public async void  Update()
    {
        // chiffer les information avant des les envoyer a l'api
        
        UpdateCallback.InvokeAsync(EntrieDto);
        
        // a voir pour appler l'api direvctement ici 
    }
    
    public void Create()
    {
        // todo affetcer les valleurs du form dans le DTO cration 
        
        
        CreateCallback.InvokeAsync((vault, EntrieDtoCreation));
    }
    
}