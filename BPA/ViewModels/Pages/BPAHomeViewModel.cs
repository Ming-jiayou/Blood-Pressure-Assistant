using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BPA.Models;
using BPA.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Controls;

namespace BPA.ViewModels.Pages
{
    public partial class BPAHomeViewModel : ObservableObject
    {
        private readonly IBloodPressureRepository _repository;

        [ObservableProperty]
        private int systolic;

        [ObservableProperty]
        private int diastolic;

        [ObservableProperty]
        private int heartRate;

        [ObservableProperty]
        private ObservableCollection<BloodPressureRecord> records = new();

        [ObservableProperty]
        string home = "你好啊";

        public BPAHomeViewModel(IBloodPressureRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            LoadRecentRecordsAsync();
        }

        private async void LoadRecentRecordsAsync()
        {
            try
            {
                var recentRecords = await _repository.GetRecentRecordsAsync(100);
                Records.Clear();
                foreach (var record in recentRecords)
                {
                    Records.Add(record);
                }
            }
            catch (Exception ex)
            {
                // 在实际应用中，你应该使用proper logging或错误处理
                System.Diagnostics.Debug.WriteLine($"加载记录时出错: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task SaveRecordAsync()
        {
            try
            {
                var record = new BloodPressureRecord
                {
                    Systolic = Systolic,
                    Diastolic = Diastolic,
                    HeartRate = HeartRate,
                    Timestamp = DateTime.Now
                };

                await _repository.AddRecordAsync(record);
                Records.Insert(0, record);

                // Reset input fields
                Systolic = 0;
                Diastolic = 0;
                HeartRate = 0;
            }
            catch (Exception ex)
            {
                // 在实际应用中，你应该使用proper logging或错误处理
                System.Diagnostics.Debug.WriteLine($"保存记录时出错: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Test()
        {
           
        }
    }
}
