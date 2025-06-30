using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BPA.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BPA.ViewModels.Pages
{
    public partial class BPAHomeViewModel : ObservableObject
    {
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

        [RelayCommand]
        private void SaveRecord()
        {
            var record = new BloodPressureRecord
            {
                Systolic = Systolic,
                Diastolic = Diastolic,
                HeartRate = HeartRate,
                Timestamp = DateTime.Now
            };

            Records.Insert(0, record);
            
            // Reset input fields
            Systolic = 0;
            Diastolic = 0;
            HeartRate = 0;
        }
    }
}
