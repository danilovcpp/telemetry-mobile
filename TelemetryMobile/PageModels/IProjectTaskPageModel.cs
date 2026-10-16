using CommunityToolkit.Mvvm.Input;
using TelemetryMobile.Models;

namespace TelemetryMobile.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}