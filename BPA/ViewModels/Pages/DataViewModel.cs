using System.Windows.Media;
using BPA.Models;
using BPA.Data;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Abstractions.Controls;

namespace BPA.ViewModels.Pages
{
    public partial class DataViewModel : ObservableObject, INavigationAware
    {
        private readonly IBloodPressureRepository _repository;
        private bool _isInitialized = false;

        [ObservableProperty]
        private ObservableCollection<BloodPressureRecord> _records;

        [ObservableProperty]
        private BloodPressureRecord _selectedRecord;

        [ObservableProperty]
        private bool _isLoading;

        public DataViewModel(IBloodPressureRepository repository)
        {
            _repository = repository;
            _records = new ObservableCollection<BloodPressureRecord>();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                var data = await _repository.GetAllRecordsAsync();
                Records.Clear();
                foreach (var record in data)
                {
                    Records.Add(record);
                }
            }
            catch (Exception ex)
            {
                // TODO: Add proper error handling
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeleteRecord()
        {
            if (SelectedRecord == null)
                return;

            try
            {
                await _repository.DeleteRecordAsync(SelectedRecord.Id);
                Records.Remove(SelectedRecord);
            }
            catch (Exception ex)
            {
                // TODO: Add proper error handling
                System.Diagnostics.Debug.WriteLine($"Error deleting record: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task RefreshData()
        {
            await LoadDataAsync();
        }

        public async Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
            {
                await LoadDataAsync();
                _isInitialized = true;
            }
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;
    }
}
