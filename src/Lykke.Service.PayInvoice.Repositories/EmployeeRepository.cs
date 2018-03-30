using System;
using System.Collections.Generic;
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
        
        public async Task<IReadOnlyList<Employee>> GetAsync(string merchantId)
        {
            IEnumerable<EmployeeEntity> entities = await _storage.GetDataAsync(GetPartitionKey(merchantId));

            var employees = Mapper.Map<List<Employee>>(entities);

            return employees;
        }

        public async Task<Employee> GetAsync(string merchantId, string employeeId)
        {
            EmployeeEntity entity = await _storage.GetDataAsync(GetPartitionKey(merchantId), GetRowKey(employeeId));

            var employee = Mapper.Map<Employee>(entity);

            return employee;
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

            var index = AzureIndex.Create(GetEmailIndexPartitionKey(), GetEmailIndexRowKey(entity.Email), entity);

            await _indexStorage.InsertAsync(index);

            return await GetAsync(entity.MerchantId, entity.RowKey);
        }

        public async Task UpdateAsync(Employee employee)
        {
            await _storage.MergeAsync(GetPartitionKey(employee.MerchantId), GetRowKey(employee.Id), entity =>
            {
                Mapper.Map(employee, entity);
                return entity;
            });
        }

        public async Task DeleteAsync(string merchantId, string employeeId)
        {
            EmployeeEntity entity = await _storage.GetDataAsync(GetPartitionKey(merchantId), GetRowKey(employeeId));
            
            if(entity == null)
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