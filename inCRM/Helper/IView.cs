using System.Windows;

namespace inCRM
{
	public interface IView
	{
		Window GetInstance();
	}

	public interface IDialogView: IView
	{
		void CloseDialog(bool responce = false);
	}

	public interface IWindowView : IView
	{
		void CloseWindow();
	}

}
