using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class CompanyRepository: RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext)
            :base(repositoryContext)
        {

        }

        public IEnumerable<Company> GetAllCompanies(bool trackChanges) =>
            FindAll(trackChanges)
            .OrderBy(Company => Company.Name)
            .ToList();

        public Company GetCompany(Guid companyId, bool trackChanges) =>
            FindByCondition(company => company.Id.Equals(companyId), trackChanges)
            .SingleOrDefault();

        public void CreateCompany(Company company) => Create(company);

        public IEnumerable<Company> GetByIds(IEnumerable<Guid> Ids, bool trackChanges) =>
            FindByCondition(company => Ids.Contains(company.Id), trackChanges: trackChanges)
            .ToList();
    }
}
