using System;
using System.Windows.Media;
using BPA.Models;
using BPA.Data;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Abstractions.Controls;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Linq;

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

        // 图表显示选项
        [ObservableProperty]
        private bool _showSystolic = true;

        [ObservableProperty]
        private bool _showDiastolic = true;

        [ObservableProperty]
        private bool _showHeartRate = true;

        public string Title { get; private set; }

        public PlotModel PlotModel { get; private set; }


        public DataViewModel(IBloodPressureRepository repository)
        {
            _repository = repository;
            _records = new ObservableCollection<BloodPressureRecord>();

            Title = "血压监测图表";
            InitializePlotModel();
        }

        private void InitializePlotModel()
        {
            PlotModel = new PlotModel { Title = Title };

            // X轴 - 时间轴
            var dateAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = "测量时间",
                StringFormat = "MM-dd\nHH:mm",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(200, 200, 200),
                MinorGridlineColor = OxyColor.FromRgb(230, 230, 230),
                Angle = -45
            };
            PlotModel.Axes.Add(dateAxis);

            // Y轴 - 血压值
            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "测量数值",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(200, 200, 200),
                MinorGridlineColor = OxyColor.FromRgb(230, 230, 230),
                Minimum = 40, // 设置最小值，避免图表过于压缩
                Maximum = 200 // 设置最大值
            };
            PlotModel.Axes.Add(valueAxis);

            // 收缩压（高压）散点系列
            var systolicSeries = new ScatterSeries
            {
                Title = "收缩压 (高压)",
                MarkerFill = OxyColor.FromRgb(220, 53, 69), // 红色
                MarkerStroke = OxyColor.FromRgb(180, 40, 55),
                MarkerType = MarkerType.Circle,
                MarkerSize = 6,
                MarkerStrokeThickness = 1
            };
            PlotModel.Series.Add(systolicSeries);

            // 舒张压（低压）散点系列
            var diastolicSeries = new ScatterSeries
            {
                Title = "舒张压 (低压)",
                MarkerFill = OxyColor.FromRgb(40, 167, 69), // 绿色
                MarkerStroke = OxyColor.FromRgb(30, 120, 50),
                MarkerType = MarkerType.Square,
                MarkerSize = 6,
                MarkerStrokeThickness = 1
            };
            PlotModel.Series.Add(diastolicSeries);

            // 心率散点系列
            var heartRateSeries = new ScatterSeries
            {
                Title = "心率",
                MarkerFill = OxyColor.FromRgb(255, 193, 7), // 黄色
                MarkerStroke = OxyColor.FromRgb(200, 150, 0),
                MarkerType = MarkerType.Diamond,
                MarkerSize = 6,
                MarkerStrokeThickness = 1
            };
            PlotModel.Series.Add(heartRateSeries);

            // 设置图例
            PlotModel.IsLegendVisible = true;
        }

        private void UpdatePlotModel()
        {
            if (PlotModel?.Series == null || Records == null || !Records.Any())
                return;

            // 清除现有数据点
            foreach (var series in PlotModel.Series.OfType<ScatterSeries>())
            {
                series.Points.Clear();
            }

            // 按时间排序记录
            var sortedRecords = Records.OrderBy(r => r.Timestamp).ToList();

            var systolicSeries = PlotModel.Series[0] as ScatterSeries;
            var diastolicSeries = PlotModel.Series[1] as ScatterSeries;
            var heartRateSeries = PlotModel.Series[2] as ScatterSeries;

            // 根据用户选择显示/隐藏系列
            systolicSeries.IsVisible = ShowSystolic;
            diastolicSeries.IsVisible = ShowDiastolic;
            heartRateSeries.IsVisible = ShowHeartRate;

            // 添加数据点
            foreach (var record in sortedRecords)
            {
                var timeValue = DateTimeAxis.ToDouble(record.Timestamp);
                
                if (ShowSystolic)
                    systolicSeries?.Points.Add(new ScatterPoint(timeValue, record.Systolic));
                if (ShowDiastolic)
                    diastolicSeries?.Points.Add(new ScatterPoint(timeValue, record.Diastolic));
                if (ShowHeartRate)
                    heartRateSeries?.Points.Add(new ScatterPoint(timeValue, record.HeartRate));
            }

            // 刷新图表
            PlotModel.InvalidatePlot(true);
        }

        [RelayCommand]
        private void UpdateChart()
        {
            UpdatePlotModel();
        }

        // 当显示选项改变时更新图表
        partial void OnShowSystolicChanged(bool value)
        {
            if (_isInitialized)
            {
                UpdatePlotModel();
            }
        }

        partial void OnShowDiastolicChanged(bool value)
        {
            if (_isInitialized)
            {
                UpdatePlotModel();
            }
        }

        partial void OnShowHeartRateChanged(bool value)
        {
            if (_isInitialized)
            {
                UpdatePlotModel();
            }
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

                // 数据加载完成后更新图表
                // UpdatePlotModel();
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

                // 删除记录后更新图表
                // UpdatePlotModel();
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
