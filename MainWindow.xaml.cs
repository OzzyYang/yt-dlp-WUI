using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using yt_dlp_WUI.tools;
using CliWrap;
using CliWrap.Buffered;
using System.CodeDom.Compiler;
using CliWrap.EventStream;
using System.Text.RegularExpressions;

namespace yt_dlp_UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		string path = "D:\\Videos\\Youtube";
		bool isTestFlag = false;
		MyLog myLog;
		StringBuilder stdOutBuffer = new StringBuilder();
		StringBuilder stdErrBuffer = new StringBuilder();
		String tipContent = "Waiting ... ";
		public MainWindow()
		{
			InitializeComponent();
			myLog = new MyLog(messageBox);
			dlProgressBar.Value = 0;
		}


		private void download(object sender, RoutedEventArgs e)
		{
			dlProgressBar.Value = 0;
			downloadButton.Visibility = Visibility.Hidden;
			tipBox.Text = "Waiting ... ;";
			initiateButton.IsEnabled = false;
			myLog.appendLog("The task is running ... ");
			downloadNowNew(addressBox.Text);
		}

		private async void downloadNowNew(string downloadUrl)
		{
			try
			{
				var results = Cli.Wrap("./yt-dlp/yt-dlp.exe")
					.WithArguments(downloadUrl)
					.WithEnvironmentVariables(new Dictionary<string, string?>
					{
						["Path"] = "./ffmpeg/bin"
					});

				await foreach (var result in results.ListenAsync())
				{
					switch (result)
					{
						case StartedCommandEvent started:
							{
								myLog.appendLog("Process started. PID " + started.ProcessId);
								tipBox.Text = "Starting ... ";
								break;
							}
						case StandardOutputCommandEvent stdOut:
							{
								cacProgressBar(stdOut.Text);
								break;
							}
						case StandardErrorCommandEvent stdErr:
							{
								myLog.appendLog(stdErr.Text);
								break;
							}
						case ExitedCommandEvent exited:
							{
								myLog.appendLog("Process exited. Code: " + exited.ExitCode);
								//dlProgressBar.Value = 100;

								break;
							}
					}
				}

			}
			catch (Exception ex)
			{
				myLog.appendLog(ex.Message);
			}
			finally
			{
				//myLog.appendLog($"{stdOutBuffer.ToString()}");
				//myLog.appendLog($"{stdErrBuffer.ToString()}");
				tipBox.Text = "";

				initiateButton.IsEnabled = true;
				downloadButton.Visibility = Visibility.Visible;

			}
			//myLog.appendLog("Download task finished.");
		}
		private void cacProgressBar(string text)
		{
			string stdOutText = text;
			if (String.IsNullOrEmpty(text)) return;
			if (text.Contains("Downloading subtitles"))
			{
				dlProgressBar.Value = 20;
				tipBox.Text = "Downloading subtitles ... 1/3";

			}
			if (text.Contains("Writing video subtitles")) { dlProgressBar.Value = 60; }
			if (text.Contains("Converting subtitles"))
			{
				dlProgressBar.Value = 80;
				tipBox.Text = "Downloading medias ... 2/3";

			}
			if (text.Contains("ETA"))
			{
				Match match = Regex.Match(text, @"\d+\.\d+%");
				if (match.Success) dlProgressBar.Value = Double.Parse(match.Value.Remove(match.Value.Length - 1));
				stdOutText = String.Empty;
			}
			if (text.Contains("Merging formats into"))
			{
				dlProgressBar.Value = 0;
				tipBox.Text = " Merging formats ... 3/3";

			}
			if (text.Contains("Deleting original file")) { dlProgressBar.Value = 100; }

			if (!String.IsNullOrEmpty(stdOutText)) myLog.appendLog("OUT> " + stdOutText);
		}

		private string downloadNow(string downloadUrl)
		{
			Process downloader = new Process();
			try
			{
				downloader.StartInfo.UseShellExecute = false;
				downloader.StartInfo.FileName = "./yt-dlp/yt-dlp.exe";
				downloader.StartInfo.CreateNoWindow = !isTestFlag;
				downloader.StartInfo.Arguments = downloadUrl;
				downloader.StartInfo.RedirectStandardOutput = true;
				downloader.StartInfo.EnvironmentVariables["Path"] = "./ffmpeg/bin";
				downloader.Start();
				StreamReader reader = downloader.StandardOutput;
				return reader.ReadToEnd();
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}

		private void openFolder(object sender, RoutedEventArgs e)
		{
			myLog.appendLog("Go and check your videos at " + path + ".");
			Process.Start("explorer.exe", path);
		}

		private void initiate(object sender, RoutedEventArgs e)
		{
			try
			{
				// Determine whether the directory exists.
				if (!Directory.Exists(path))
				{
					// Create the directory it does not exist.
					Directory.CreateDirectory(path);
					myLog.appendLog("The directory has been set up");
				}
			}
			catch (Exception ex)
			{
				myLog.appendLog("The process failed: {0}" + ex.ToString());
			}
			finally
			{
				myLog.appendLog("Initiation done.");
			}


		}

		private void isTest(object sender, RoutedEventArgs e)
		{
			isTestFlag = isTestFlag ? false : true;
			addressBox.Text = isTestFlag ? "https://www.youtube.com/watch?v=nZNPgC0eyHk" : "";
		}

		private void testButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			myLog.appendAll();
		}

		private void dlProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}
	}
}
