using System;
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

        [ObservableProperty]
        private DateTime _startDate = DateTime.Today.AddDays(-30); // 默认显示最近30天

        [ObservableProperty]
        private DateTime _endDate = DateTime.Today;

        [ObservableProperty]
        private bool _useCustomDateRange;

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
                var data = UseCustomDateRange 
                    ? await _repository.GetRecordsByDateRangeAsync(StartDate, EndDate.AddDays(1).AddSeconds(-1))
                    : await _repository.GetAllRecordsAsync();
                
                Records.Clear();
                foreach (var record in data)
                {
                    Records.Add(record);
                }
            }
            catch (Exception ex)
            {
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
                System.Diagnostics.Debug.WriteLine($"Error deleting record: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task RefreshData()
        {
            await LoadDataAsync();
        }

        partial void OnStartDateChanged(DateTime value)
        {
            if (value > EndDate)
            {
                EndDate = value;
            }
            if (_isInitialized)
            {
                RefreshDataCommand.Execute(null);
            }
        }

        partial void OnEndDateChanged(DateTime value)
        {
            if (value < StartDate)
            {
                StartDate = value;
            }
            if (_isInitialized)
            {
                RefreshDataCommand.Execute(null);
            }
        }

        partial void OnUseCustomDateRangeChanged(bool value)
        {
            if (_isInitialized)
            {
                RefreshDataCommand.Execute(null);
            }
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
