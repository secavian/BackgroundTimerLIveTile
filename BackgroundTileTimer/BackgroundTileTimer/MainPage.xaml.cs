using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BackgroundTileTimer
{
    public sealed partial class MainPage : Page
    {
        private const string TASK_NAME = "TILE_UPDATE_TIMER_TASK_SAMPLE";

        public MainPage()
        {
            this.InitializeComponent();
            ToggleButtons();
        }

        private void ToggleButtons()
        {
            DisableButton.IsEnabled = GetTask();
            EnableButton.IsEnabled = !DisableButton.IsEnabled;
        }

        private async void Enable(object sender, RoutedEventArgs e)
        {
            try { await RegisterTask(); ToggleButtons(); }
            catch (Exception ex) { ShowDialog(ex.ToString()); }
        }

        private void Disable(object sender, RoutedEventArgs e)
        {
            try { UnregisterTask(); ToggleButtons(); }
            catch (Exception ex) { ShowDialog(ex.ToString()); }
        }

        private async void ShowDialog(string message)
        {
            var dlg = new MessageDialog(message);
            await dlg.ShowAsync();
        }

        public async Task RegisterTask()
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.Denied) { return; }

            if (GetTask()) { return; }

            var timeTrigger = new TimeTrigger(15, false);

            var backgroundTaskBuilder = new BackgroundTaskBuilder();
            backgroundTaskBuilder.Name = TASK_NAME;
            backgroundTaskBuilder.TaskEntryPoint = typeof(BackgroundTileTimerTask.BackgroundTask).FullName;
            backgroundTaskBuilder.SetTrigger(timeTrigger);

            backgroundTaskBuilder.Register();
        }

        public void UnregisterTask()
        {
            var task = new KeyValuePair<Guid, IBackgroundTaskRegistration>();
            if (GetTask(ref task))
            {
                task.Value.Unregister(true);
            }
        }

        public bool GetTask()
        {
            var task = new KeyValuePair<Guid, IBackgroundTaskRegistration>();
            return GetTask(ref task);
        }

        public bool GetTask(ref KeyValuePair<Guid, IBackgroundTaskRegistration> task)
        {
            foreach (var t in BackgroundTaskRegistration.AllTasks)
            {
                if (t.Value.Name == TASK_NAME)
                {
                    task = t;
                    return true;
                }
            }
            return false;
        }
    }
}
