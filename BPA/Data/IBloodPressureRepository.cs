using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BPA.Models;

namespace BPA.Data
{
    public interface IBloodPressureRepository
    {
        /// <summary>
        /// 获取所有血压记录
        /// </summary>
        Task<IEnumerable<BloodPressureRecord>> GetAllRecordsAsync();

        /// <summary>
        /// 根据ID获取血压记录
        /// </summary>
        Task<BloodPressureRecord> GetRecordByIdAsync(int id);

        /// <summary>
        /// 添加新的血压记录
        /// </summary>
        Task AddRecordAsync(BloodPressureRecord record);

        /// <summary>
        /// 更新血压记录
        /// </summary>
        Task UpdateRecordAsync(BloodPressureRecord record);

        /// <summary>
        /// 删除血压记录
        /// </summary>
        Task DeleteRecordAsync(int id);

        /// <summary>
        /// 获取最近的记录
        /// </summary>
        /// <param name="count">要获取的记录数量</param>
        Task<IEnumerable<BloodPressureRecord>> GetRecentRecordsAsync(int count);
    }
} 