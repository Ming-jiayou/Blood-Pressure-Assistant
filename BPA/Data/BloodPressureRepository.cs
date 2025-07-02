using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BPA.Models;
using Microsoft.EntityFrameworkCore;

namespace BPA.Data
{
    public class BloodPressureRepository : IBloodPressureRepository
    {
        private readonly BPADbContext _context;

        public BloodPressureRepository(BPADbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<BloodPressureRecord>> GetAllRecordsAsync()
        {
            return await _context.BloodPressureRecords
                .OrderByDescending(r => r.Timestamp)
                .ToListAsync();
        }

        public async Task<BloodPressureRecord> GetRecordByIdAsync(int id)
        {
            return await _context.BloodPressureRecords.FindAsync(id);
        }

        public async Task AddRecordAsync(BloodPressureRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            await _context.BloodPressureRecords.AddAsync(record);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRecordAsync(BloodPressureRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            _context.Entry(record).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRecordAsync(int id)
        {
            var record = await _context.BloodPressureRecords.FindAsync(id);
            if (record != null)
            {
                _context.BloodPressureRecords.Remove(record);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<BloodPressureRecord>> GetRecentRecordsAsync(int count)
        {
            return await _context.BloodPressureRecords
                .OrderByDescending(r => r.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodPressureRecord>> GetRecordsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.BloodPressureRecords
                .Where(r => r.Timestamp >= startDate && r.Timestamp <= endDate)
                .OrderByDescending(r => r.Timestamp)
                .ToListAsync();
        }
    }
} 