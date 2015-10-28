﻿using System.Threading.Tasks;
using GoldInventory.Models;
using Parse;

namespace GoldInventory.ParseWrappers
{
    public class CompanyHelper
    {
        public Company GetCurrentCompany()
        {
            var currentUser = ParseUser.CurrentUser;
            if (currentUser == null)
                return null;
            
            return GetCompanyById(((ParseObject)(currentUser["CompanyId"]))?.ObjectId);
        }

        public Company GetCompanyById(string id)
        {
            var company = (ParseObject)GetRawParseCompanyObjectById(id);
            if (company == null)
                return null;

            return new Company
            {
                Name = company["Name"]?.ToString(),
                Id = company.ObjectId,
                Description = company["Description"]?.ToString(),
                AddressLine1 = company["AddressLine1"]?.ToString(),
                AddressLine2 = company["AddressLine2"]?.ToString(),
                City = company["City"]?.ToString(),
                State = company["State"]?.ToString(),
                Country = company["Country"]?.ToString()
            };
        }

        public object GetRawParseCompanyObjectById(string id)
        {
            var companyQuery = ParseObject.GetQuery("Company");
            return companyQuery.GetAsync(id).Result;
        }

        public async Task<object> SaveCompany(Company company)
        {
            var companyParseObject = new ParseObject("Company");
            if (!string.IsNullOrEmpty(company.Id))
                companyParseObject.ObjectId = company.Id;

            companyParseObject["Name"] = company.Name;
            companyParseObject["Description"] = company.Description;
            companyParseObject["AddressLine1"] = company.AddressLine1;
            companyParseObject["AddressLine2"] = company.AddressLine2;
            companyParseObject["City"] = company.City;
            companyParseObject["State"] = company.State;
            companyParseObject["Country"] = company.Country;
            await companyParseObject.SaveAsync();

            return companyParseObject;
        }
    }
}