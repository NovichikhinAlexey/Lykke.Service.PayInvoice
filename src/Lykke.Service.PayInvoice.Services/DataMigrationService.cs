using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.PayInvoice.Core.Domain.DataMigrations;
using Lykke.Service.PayInvoice.Core.Repositories;
using Lykke.Service.PayInvoice.Core.Services;

namespace Lykke.Service.PayInvoice.Services
{
    public class DataMigrationService : IDataMigrationService
    {
        private readonly IDataMigrationRepository _dataMigrationRepository;
        private readonly IDataMigrationOneDotSeven _dataMigrationOneDotSeven;
        private readonly ILog _log;
        private readonly IReadOnlyList<string> ValidMigrations;
        private static readonly ConcurrentBag<string> _executingMigrations = new ConcurrentBag<string>();

        public DataMigrationService(
            IDataMigrationRepository dataMigrationRepository,
            IDataMigrationOneDotSeven dataMigrationOneDotSeven,
            ILog log)
        {
            _dataMigrationRepository = dataMigrationRepository;
            _dataMigrationOneDotSeven = dataMigrationOneDotSeven;
            _log = log.CreateComponentScope(nameof(DataMigrationService));

            ValidMigrations = new List<string>
            {
                _dataMigrationOneDotSeven.Name
            };
        }

        public Task<IReadOnlyList<string>> GetAll()
        {
            return _dataMigrationRepository.GetAllAsync();
        }

        public async Task<DataMigrationResult> ExecuteAsync(string migrationName)
        {
            LogInfo("Called migration execution");

            if (!ValidMigrations.Contains(migrationName))
            {
                var info = "Migration name is not valid";
                LogInfo(info);
                return new DataMigrationResult { HasExecuted = false, Info = info };
            }

            if (await _dataMigrationRepository.IsExistAsync(migrationName))
            {
                var info = "Such migration has been already executed";
                LogInfo(info);
                return new DataMigrationResult { HasExecuted = false, Info = info };
            }
            
            if (_executingMigrations.Contains(migrationName))
            {
                var info = "Migration is executing";
                LogInfo(info);
                return new DataMigrationResult { HasExecuted = false, Info = info };
            }
            
            _executingMigrations.Add(migrationName);

            try
            {
                switch (migrationName)
                {
                    case string s when (s == _dataMigrationOneDotSeven.Name):
                        await _dataMigrationOneDotSeven.ExecuteAsync();
                        break;
                    default:
                        throw new KeyNotFoundException(migrationName);
                }
            }
            catch (Exception ex)
            {
                _log.WriteError(nameof(ExecuteAsync), migrationName, ex);
                return new DataMigrationResult { HasExecuted = false, Info = "Error occured during execution" };
            }
            finally
            {
                _executingMigrations.TryTake(out migrationName);
            }

            return new DataMigrationResult { HasExecuted = true, Info = "Migration has been successfully executed" };

            void LogInfo(string info)
            {
                _log.WriteInfo(nameof(ExecuteAsync), migrationName, info);
            }
        }
    }
}