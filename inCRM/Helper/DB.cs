using System;
using System.Collections.Generic;
using DBHandler;
using System.Windows;
using System.Data.SqlClient;

namespace inCRM
{
    public static class DB
    {

        #region General Methods

        public static string connString { get; set; }
        public static SqlCredential credentials;

        public static bool IsConnected()
        {
			if (credentials.UserId == String.Empty)
				return false;
            using (SqlConnection con = new SqlConnection(connString, credentials))
            {
                try
                {
                    con.Open();
                    return DBActions.IsConnected(con);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static string CurrentUser()
        {
			if (credentials.UserId != String.Empty)
				return credentials.UserId;
			using (SqlConnection con = new SqlConnection(connString, credentials))
            {
                try
                {
                    return DBActions.CurrentUser(con);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        #endregion

        #region Customers Methods

        public static Customer GetCustomer(int id)
        {
            using (SqlConnection con = new SqlConnection(connString, credentials))
            {
                try
                {
                    con.Open();
                    Dictionary<string, object> p = new Dictionary<string, object>
                    {
                        { "@Id", id }
                    };
                    return DBActions.ExecReaderSP<Customer>(con, "getCustomerById", p);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static List<Customer> GetCustomers()
        {
            using (SqlConnection con = new SqlConnection(connString, credentials))
            {
                try
                {
                    con.Open();
                    return DBActions.ExecReaderListSP<Customer>(con, "getAllCustomers");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }
        }

		public static List<CustomerItemVM> GetCustomerItemsVM()
		{
			using (SqlConnection con = new SqlConnection(connString, credentials)) {
				try {
					string query = @"SELECT cust.Id, cust.FirstName, cust.LastName, cust.DateOfBirth, comp.Name as CompanyName " +
									"FROM Customers cust " +
									"LEFT JOIN Companies comp " +
										"ON cust.CompanyId = comp.Id";
					con.Open();
					return DBActions.ExecReaderListQR<CustomerItemVM>(con, query, null);
				}
				catch (Exception ex) {
					MessageBox.Show(ex.Message);
					return null;
				}
				finally {
					con.Close();
				}
			}
		}

		public static int AddCustomer(Customer c)
        {
            using (SqlConnection con = new SqlConnection(connString, credentials))
            {
                try
                {
                    int retVal = -1;
                    Dictionary<string, object> inParams = new Dictionary<string, object>
                    {
                        { "@FirstName", c.FirstName },
                        { "@LastName", c.LastName },
                        { "@DateOfBirth", c.DateOfBirth },
                        { "@CompanyId", c.CompanyId },
                    };
                    Dictionary<string, object> outParams = new Dictionary<string, object>
                    {
                        { "@NewId", -1 }
                    };
                    con.Open();
                    retVal = (int)DBActions.ExecNonReaderWithReturnSP(con, "createNewCustomer", inParams, outParams);
                    return retVal;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return -1;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static int AddCustomerNumber(ContactDetails number)
        {
            using (SqlConnection con = new SqlConnection(connString, credentials))
            {
                try
                {
                    int retVal = -1;
                    Dictionary<string, object> inParams = new Dictionary<string, object>
                    {
                        { "@CustomerId", number.CustomerId },
                        { "@ContactTypeId", number.ContactTypeId },
                        { "@ContactInfo", number.ContactInfo }
                    };
                    Dictionary<string, object> outParams = new Dictionary<string, object>
                    {
                        { "@NewId", -1 }
                    };
                    con.Open();
                    retVal = (int)DBActions.ExecNonReaderWithReturnSP(con, "addCustomerNumber", inParams, outParams);
                    return retVal;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return -1;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static int UpdateCustomer(Customer c)
        {
            using (SqlConnection con = new SqlConnection(connString, credentials))
            {
                try
                {
                    Dictionary<string, object> p = new Dictionary<string, object>
                    {
                        { "@Id", c.Id },
                        { "@FirstName", c.FirstName },
                        { "@LastName", c.LastName },
                        { "@DateOfBirth", c.DateOfBirth },
                        { "@CompanyId", c.CompanyId }
                    };
                    con.Open();
                    return DBActions.ExecNonReaderSP(con, "updateCustomerByID", p);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return -1;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static int DeleteCustomer(int id)
        {
            using (SqlConnection con = new SqlConnection(connString, credentials))
            {
                try
                {
                    Dictionary<string, object> p = new Dictionary<string, object>
                    {
                        { "@Id", id }
                    };
                    con.Open();
                    return DBActions.ExecNonReaderSP(con, "removeCustomerByID", p);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return -1;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static List<ContactVM> GetCustomerNumbersList(int id)
        {
            using (SqlConnection con = new SqlConnection(connString, credentials))
            {
                try
                {
                    Dictionary<string, object> p = new Dictionary<string, object>
                    {
                        { "@Id", id }
                    };
                    string query = "SELECT ccd.Id, ct.Type, ccd.ContactTypeId, ccd.ContactInfo " +
                                        "FROM CustomerContactDetails ccd " +
                                        "INNER JOIN ContactType ct " +
                                            "ON ccd.ContactTypeId=ct.Id " +
                                        "WHERE ccd.CustomerId = @Id";
                    con.Open();
                    return DBActions.ExecReaderListQR<ContactVM>(con, query, p);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static List<ContactDetails> GetCustomerNumbers(int id)
        {
            using (SqlConnection con = new SqlConnection(connString, credentials))
            {
                try
                {
                    Dictionary<string, object> p = new Dictionary<string, object>
                    {
                        { "@Id", id }
                    };
                    string query = "SELECT Id, CustomerId, ContactTypeId, ContactInfo " +
                                        "FROM CustomerContactDetails " +
                                        "WHERE CustomerId = @Id";
                    con.Open();
                    return DBActions.ExecReaderListQR<ContactDetails>(con, query, p);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static int DeleteCustomerNumbers(int id)
        {
            using (SqlConnection con = new SqlConnection(connString, credentials))
            {
                try
                {
                    Dictionary<string, object> p = new Dictionary<string, object>
                    {
                        { "@Id", id }
                    };
                    string cmdText = "DELETE " +
                                    "FROM CustomerContactDetails " +
                                    "WHERE CustomerId = @Id";
                    con.Open();
                    return DBActions.ExecNonReaderQR(con, cmdText, p);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return -1;
                }
                finally
                {
                    con.Close();
                }
            }
        }

		public static int AddCustomerAddress(AddressDetails address)
		{
			using (SqlConnection con = new SqlConnection(connString, credentials)) 
			{
				try 
				{
					int retVal = -1;
					Dictionary<string, object> inParams = new Dictionary<string, object>
					{
						{ "@CustomerId", address.CustomerId },
						{ "@Road", address.Road },
						{ "@Number", address.Number },
						{ "@City", address.City },
						{ "@PostalCode", address.PostalCode },
						{ "@Region", address.Region },
						{ "@state", address.State },
						{ "@Country", address.Country }
					};
					Dictionary<string, object> outParams = new Dictionary<string, object>
					{
						{ "@NewId", -1 }
					};
					con.Open();
					retVal = (int)DBActions.ExecNonReaderWithReturnSP(con, "addCustomerAddress", inParams, outParams);
					return retVal;
				}
				catch (Exception ex) 
				{
					MessageBox.Show(ex.Message);
					return -1;
				}
				finally 
				{
					con.Close();
				}
			}
		}

		public static List<AddressVM> GetCustomerAddressList(int id)
		{
			using (SqlConnection con = new SqlConnection(connString, credentials)) 
			{
				try 
				{
					Dictionary<string, object> p = new Dictionary<string, object>
					{
						{ "@Id", id }
					};
					string query = "SELECT Id, Road, Number, City, Region, State, PostalCode, Country " +
										"FROM CustomerAddress " +
										"WHERE CustomerId = @Id";
					con.Open();
					return DBActions.ExecReaderListQR<AddressVM>(con, query, p);
				}
				catch (Exception ex) 
				{
					MessageBox.Show(ex.Message);
					return null;
				}
				finally 
				{
					con.Close();
				}
			}
		}

		public static List<AddressDetails> GetCustomerAddresses(int id)
		{
			using (SqlConnection con = new SqlConnection(connString, credentials)) {
				try {
					Dictionary<string, object> p = new Dictionary<string, object>
					{
						{ "@Id", id }
					};
					string query = "SELECT Id, CustomerId, Road, Number, City, Region, State, PostalCode, Country " +
										"FROM CustomerAddress " +
										"WHERE CustomerId = @Id";
					con.Open();
					return DBActions.ExecReaderListQR<AddressDetails>(con, query, p);
				}
				catch (Exception ex) {
					MessageBox.Show(ex.Message);
					return null;
				}
				finally {
					con.Close();
				}
			}
		}

		public static int DeleteCustomerAddresses(int id)
		{
			using (SqlConnection con = new SqlConnection(connString, credentials)) 
			{
				try 
				{
					Dictionary<string, object> p = new Dictionary<string, object>
					{
						{ "@Id", id }
					};
					string cmdText = "DELETE " +
									"FROM CustomerAddress " +
									"WHERE CustomerId = @Id";
					con.Open();
					return DBActions.ExecNonReaderQR(con, cmdText, p);
				}
				catch (Exception ex) 
				{
					MessageBox.Show(ex.Message);
					return -1;
				}
				finally 
				{
					con.Close();
				}
			}
		}

		#endregion

		#region Companies

		public static List<Company> GetAllCompanies()
        {
            using (SqlConnection con = new SqlConnection(connString, credentials))
            {
                try
                {
                    string query = "SELECT Id, Name, Title, Country " +
                                        "FROM Companies";
                    con.Open();
                    return DBActions.ExecReaderListQR<Company>(con, query, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static Company GetCompany(int? id)
        {
            using (SqlConnection con = new SqlConnection(connString, credentials))
            {
                try
                {
                    if (id is null)
                        return null;
                    Dictionary<string, object> p = new Dictionary<string, object>
                    {
                        { "@Id", id }
                    };
                    string query = "SELECT Id, Name, Title, Country " +
                                        "FROM Companies " +
                                   "WHERE Id = @Id";
                    con.Open();
                    return DBActions.ExecReaderQR<Company>(con, query, p);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        #endregion

        #region Contacts

        public static List<ContactType> GetContactTypes()
        {
            using (SqlConnection con = new SqlConnection(connString, credentials))
            {
                try
                {
                    string query = "SELECT Id, Type " +
                                        "FROM ContactType";
                    con.Open();
                    return DBActions.ExecReaderListQR<ContactType>(con, query, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }
        }
        #endregion
    }
}
