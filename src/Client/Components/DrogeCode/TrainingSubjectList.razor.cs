using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public partial class TrainingSubjectList : ComponentBase
{
    [Inject, NotNull] private IStringLocalizer<TrainingSubjectList>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private TrainingTargetRepository? TrainingTargetRepository { get; set; }
    [Parameter] public SelectionMode SelectionMode { get; set; }
    [Parameter] public EventCallback<IReadOnlyCollection<Guid>> SelectedTargetsChanged { get; set; }
    private IReadOnlyCollection<Guid> _selectedTargets = [];

    [Parameter, EditorRequired] public bool TargetSetReadonly { get; set; }

    [Parameter]
    public IReadOnlyCollection<Guid> SelectedTargets
    {
        get => _selectedTargets;
        set
        {
            DebugHelper.WriteLine($"SelectedTargets changed to {value.Count}");
            if (Equals(_selectedTargets, value)) return;
            _selectedTargets = value;
            SelectedTargetsChanged.InvokeAsync(value);
        }
    }

    private readonly CancellationTokenSource _cls = new();
    private List<TrainingSubject>? _trainingSubjects;
    private string _searchTargetText = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _trainingSubjects = await TrainingTargetRepository.AllTrainingTargets(50, 0, _cls.Token);
            StateHasChanged();
        }
    }

    private void OnSearchTarget(string search)
    {
        if (!string.IsNullOrEmpty(search) && _trainingSubjects is not null)
        {
            UpdateTrainingVisibilityBasedOnSearch(_trainingSubjects, search);
        }
    }

    private bool UpdateTrainingVisibilityBasedOnSearch(List<TrainingSubject> trainingSubjects, string search)
    {
        var isVisible = false;
        foreach (var subject in trainingSubjects)
        {
            subject.IsVisible = false;
            if (subject.TrainingTargets is not null && subject.TrainingTargets.Count != 0)
            {
                foreach (var target in subject.TrainingTargets)
                {
                    if (target.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) == true || SelectedTargets.Contains(target.Id))
                    {
                        target.IsVisible = true;
                        subject.IsVisible = true;
                    }
                    else
                    {
                        target.IsVisible = false;
                    }
                }
            }
            else
            {
                subject.IsVisible = subject.TrainingSubjects is not null && UpdateTrainingVisibilityBasedOnSearch(subject.TrainingSubjects, search);
            }

            if (subject.IsVisible)
            {
                isVisible = true;
            }
        }

        return isVisible;
    }
}