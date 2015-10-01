using System;
using System.Windows;
using System.Windows.Input;

namespace WpfMetroWindow
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml.
	/// </summary>
	public partial class MainWindow : Window
	{
		private static readonly TimeSpan s_doubleClick = TimeSpan.FromMilliseconds(500);
		private DateTime m_headerLastClicked;
	 
		public MainWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Handles the close click.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> 
		/// instance containing the event data.</param>
		private void HandleCloseClick(Object sender, RoutedEventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Handles the preview mouse move.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> 
		/// instance containing the event data.</param>
		private void HandlePreviewMouseMove(Object sender, MouseEventArgs e)
		{
			if (Mouse.LeftButton != MouseButtonState.Pressed)
			{
				Cursor = Cursors.Arrow;
			}
		}

		/// <summary>
		/// Handles the header preview mouse down.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/>
		/// instance containing the event data.</param>
		private void HandleHeaderPreviewMouseDown(Object sender, MouseButtonEventArgs e)
		{
			m_headerLastClicked = DateTime.Now;

			if (Mouse.LeftButton == MouseButtonState.Pressed)
			{
				DragMove();
			}
		}
	}
}
