using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode.Dialogs;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public sealed partial class ReadMoreChip
{
    [Inject] private IStringLocalizer<ReadMoreChip> L { get; set; } = default!;
    [Inject] private IDialogService DialogProvider { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter, EditorRequired, NotNull] public TrainingAdvance? Training { get; set; }

    private bool _authTargetRead;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _authTargetRead =  await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_target_read);
            StateHasChanged();
        }
    }
    
    private Task OpenMessageDialog()
    {
        var parameters = new DialogParameters<TrainingMessageDialog>
        {
            { x => x.Training, Training },
        };
        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Large,
            CloseButton = true,
            FullWidth = true
        };
        return DialogProvider.ShowAsync<TrainingMessageDialog>(L["Training message"], parameters, options);
    }
}