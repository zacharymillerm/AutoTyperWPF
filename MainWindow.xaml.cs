using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WindowsInput;
using WindowsInput.Native;
using System.Runtime.InteropServices;

namespace AutoPiper
{
	public partial class MainWindow : Window
	{
		[DllImport("User32.dll")]
		private static extern int SetForegroundWindow(IntPtr point);

		private CancellationTokenSource _cts = new CancellationTokenSource();

		public MainWindow()
		{
			InitializeComponent();
		}

		private async void btnSend_Click(object sender, RoutedEventArgs e)
		{
			string textToType = txtInput.Text;

			if (string.IsNullOrWhiteSpace(textToType))
			{
				MessageBox.Show("Please enter text to send.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			_cts?.Cancel();
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				await Task.Delay(4000, token); // Wait for 4 seconds before starting, can be canceled
				await SimulateTypingAsync(textToType, token);
			}
			catch (TaskCanceledException)
			{
				// Task was canceled, safely exit without crashing
			}
		}

		private async Task SimulateTypingAsync(string text, CancellationToken token)
		{
			var simulator = new InputSimulator();
			Random random = new Random();
			int speed = cbxDouble.IsChecked == true ? 1 : 2;

			foreach (char c in text)
			{
				if (token.IsCancellationRequested) return;

				if (c == '\n')
				{
					simulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
				}
				else
				{
					simulator.Keyboard.TextEntry(c);
				}

				int delay = random.Next(35 * speed, 50 * speed + 1);
				try
				{
					await Task.Delay(delay, token);
				}
				catch (TaskCanceledException)
				{
					return; // Stop execution without crashing
				}
			}
		}

		private void btnStop_Click(object sender, RoutedEventArgs e)
		{
			_cts?.Cancel();
		}
	}
}
