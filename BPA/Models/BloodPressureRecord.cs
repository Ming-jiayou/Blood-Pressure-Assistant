using System;

namespace BPA.Models
{
    public class BloodPressureRecord
    {
        public int Id { get; set; }  // 主键
        public int Systolic { get; set; }  // 收缩压（高压）
        public int Diastolic { get; set; }  // 舒张压（低压）
        public int HeartRate { get; set; }  // 心率
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
} 