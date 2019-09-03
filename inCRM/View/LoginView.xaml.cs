using System.Windows;


namespace inCRM
{
    public partial class LoginView : Window, IDialogView
    {
        public LoginView(LoginViewModel loginVM)
        {
            InitializeComponent();
			loginVM.View = this;
            DataContext = loginVM;
        }

		public Window GetInstance()
		{
			return this;
		}

		public void CloseDialog(bool responce)
		{
			this.DialogResult = responce;
		}
    }

}
