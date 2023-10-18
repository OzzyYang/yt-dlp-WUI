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

namespace yt_dlp_UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		string path = "D:\\Videos\\Youtube";
		public MainWindow()
		{
			InitializeComponent();
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

		}


		private void download(object sender, RoutedEventArgs e)
		{
			messageBox.Text = "The task is running ... ";
			messageBox.Text = downloadNow(addressBox.Text);
			messageBox.ScrollToEnd();
		}


		private string downloadNow(string downloadUrl)
		{
			Process downloader = new Process();
			try
			{
				downloader.StartInfo.UseShellExecute = false;
				downloader.StartInfo.FileName = "./yt-dlp/yt-dlp.exe";
				downloader.StartInfo.CreateNoWindow = true;
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
			messageBox.Text = "Go and check your videos.";
			Process.Start("explorer.exe", path);
		}

		private void initiate(object sender, RoutedEventArgs e)
		{
			messageBox.Text = "Initiation done.";

			try
			{
				// Determine whether the directory exists.
				if (!Directory.Exists(path))
				{
					// Create the directory it does not exist.
					Directory.CreateDirectory(path);
				}
			}
			catch (Exception ex)
			{
				messageBox.Text = "The process failed: {0}" + ex.ToString();
			}


		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{

		}

		private void isTest(object sender, RoutedEventArgs e)
		{
			addressBox.Text = "https://www.youtube.com/watch?v=nZNPgC0eyHk";
		}
	}
}
