using AutoMapper;
using CompanyEmployees;
using Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CompanyEmployeesTests
{
    [TestClass]
    public class TestUnitBase
    {
        protected Mock<IRepositoryManager> _mockedRepositoryManager;
        protected Mock<IEmployeeRepository> _mockedEmployeeRepository;
        protected Mock<ICompanyRepository> _mockedCompanyRepository;
        protected Mock<ILoggerManager> _mockedLogger;
        protected IMapper _mapper;

        [TestInitialize]
        public void Initialize()
        {
            _mockedRepositoryManager = new Mock<IRepositoryManager>();
            _mockedEmployeeRepository = new Mock<IEmployeeRepository>();
            _mockedCompanyRepository = new Mock<ICompanyRepository>();
            _mockedLogger = new Mock<ILoggerManager>();

            _mockedRepositoryManager.Setup(a => a.Employee)
                .Returns(_mockedEmployeeRepository.Object);

            _mockedRepositoryManager.Setup(a => a.Company)
                .Returns(_mockedCompanyRepository.Object);

            var randsomeString = It.IsAny<string>();
            _mockedLogger.Setup(logger => logger.LogInfo(randsomeString));
            _mockedLogger.Setup(logger => logger.LogDebug(randsomeString));
            _mockedLogger.Setup(logger => logger.LogWarn(randsomeString));
            _mockedLogger.Setup(logger => logger.LogError(randsomeString));

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

        }
    }
}
