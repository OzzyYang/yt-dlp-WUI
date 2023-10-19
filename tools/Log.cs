using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;

namespace yt_dlp_WUI.tools
{
	public class MyLog
	{
		StringBuilder allLogs = new StringBuilder();
		TextBox output;

		public MyLog(TextBox output)
		{
			this.output = output;
			appendLog("The status will show here.");
		}

		public String appendLog(String content)
		{
			cleanLogs();
			allLogs.Append(DateTime.Now + ": " + content + "\n");
			output.Text = allLogs.ToString();
			output.ScrollToEnd();
			return allLogs.ToString();
		}

		public String getCurrentLogs()
		{
			return allLogs.ToString();
		}

		private void cleanLogs()
		{
			if (allLogs.Length > 5000)
			{
				appendAll();
			}
		}

		public void appendAll()
		{
			using (StreamWriter sw = new StreamWriter("Logs.txt", true))
			{
				sw.Write(allLogs.ToString());
			}
			allLogs.Clear();
		}

	}
}
