using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Repositories;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly INoSQLTableStorage<EmployeeEntity> _storage;
        private readonly INoSQLTableStorage<AzureIndex> _indexStorage;

        public EmployeeRepository(
            INoSQLTableStorage<EmployeeEntity> storage,
            INoSQLTableStorage<AzureIndex> indexStorage)
        {
            _storage = storage;
            _indexStorage = indexStorage;
        }

        public async Task<IReadOnlyList<Employee>> GetAsync()
        {
            IEnumerable<EmployeeEntity> entities = await _storage.GetDataAsync();

            var employees = Mapper.Map<List<Employee>>(entities);

            return employees;
        }

        public async Task<Employee> GetAsync(string employeeId, string merchantId)
        {
            var entity = await _storage.GetDataAsync(GetPartitionKey(merchantId), GetRowKey(employeeId));

            return Mapper.Map<Employee>(entity);
        }

        public async Task<Employee> GetByIdAsync(string employeeId)
        {
            IEnumerable<EmployeeEntity> entities =
                await _storage.GetDataRowKeysOnlyAsync(new[] { GetRowKey(employeeId) });

            EmployeeEntity entity = entities.FirstOrDefault();

            if (entity == null)
                return null;

            return Mapper.Map<Employee>(entity);
        }

        public async Task<IReadOnlyList<Employee>> GetByMerchantIdAsync(string merchantId)
        {
            IEnumerable<EmployeeEntity> entities = await _storage.GetDataAsync(GetPartitionKey(merchantId));

            var employees = Mapper.Map<List<Employee>>(entities);

            return employees;
        }

        public async Task<Employee> FindAsync(string email)
        {
            AzureIndex index =
                await _indexStorage.GetDataAsync(GetEmailIndexPartitionKey(), GetEmailIndexRowKey(email));

            if (index == null)
                return null;

            EmployeeEntity entity = await _storage.GetDataAsync(index);
            
            var employee = Mapper.Map<Employee>(entity);

            return employee;
        }

        public async Task<Employee> InsertAsync(Employee employee)
        {
            var entity = new EmployeeEntity(GetPartitionKey(employee.MerchantId), CreateRowKey());
            
            Mapper.Map(employee, entity);

            await _storage.InsertAsync(entity);

            var emailIndex = AzureIndex.Create(GetEmailIndexPartitionKey(), GetEmailIndexRowKey(entity.Email), entity);

            await _indexStorage.InsertAsync(emailIndex);

            return Mapper.Map<Employee>(entity);
        }

        public async Task MarkDeletedAsync(string merchantId, string employeeId)
        {
            await _storage.MergeAsync(GetPartitionKey(merchantId), GetRowKey(employeeId), entity =>
            {
                entity.IsDeleted = true;
                return entity;
            });
        }

        public async Task UpdateAsync(Employee employee, string previousEmail)
        {
            var entity = await _storage.MergeAsync(GetPartitionKey(employee.MerchantId), GetRowKey(employee.Id), mergingEntity =>
            {
                Mapper.Map(employee, mergingEntity);
                return mergingEntity;
            });

            // update email index
            if (!employee.Email.Equals(previousEmail, StringComparison.InvariantCultureIgnoreCase))
            {
                await _indexStorage.DeleteAsync(GetEmailIndexPartitionKey(), GetEmailIndexRowKey(previousEmail));

                var emailIndex = AzureIndex.Create(GetEmailIndexPartitionKey(), GetEmailIndexRowKey(entity.Email), entity);

                await _indexStorage.InsertAsync(emailIndex);
            }
        }

        public async Task DeleteAsync(string employeeId)
        {
            IEnumerable<EmployeeEntity> entities =
                await _storage.GetDataRowKeysOnlyAsync(new[] { GetRowKey(employeeId) });

            EmployeeEntity entity = entities.FirstOrDefault();

            if (entity == null)
                return;

            await _storage.DeleteAsync(entity);

            await _indexStorage.DeleteAsync(GetEmailIndexPartitionKey(), GetEmailIndexRowKey(entity.Email));
        }

        private static string GetPartitionKey(string merchantId)
            => merchantId;

        private static string GetRowKey(string employeeId)
            => employeeId;
        
        private static string CreateRowKey()
            => Guid.NewGuid().ToString("D");
        
        private static string GetEmailIndexPartitionKey()
            => "EmailIndex";

        private static string GetEmailIndexRowKey(string email)
            => email.ToLower().Trim();
    }
}
