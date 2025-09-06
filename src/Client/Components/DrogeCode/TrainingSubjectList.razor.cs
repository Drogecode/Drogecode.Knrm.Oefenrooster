using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public partial class TrainingSubjectList : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<TrainingSubjectList>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private TrainingTargetRepository? TrainingTargetRepository { get; set; }
    [Parameter] public SelectionMode SelectionMode { get; set; }
    [Parameter] public EventCallback<IReadOnlyCollection<Guid>> SelectedTargetsChanged { get; set; }
    private IReadOnlyCollection<Guid>? _selectedTargets;
    
    [Parameter] public RenderFragment<TrainingTarget>? BodyContent { get; set; }
    [Parameter] public bool TargetSetReadonly { get; set; }
    [Parameter]
    public IReadOnlyCollection<Guid>? SelectedTargets
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
            _trainingSubjects = new List<TrainingSubject>();
            var count = 15;
            var skip = 0;
            while(true)
            {
                var d = await TrainingTargetRepository.AllTrainingTargets(count, skip * count, _cls.Token);
                if (d?.TrainingSubjects is null)
                {
                    DebugHelper.WriteLine("TrainingSubjects is null");
                    break;
                }
                _trainingSubjects.AddRange( d.TrainingSubjects);
                if (d.TotalCount <= _trainingSubjects.Count)
                {
                    DebugHelper.WriteLine($"TotalCount <= _trainingSubjects.Count == {d.TotalCount} <= {_trainingSubjects.Count}");
                    break;
                }
                skip++;
            }
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

    public void Dispose()
    {
        _cls.Cancel();
    }
}