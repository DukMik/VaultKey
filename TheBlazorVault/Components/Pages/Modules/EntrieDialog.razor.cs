using System.Text;
using Microsoft.AspNetCore.Components;
using TheApiDto;
using System.Security.Cryptography;
using TheBlazorVault.Service;
using Microsoft.JSInterop;

namespace TheBlazorVault.Components.Pages.Modules;

public partial class EntrieDialog : ComponentBase
{
    [Inject] CallServices CallServices { get; set; } = default!;
    [Inject] private IJSRuntime IjsRuntime { get; set; } = default!;

    // [Parameter]
    // public EntrieUncryptedDto? EntrieUpdate { get; set; }

    [Parameter]
    public EntrieDto? EntrieUpdate { get; set; }

    [Parameter]
    public EventCallback<int> CloseCallback { get; set; } = default;

    [Parameter]
    public EventCallback<EntrieDto> UpdateCallback { get; set; }

    [Parameter]
    public EventCallback<EntrieDtoCreation> CreateCallback { get; set; }

    [Parameter]
    public string IsCreateOrIsEdit { get; set; } = "";

    public record MiniCrypt(byte[] cypher, byte[] iv, byte[] tag);

    //public int CurrentVault { get; set; } = 0;
    //public bool desactivated { get; set; } = false;
    //public string password { get; set; } = "";
    //public string name { get; set; } = "";
    //public string username { get; set; } = "";
    //public string url { get; set; } = "";
    //public string comment { get; set; } = "";

 
    public EntrieDto EntrieDto { get; set; } = new EntrieDto
    {
        NameData = new EncryptedDataDto(),
        UserNameData = new EncryptedDataDto(),
        UrlData = new EncryptedDataDto(),
        CommentData = new EncryptedDataDto()
    };

    //public EntrieUncryptedDto EntrieUncryptedDto { get; set; } = new EntrieUncryptedDto
    //{
    //    NameData = "",
    //    UserNameData = "",
    //    UrlData = "",
    //    CommentData = ""
    //};

    public EntrieDtoCreation EntrieDtoCreation { get; set; } = new EntrieDtoCreation
    {
        NameData = new EncryptedDataDtoCreation(),
        UserNameData = new EncryptedDataDtoCreation(),
        UrlData = new EncryptedDataDtoCreation(),
        CommentData = new EncryptedDataDtoCreation()
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                if (IsCreateOrIsEdit == "Edit" && EntrieUpdate != null)
                {
                    // utiliser le décrypt 

                    // get le password via l'API + utiliser decrypt 
                    //password = EntrieUpdate.PasswordData;
                    //name = EntrieUpdate.NameData;
                    //username = EntrieUpdate.UserNameData;
                    //url = EntrieUpdate.UrlData;
                    //comment = EntrieUpdate.CommentData;
                    //desactivated = EntrieUpdate.IsDesactivated;
                }
                StateHasChanged();
                //base.OnAfterRender(firstRender);
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

    public async void Update()
    {
        await UpdateCallback.InvokeAsync(EntrieDto);
    }

    public async void Create()
    {
        MiniCrypt name = await IjsRuntime.InvokeAsync<MiniCrypt>("encrypt", ["name"]);
        MiniCrypt username = await IjsRuntime.InvokeAsync<MiniCrypt>("encrypt", ["username"]);
        MiniCrypt url = await IjsRuntime.InvokeAsync<MiniCrypt>("encrypt", ["url"]);
        MiniCrypt password = await IjsRuntime.InvokeAsync<MiniCrypt>("encrypt", ["password"]);
        MiniCrypt comment = await IjsRuntime.InvokeAsync<MiniCrypt>("encrypt", ["comment"]);

        EntrieDtoCreation = new EntrieDtoCreation
        {
            NameData = new EncryptedDataDtoCreation() { Iv = name.iv, CryptedData = name.cypher,Tag = name.tag },
            UserNameData = new EncryptedDataDtoCreation() { Iv = username.iv, CryptedData = username.cypher, Tag = username.tag },
            UrlData = new EncryptedDataDtoCreation() { Iv = url.iv, CryptedData = url.cypher, Tag = url.tag },
            PasswordData = new EncryptedDataDtoCreation() { Iv = password.iv, CryptedData = password.cypher, Tag = password.tag },
            CommentData = new EncryptedDataDtoCreation() { Iv = comment.iv, CryptedData = comment.cypher, Tag = comment.tag },
        };

        // Invoking the CreateCallback with the newly created EntrieUncryptedDto
        await CreateCallback.InvokeAsync(EntrieDtoCreation);
    }
}
