using Contracts;
using Entities.DataTransferObjects;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CompanyEmployees.Utility
{
    public class EmployeeLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<EmployeeDto> _dataShaper;

        public EmployeeLinks(
            LinkGenerator linkGenerator,
            IDataShaper<EmployeeDto> dataShaper
        )
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }

        public LinkResponse TryGenerateLinks(
            IEnumerable<EmployeeDto> employeeDtos,
            string fields,
            Guid companyId,
            HttpContext httpContext
        )
        {
            var shapedEmployees = ShapeData(employeeDtos, fields);
            if (ShouldGenerateLinks(httpContext))
            {
                return ReturnLinkedEmployees(employeeDtos, fields, companyId, httpContext, shapedEmployees);
            }

            return ReturnShapedEmployees(shapedEmployees);
        }

        private List<Entity> ShapeData(IEnumerable<EmployeeDto> employeesDtos, string fields)
            =>
                _dataShaper.ShapeData(employeesDtos, fields)
                .Select(data => data.Entity)
                .ToList();

        private bool ShouldGenerateLinks(HttpContext httpContext)
        {
            var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];

            return mediaType.MediaType.Contains("hateoas", StringComparison.InvariantCultureIgnoreCase);
        }

        private LinkResponse ReturnShapedEmployees(List<Entity> shapedEmployees)
            =>
                new LinkResponse { ShapedEntities = shapedEmployees };

        private LinkResponse ReturnLinkedEmployees(
            IEnumerable<EmployeeDto> employeesDtos,
            string fields,
            Guid companyId,
            HttpContext httpContext,
            List<Entity> shapedEmployees
            )
        {
            var employeeDtoList = employeesDtos.ToList();

            for (var index = 0; index < employeeDtoList.Count(); index++)
            {
                var employeeLinks = CreateLinksForEmployee(
                    httpContext,
                    companyId,
                    employeeDtoList[index].Id,
                    fields
                );
                shapedEmployees[index].Add("Links", employeeLinks);
            }

            var employeeCollection = new LinkCollectionWrapper<Entity>(shapedEmployees);
            var linkedEmployees = CreateLinksForEmployees(httpContext, employeeCollection);
            return new LinkResponse { HasLinks = true, LinkedEntities = linkedEmployees };
        }

        private List<Link> CreateLinksForEmployee(HttpContext httpContext, Guid companyId, Guid id, string fields = "")
        {
            var links = new List<Link>
            {
                new Link(
                    _linkGenerator.GetUriByAction(
                        httpContext,
                        "GetEmployeeForCompany",
                        values: new { companyId, id, fields }
                    ),
                    "self",
                    "GET"
                 ),
                new Link(
                    _linkGenerator.GetUriByAction(
                        httpContext,
                        "DeleteEmployeeForCompany",
                        values: new { companyId, id }
                    ),
                    "delete_employee",
                    "DELETE"
                ),
                new Link(
                    _linkGenerator.GetUriByAction(
                        httpContext,
                        "UpdateEmployeeForCompany",
                        values: new { companyId, id }
                    ),
                    "update_employee",
                    "PUT"
                ),
                new Link(
                    _linkGenerator.GetUriByAction(
                        httpContext,
                        "PartiallyUpdateEmployeeForCompany",
                        values: new { companyId, id }
                    ),
                    "partially_update_employee",
                    "PATCH"
                )
            };

            return links;
        }

        private LinkCollectionWrapper<Entity> CreateLinksForEmployees(HttpContext httpContext, LinkCollectionWrapper<Entity> employeesWrapper)
        {
            employeesWrapper.Links.Add(
                new Link(
                    _linkGenerator.GetUriByAction(
                        httpContext,
                        "GetEmployeesForCompany",
                        values: new { }
                    ),
                    "self",
                    "GET"
                )
            );

            return employeesWrapper;
        }
    }
}
