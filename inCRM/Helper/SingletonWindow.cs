using System;
using System.Windows;

namespace inCRM
{
	public class SingletonWindow<T>: Window where T : class, new()
	{
		private static T instance = null;

		public SingletonWindow() : base()
		{
			base.Closed += Window_Closed;
		}

		public static T CreateWindow()
		{
			if (instance is null)
				instance = new T();
			return instance;
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			instance = null;
		}

	}
}
