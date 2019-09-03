using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using inCRM;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace inCRMTests
{
    [TestClass]
    public class DBTest
    {
        [TestMethod]
        public void AddCustomers()
        {
			DB.connString = @"Data Source=MIBOOK\SQLEXPRESS;Initial Catalog=miCRM;Integrated Security=False;Connect Timeout=5;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
			System.Security.SecureString pwd = new System.Security.SecureString();
			pwd.MakeReadOnly();
			DB.credentials = new SqlCredential("miraklis", pwd);
			Customer cust;
			for (int i = 0; i < 10000; i++) {
				cust = new Customer
				{
					FirstName = "First Name" + i.ToString(),
					LastName = "Last Name" + i.ToString(),
					DateOfBirth = DateTime.Now,
					CompanyId = ((i % 2) + 1)
				};
				DB.AddCustomer(cust);
			}
			Assert.IsTrue(true);
        }
    }
}
