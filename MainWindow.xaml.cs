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

			// Ensure previous operation is canceled before starting a new one
			_cts?.Cancel();
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			await Task.Delay(4000, token); // Wait for 4 seconds before starting, can be canceled

			await SimulateTypingAsync(textToType, token);
		}

		private async Task SimulateTypingAsync(string text, CancellationToken token)
		{
			var simulator = new InputSimulator();
			Random random = new Random();

			foreach (char c in text)
			{
				if (token.IsCancellationRequested) return; // Stop if canceled

				if (c == '\n')
				{
					simulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
				}
				else
				{
					simulator.Keyboard.TextEntry(c);
				}

				int delay = random.Next(60, 81);
				await Task.Delay(delay, token); // Allow cancellation during delay
			}
		}

		private void btnStop_Click(object sender, RoutedEventArgs e)
		{
			_cts?.Cancel(); // Stop typing when the stop button is clicked
		}
	}
}
