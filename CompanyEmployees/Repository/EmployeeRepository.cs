using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository 
    { 
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext) 
        {

        }

        public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges) =>
            FindByCondition(employee => employee.CompanyId.Equals(companyId), trackChanges)
                .OrderBy(employee => employee.Name);

        public Employee GetEmployee(Guid companyId, Guid id, bool trackChanges) => 
            FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
                .SingleOrDefault();

        public void CreateEmployeeForCompany(Guid CompanyId, Employee employee)
        {
            employee.CompanyId = CompanyId;
            Create(employee);

        }
    }
}
