using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace inCRM
{
    public class LoginViewModel: ValidatedViewModel
    {

        #region Constructors

        public LoginViewModel()
        {
			templateConnString = System.Configuration.ConfigurationManager.ConnectionStrings["dbCRMConStr"].ConnectionString;

			//TEST
			SQLServer = @".\SQLExpress";
			Username = "miraklis";			
		}

		#endregion

		#region Private Members

		private readonly string templateConnString;

		#endregion

		#region Commands

		private RelayCommand<bool> closeWindowCommand;
		public RelayCommand<bool> CloseWindowCommand
		{
			get => closeWindowCommand ?? (closeWindowCommand = new RelayCommand<bool>(CloseWindow));
		}
		private void CloseWindow(bool responce)
		{
			View?.CloseDialog(responce);
		}

		private RelayCommand<PasswordBox> loginCommand;
		public RelayCommand<PasswordBox> LoginCommand
		{
			get => loginCommand ?? (loginCommand = new RelayCommand<PasswordBox>(Login));
		}
		private void Login(PasswordBox pb)
		{
			ConnectionEstablished = false;

			// Validate UserInput
			if (!ValidateData())
				return;

			// Construct SQL Connection string
			SqlConnectionStringBuilder connString = new SqlConnectionStringBuilder(templateConnString);
			connString.DataSource = this.SQLServer;
			DB.connString = connString.ToString();

			// Construct SQL Credentials (username, password)
			SecureString password = pb.SecurePassword;
			password.MakeReadOnly();
			DB.credentials = new SqlCredential(Username, password);
			// Test Connection
			ConnectionEstablished = DB.IsConnected();
			CloseWindowCommand.Execute(true);
		}

		private RelayCommand<object> logoutCommand;
		public RelayCommand<object> LogoutCommand
		{
			get => logoutCommand ?? (logoutCommand = new RelayCommand<object>(Logout));
		}
		private void Logout(object obj)
		{
			Username = String.Empty;
			SecureString password = new SecureString();
			password.MakeReadOnly();
			DB.credentials = new SqlCredential(Username, password);
			ConnectionEstablished = false;
		}

		#endregion

		#region Public Properties

		public IDialogView View { get; set; }

		private string sqlServer;
		public string SQLServer
        {
            get
            {
				return sqlServer;
            }
            set
            {
				if (sqlServer != value) {
					sqlServer = value;
					OnPropertyChanged();
					ValidateSQLServer();
				}
            }
        }

		private string userName;		
        public string Username
        {
            get
            {
				return userName;
            }
            set
            {
				if (userName != value) {
					userName = value;
					OnPropertyChanged();
					ValidateUsername();
				}
            }
        }

        public bool ConnectionEstablished
        {
            get
            {
				return Globals.GetInstance().ConnectionEstablished;
            }
            set
            {
				if (Globals.GetInstance().ConnectionEstablished != value) {
					Globals.GetInstance().ConnectionEstablished = value;
					OnPropertyChanged();
				}
			}
		}

        #endregion

		#region Validation

		private void ValidateUsername()
        {
            List<string> propertyErrors = new List<string>();
            if (String.IsNullOrWhiteSpace(Username))
                propertyErrors.Add("User Name cannot be empty");
            SetErrors(nameof(Username), propertyErrors);
        }

        private void ValidateSQLServer()
        {
            List<string> propertyErrors = new List<string>();
            if (String.IsNullOrWhiteSpace(SQLServer))
                propertyErrors.Add("SQL Server Address cannot be empty");
            SetErrors(nameof(SQLServer), propertyErrors);
        }

        private bool ValidateData()
        {
            ValidateUsername();
            ValidateSQLServer();
            if (HasErrors) {
                StringBuilder msg = new StringBuilder();
                foreach (var k in validationErrors) {
                    foreach (var v in k.Value) {
                        msg.Append(v);
                        msg.Append(Environment.NewLine);
                    }
                }
                MessageBox.Show(msg.ToString(), "Validation Errors");
            }
            return !HasErrors;
        }

        #endregion

    }
}
