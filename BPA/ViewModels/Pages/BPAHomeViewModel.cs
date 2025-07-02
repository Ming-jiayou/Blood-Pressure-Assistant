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
        private string validationMessage = string.Empty;

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
                // 验证输入数值
                if (Systolic < 80)
                {
                    ValidationMessage = "高压数值必须大于80，请检查输入";
                    return;
                }
                if (Diastolic < 50)
                {
                    ValidationMessage = "低压数值必须大于50，请检查输入";
                    return;
                }
                if (HeartRate < 45)
                {
                    ValidationMessage = "心率必须大于45，请检查输入";
                    return;
                }

                // 清除验证消息
                ValidationMessage = string.Empty;

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
                ValidationMessage = $"保存记录时出错: {ex.Message}";
            }
        }

        [RelayCommand]
        private void Test()
        {
           
        }
    }
}
