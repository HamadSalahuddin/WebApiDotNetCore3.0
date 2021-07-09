using AutoMapper;
using CompanyEmployees;
using CompanyEmployees.Controllers;
using Contracts;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CompanyEmployeesTests
{
    [TestClass]
    public class EmployeesTests : TestUnitBase
    {
        [TestMethod]
        public async Task EmployeesController_GetEmployeesForCompany_ShouldNot_ReturnEmployeesWhenAgeRangeIsInvalid()
        {
            var companyId = Guid.NewGuid();
            
            var employeeRequest = new EmployeeParameters
            {
                MaxAge = 27,
                MinAge = 29
            };

            EmployeesController employeesController = new EmployeesController(
                _mockedRepositoryManager.Object,
                _mockedLogger.Object,
                _mapper,
                null,
                null
            );

            var badRequestObjectResult = await employeesController.GetEmployeesForCompanyAsync(companyId, employeeRequest) as BadRequestObjectResult;
            
            using var scope = new AssertionScope();

            badRequestObjectResult
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.BadRequest);

            var logMessage = "Max age can't be less than min age.";
            badRequestObjectResult
                .Value
                .Should()
                .BeEquivalentTo(logMessage);
        }

        [TestMethod]
        public async Task EmployeesController_GetEmployeesForCompany_ShouldNot_ReturnEmployeesWhenCompanyNotFound()
        {
            var companyId = Guid.NewGuid(); 
            var trackChanges = false;

            var employeeRequest = new EmployeeParameters();

            EmployeesController employeesController = new EmployeesController(
                _mockedRepositoryManager.Object,
                _mockedLogger.Object,
                _mapper,
                null,
                null
            );

            _mockedCompanyRepository.Setup(
                companyRepo => companyRepo.GetCompanyAsync(companyId, trackChanges)
           )
                .Returns(
                    Task.FromResult<Entities.Models.Company>(
                        null
                    )
           );

            var notFoundObjectResult = await employeesController.GetEmployeesForCompanyAsync(companyId, employeeRequest) as NotFoundObjectResult;

            using var scope = new AssertionScope();

            notFoundObjectResult
                .StatusCode
                .Should()
                .Be((int)HttpStatusCode.NotFound);

            var logMessage = $"Company with id: { companyId } doesn't exist in the database.";
            notFoundObjectResult
                .Value
                .Should()
                .BeEquivalentTo(logMessage);
        }

        [TestMethod]
        public async Task EmployeesController_GetEmployeesForCompany_ShouldNot_ReturnEmployeesWithPaging()
        {
            var companyId = Guid.NewGuid();
            var trackChanges = false;

            var employeeRequest = new EmployeeParameters
            {
                PageNumber = 1,
                PageSize = 3,
            };

            EmployeesController employeesController = new EmployeesController(
                _mockedRepositoryManager.Object,
                _mockedLogger.Object,
                _mapper,
                null,
                null
            );

            _mockedCompanyRepository.Setup(
                companyRepo => companyRepo.GetCompanyAsync(companyId, trackChanges)
           )
                .Returns(
                    Task.FromResult<Entities.Models.Company>(
                        null
                    )
           );

            var notFoundObjectResult = await employeesController.GetEmployeesForCompanyAsync(companyId, employeeRequest) as NotFoundObjectResult;

            using var scope = new AssertionScope();

            notFoundObjectResult
                .StatusCode
                .Should()
                .Be((int)HttpStatusCode.NotFound);

            var logMessage = $"Company with id: { companyId } doesn't exist in the database.";
            notFoundObjectResult
                .Value
                .Should()
                .BeEquivalentTo(logMessage);
        }

        [TestMethod]
        public async Task EmployeesController_GetEmployeeForCompany_Should_ReturnTheEmployee()
        {
            var companyId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var trackChanges = false;

            var employeeEntity = new Entities.Models.Employee
            {
                CompanyId = companyId,
                Id = employeeId,
                Age = 23,
                Name = "Test Employee",
                Position = "Manager"
            };

            
            _mockedEmployeeRepository.Setup(
                employeeRepo => employeeRepo.GetEmployeeAsync(companyId, employeeId, trackChanges)
            )
                .Returns(
                    Task.FromResult(
                        employeeEntity
                    )
             );

            _mockedCompanyRepository.Setup(
                companyRepo => companyRepo.GetCompanyAsync(companyId, trackChanges)
           )
                .Returns(
                    Task.FromResult(
                        new Entities.Models.Company
                        {
                            Id = companyId,
                            Address = "test address",
                            Country = "Test Country",
                            Employees = null,
                            Name = "Test company Name"
                        }
                    )
           );
            
            EmployeesController employeesController = new EmployeesController(
                _mockedRepositoryManager.Object,
                _mockedLogger.Object,
                _mapper,
                null,
                null
            );

            var okObjectResult = await employeesController.GetEmployeeForCompany(companyId, employeeId) as OkObjectResult;
            var objectResult = okObjectResult as ObjectResult;
            var response = objectResult.Value as EmployeeDto;

            using var scope = new AssertionScope();

            okObjectResult
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.OK);

            var expectedEmployeeDto = _mapper.Map<EmployeeDto>(employeeEntity);

            response
                .Should()
                .BeEquivalentTo(expectedEmployeeDto);
        }

        [TestMethod]
        public async Task EmployeesController_GetEmployeeForCompany_Should_ReturnNotFoundWhenCompanyNotFound()
        {
            var companyId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var trackChanges = false;

            _mockedCompanyRepository.Setup(
                companyRepo => companyRepo.GetCompanyAsync(companyId, trackChanges)
            )
                .Returns(
                    Task.FromResult<Entities.Models.Company>(
                        null
                    )
                );

            EmployeesController employeesController = new EmployeesController(
                _mockedRepositoryManager.Object,
                _mockedLogger.Object,
                _mapper,
                null,
                null
            );

            var notFoundObjectResult = await employeesController.GetEmployeeForCompany(companyId, employeeId) as NotFoundObjectResult;
            
            using var scope = new AssertionScope();

            notFoundObjectResult
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.NotFound);

            var notFoundMessage = $"Company with id: {companyId} doesn't exist in the database.";
            notFoundObjectResult
                .Value
                .Should()
                .BeEquivalentTo(notFoundMessage, because: notFoundMessage);
        }

        [TestMethod]
        public async Task EmployeesController_GetEmployeeForCompany_Should_ReturnNotFoundWhenEmployeeNotFound()
        {
            var companyId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var trackChanges = false;

            _mockedCompanyRepository.Setup(
                companyRepo => companyRepo.GetCompanyAsync(companyId, trackChanges)
            )
                .Returns(
                    Task.FromResult(
                        new Entities.Models.Company
                        {
                            Id = companyId,
                            Address = "test address",
                            Country = "Test Country",
                            Employees = null,
                            Name = "Test company Name"
                        }
                    )
                );

            _mockedEmployeeRepository.Setup(
                employeeRepo => employeeRepo.GetEmployeeAsync(companyId, employeeId, trackChanges)
            )
                .Returns(
                    Task.FromResult<Entities.Models.Employee>(
                        null
                    )
                );


            EmployeesController employeesController = new EmployeesController(
                _mockedRepositoryManager.Object,
                _mockedLogger.Object,
                _mapper,
                null,
                null
            );

            var notFoundObjectResult = await employeesController.GetEmployeeForCompany(companyId, employeeId) as NotFoundObjectResult;

            using var scope = new AssertionScope();

            notFoundObjectResult
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.NotFound);

            var notFoundMessage = $"Employee with id: {employeeId} doesn't exist in the database.";
            notFoundObjectResult
                .Value
                .Should()
                .BeEquivalentTo(notFoundMessage, because: notFoundMessage);
        }
    }
}
