namespace General
{
	using UnityEngine;
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Collections.Generic;
	using System.Text;
#if !UNITY_WEBPLAYER	
	using System.IO;
#endif	

	public class Logger
	{
		// Log file settings
		public static long maxLogSize = -1;	// -1 = infinite log size, 1000000 = 1 MB
		public static string logPath1 = ".";
		public static string screenshotSubfolder1 = "screens1";
		public static string logFile1 = "log1.html";
		public static string logPath2 = "";
		public static string screenshotSubfolder2 = "";
		public static string logFile2 = "";
		public static string logPath3 = "";
		public static string screenshotSubfolder3 = "";
		public static string logFile3 = "";
		// Log message switches
		public static bool logToHTML = true;
		public static bool logToConsole = true;
		public static bool logMessages = true;
		public static bool logWarnings = true;
		public static bool logErrors = true;
		public static bool logExceptions = true;
		public static bool logDetails = true;
		public static bool logStackTraces = true;
		public static bool logScreenShots = true;
		public static bool logCharts = true;

		private static int currentLog = 1;
		private static string currentLogPath = logPath1;
		private static string currentScreenshotSubfolder = screenshotSubfolder1;
		private static string currentLogFile = logFile1;
#if !UNITY_WEBPLAYER
		private static StreamWriter OutputStream;
		private static HashSet<string> categoriesSet = new HashSet<string>();
		private static HashSet<string> subcategoriesSet = new HashSet<string>();
#endif
		
		void OnDestroy()
		{
			Logger.Close();
		} // OnDestroy
		
		public static void Close()
		{
#if !UNITY_WEBPLAYER			
			if (null != OutputStream)
			{
				OutputStream.WriteLine("\t</table>");
				OutputStream.WriteLine("</div>");
				OutputStream.WriteLine("</body>");
				OutputStream.WriteLine("</html>");
				OutputStream.Close();
				OutputStream = null;
			} // if
#endif
		} // Close
		
		public static void Log(string inCategory, string inSubcategory, string inMessage)
		{
			if (!logMessages)
			{
				return;
			} // if

			if (logToHTML)
			{
				AddMessageToLog("LOG", inCategory, inSubcategory, inMessage);
			} // if
			if (logToConsole)
			{
				UnityEngine.Debug.Log(inCategory + "::" + inSubcategory + ": " + inMessage);
			} // if
		} // Log
		
		public static void LogWarning(string inCategory, string inSubcategory, string inMessage)
		{
			if (!logWarnings)
			{
				return;
			} // if
			
			if (logToHTML)
			{
				AddMessageToLog("WARN", inCategory, inSubcategory, inMessage);
			} // if
			if (logToConsole)
			{
				UnityEngine.Debug.LogWarning(inCategory + "::" + inSubcategory + ": " + inMessage);
			} // if
		} // LogWarning
		
		public static void LogError(string inCategory, string inSubcategory, string inMessage)
		{
			if (!logErrors)
			{
				return;
			} // if
			
			if (logToHTML)
			{
				AddMessageToLog("ERROR", inCategory, inSubcategory, inMessage);
			} // if
			if (logToConsole)
			{
				UnityEngine.Debug.LogError(inCategory + "::" + inSubcategory + ": " + inMessage);
			} // if
		} // LogError
		
		public static void LogException(string inCategory, string inSubcategory, Exception inException)
		{
			if (!logExceptions)
			{
				return;
			} // if
			
			if (logToHTML)
			{
				AddMessageToLog("EXCEPTION", inCategory, inSubcategory, inException.ToString());
			} // if
			if (logToConsole)
			{
				UnityEngine.Debug.LogException(inException);
			} // if
		} // LogException
		
		public static void LogDetail(string inCategory, string inSubcategory, string inMessage)
		{
			if (!logDetails)
			{
				return;
			} // if
			
			if (logToHTML)
			{
				AddMessageToLog("DETAIL", inCategory, inSubcategory, inMessage);
			} // if
			if (logToConsole)
			{
				UnityEngine.Debug.Log(inCategory + "::" + inSubcategory + ": " + inMessage);
			} // if
		} // LogDetail

		public static void LogStackTrace(string inCategory, string inSubcategory)
		{
			if (!logStackTraces)
			{
				return;
			} // if
			
			string stackTrace = "";
			System.Diagnostics.StackFrame[] frames = new System.Diagnostics.StackTrace().GetFrames();
			
			for (int i = 1; i < frames.Length; i++) 
			{
				System.Diagnostics.StackFrame currFrame = frames[i];
				System.Reflection.MethodBase method = currFrame.GetMethod();
				string typeName = (null != method.ReflectedType)? method.ReflectedType.Name : string.Empty;
				
				stackTrace += typeName;
				stackTrace += ":";
				stackTrace += method.Name;
				stackTrace += "\t";
			} // for
			
			if (logToHTML)
			{
				AddMessageToLog("STACK", inCategory, inSubcategory, stackTrace);
			} // if
			if (logToConsole)
			{
				UnityEngine.Debug.Log(inCategory + "::" + inSubcategory + ": " + stackTrace);
			} // if
		} // LogStackTrace
		
		public static void LogScreenShot(string inCategory, string inSubcategory)
		{
			if (!logScreenShots)
			{
				return;
			} // if
			
			if (!logToHTML)
			{
				return;
			} // if
			
			string screenshotFilename;
			int screenshotCount = 0;
			DirectoryInfo oDirectoryInfo = new DirectoryInfo(currentLogPath + "/" + currentScreenshotSubfolder);

			if (!oDirectoryInfo.Exists)
			{
				oDirectoryInfo.Create();
			} // if

			do
			{
				screenshotCount++;
				screenshotFilename = currentLogPath + "/" + currentScreenshotSubfolder + "/screenshot" + screenshotCount + ".png";
			} while (System.IO.File.Exists(screenshotFilename));
			
			ScreenCapture.CaptureScreenshot(screenshotFilename);
			AddScreenShotToLog("SCREEN", inCategory, inSubcategory, currentScreenshotSubfolder + "/screenshot" + screenshotCount + ".png");
		} // LogScreenShot
		
		public static void LogChart(string inCategory, string inSubcategory, Chart inChart)
		{
			if (!logCharts)
			{
				return;
			} // if
			
			if (!logToHTML)
			{
				return;
			} // if
			
			string chartData = inChart.ToJSON();
			
			AddChartToLog("CHART", inCategory, inSubcategory, chartData);
		} // LogChart
		
		protected static void AddMessageToLog(string inMessageType, string inCategory, string inSubcategory, string inMessage)
		{
#if !UNITY_WEBPLAYER
			SetupLog();

			DateTime now = DateTime.Now;
			string timestamp = now.ToString("MM/dd/yyyy hh:mm:ss.fff");
			string encodedCategory = System.Security.SecurityElement.Escape(inCategory);
			string encodedSubcategory = System.Security.SecurityElement.Escape(inSubcategory);
			string encodedMessage = System.Security.SecurityElement.Escape(inMessage);
			
			if (null != OutputStream)
			{
				OutputStream.WriteLine("<script>AddMessage('" + inMessageType + "', '" + timestamp + "', '" + encodedCategory + "', '" + encodedSubcategory + "', '" + encodedMessage + "');</script>");
				OutputStream.Flush();
				
				int categoriesCount = categoriesSet.Count;
				int subcategoriesCount = subcategoriesSet.Count;
				
				categoriesSet.Add(inCategory);
				subcategoriesSet.Add(inSubcategory);
				
				if (categoriesSet.Count != categoriesCount)
				{
					OutputStream.WriteLine("<script>AddCategory('" + inCategory + "');</script>");
				} // if
				if (subcategoriesSet.Count != subcategoriesCount)
				{
					OutputStream.WriteLine("<script>AddSubcategory('" + inSubcategory + "');</script>");
				} // if
			} // if
#endif
		} // AddMessageToLog
		
		protected static void AddScreenShotToLog(string inMessageType, string inCategory, string inSubcategory, string inMessage)
		{
#if !UNITY_WEBPLAYER
			SetupLog();

			DateTime now = DateTime.Now;
			string timestamp = now.ToString("MM/dd/yyyy hh:mm:ss.fff");
			string encodedCategory = System.Security.SecurityElement.Escape(inCategory);
			string encodedSubcategory = System.Security.SecurityElement.Escape(inSubcategory);
			string encodedMessage = System.Security.SecurityElement.Escape(inMessage);
			string imagethumb = "<a href=\"" + encodedMessage + "\"><img src=\"" + encodedMessage + "\" width=\"50px\"; height=\"50px\";>Click to view full size image.</a>";
			
			if (null != OutputStream)
			{
				OutputStream.WriteLine("<script>AddScreenShot('" + inMessageType + "', '" + timestamp + "', '" + encodedCategory + "', '" + encodedSubcategory + "', '" + imagethumb + "');</script>");
				OutputStream.Flush();
				
				int categoriesCount = categoriesSet.Count;
				int subcategoriesCount = subcategoriesSet.Count;
				
				categoriesSet.Add(inCategory);
				subcategoriesSet.Add(inSubcategory);
				
				if (categoriesSet.Count != categoriesCount)
				{
					OutputStream.WriteLine("<script>AddCategory('" + inCategory + "');</script>");
				} // if
				if (subcategoriesSet.Count != subcategoriesCount)
				{
					OutputStream.WriteLine("<script>AddSubcategory('" + inSubcategory + "');</script>");
				} // if
			} // if
#endif
		} // AddScreenShotToLog
		
		protected static void AddChartToLog(string inMessageType, string inCategory, string inSubcategory, string inMessage)
		{
#if !UNITY_WEBPLAYER
			SetupLog();

			DateTime now = DateTime.Now;
			string timestamp = now.ToString("MM/dd/yyyy hh:mm:ss.fff");
			string encodedCategory = System.Security.SecurityElement.Escape(inCategory);
			string encodedSubcategory = System.Security.SecurityElement.Escape(inSubcategory);
			
			if (null != OutputStream)
			{
				OutputStream.WriteLine("<script>AddChart('" + inMessageType + "', '" + timestamp + "', '" + encodedCategory + "', '" + encodedSubcategory + "', '" + inMessage + "');</script>");
				OutputStream.Flush();
				
				int categoriesCount = categoriesSet.Count;
				int subcategoriesCount = subcategoriesSet.Count;
				
				categoriesSet.Add(inCategory);
				subcategoriesSet.Add(inSubcategory);
				
				if (categoriesSet.Count != categoriesCount)
				{
					OutputStream.WriteLine("<script>AddCategory('" + inCategory + "');</script>");
				} // if
				if (subcategoriesSet.Count != subcategoriesCount)
				{
					OutputStream.WriteLine("<script>AddSubcategory('" + inSubcategory + "');</script>");
				} // if
			} // if
#endif
		} // AddChartToLog

		protected static void SetupLog()
		{
			if (-1 != maxLogSize)
			{
				try
				{
					FileInfo oFileInfo = new FileInfo(GetLog());

					if (oFileInfo.Length > maxLogSize)
					{
						currentLog++;
						if (3 < currentLog)
						{
							currentLog = 1;
						} // if
						SetLog(true);
						OutputStream = null;
					} // if
				} // try
				catch
				{
				} // catch
			} // if

			if (null == OutputStream)
			{
				StartOutput();
			} // if
		} // SetupLog

		protected static void SetLog(bool inDeleteNewLog)
		{
			if (3 == currentLog)
			{
				if ((string.IsNullOrEmpty(logPath3))
			    || (string.IsNullOrEmpty(logFile3))
			    || (string.IsNullOrEmpty(screenshotSubfolder3)))
				{
					currentLog = 1;
					SetLog(inDeleteNewLog);
					return;
				} // if
				currentLogPath = logPath3;
				currentScreenshotSubfolder = screenshotSubfolder3;
				currentLogFile = logFile3;
			} // if
			else if (2 == currentLog)
			{
				if ((string.IsNullOrEmpty(logPath2))
			    || (string.IsNullOrEmpty(logFile2))
			    || (string.IsNullOrEmpty(screenshotSubfolder2)))
				{
					currentLog = 3;
					SetLog(inDeleteNewLog);
					return;
				} // if
				currentLogPath = logPath2;
				currentScreenshotSubfolder = screenshotSubfolder2;
				currentLogFile = logFile2;
			} // else if
			else
			{
				currentLogPath = logPath1;
				currentScreenshotSubfolder = screenshotSubfolder1;
				currentLogFile = logFile1;
			} // else if

			if (true == inDeleteNewLog)
			{
				try
				{
					FileInfo oFileInfo = new FileInfo(GetLog());
					DirectoryInfo oDirectoryInfo = new DirectoryInfo(currentLogPath + "/" + currentScreenshotSubfolder);
					FileInfo[] aScreenshots = oDirectoryInfo.GetFiles();

					if (oFileInfo.Exists)
					{
						oFileInfo.Delete();
					} // if
					for (int loop1 = 0; loop1 < aScreenshots.Length; loop1++)
					{
						FileInfo oScreenshot = aScreenshots[loop1];

						oScreenshot.Delete();
					} // for
				} // try
				catch
				{
				} // catch
			} // if
		} // SetLog

		protected static string GetLog()
		{
			UnityEngine.Debug.Log(currentLogPath + "/" + currentLogFile);
			return currentLogPath + "/" + currentLogFile;
		} // GetLog

		protected static void StartOutput()
		{
#if !UNITY_WEBPLAYER
			OutputStream = new StreamWriter(GetLog(), false);
			OutputStream.Close();
			OutputStream = new StreamWriter(GetLog(), true);
			OutputStream.WriteLine("<!DOCTYPE html>");
			OutputStream.WriteLine("<html>");
			OutputStream.WriteLine("<head>");
			OutputStream.WriteLine("<style>table, td {border: 1px solid black;}</style>");
			OutputStream.WriteLine("<style>#LogHeader.stick {position: fixed;}</style>");
			OutputStream.WriteLine("<script src='http://code.jquery.com/jquery-2.1.4.min.js'></script>");
			OutputStream.WriteLine("<script>");
			OutputStream.WriteLine("\tvar colorLookupTable = [];");
			OutputStream.WriteLine("\tvar categoryTable = [];");
			OutputStream.WriteLine("\tvar subcategoryTable = [];");
			OutputStream.WriteLine("\tvar messageTypeVisibleTable = [];");
			OutputStream.WriteLine("\tvar categoryVisibleTable = [];");
			OutputStream.WriteLine("\tvar subcategoryVisibleTable = [];\n");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(0,255,255,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(130,0,130,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(138,43,226,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(165,42,42,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(222,184,135,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(95,158,160,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(127,255,0,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(210,105,30,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(255,127,80,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(150,150,200,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(220,20,60,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(50,50,139,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(0,139,139,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(184,134,11,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(169,169,169,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(10,100,0,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(189,183,107,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(205,92,92,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(85,107,47,1)');");
			OutputStream.WriteLine("\tcolorLookupTable.push('rgba(255,140,0,1)');");
			OutputStream.WriteLine("\tmessageTypeVisibleTable.push('LOG');");
			OutputStream.WriteLine("\tmessageTypeVisibleTable.push('WARN');");
			OutputStream.WriteLine("\tmessageTypeVisibleTable.push('ERROR');");
			OutputStream.WriteLine("\tmessageTypeVisibleTable.push('DETAIL');");
			OutputStream.WriteLine("\tmessageTypeVisibleTable.push('STACK');");
			OutputStream.WriteLine("\tmessageTypeVisibleTable.push('SCREEN');");
			OutputStream.WriteLine("\tmessageTypeVisibleTable.push('CHART');");
			OutputStream.WriteLine("\tmessageTypeVisibleTable.push('EXCEPTION');");
			OutputStream.WriteLine("");
			OutputStream.WriteLine("\tfunction AddMessage(inMessageType, inTimestamp, inCategory, inSubcategory, inMessage)");
			OutputStream.WriteLine("\t{");
			OutputStream.WriteLine("\t\tvar table = document.getElementById('MessagesTable');");
			OutputStream.WriteLine("\t\tvar row = table.insertRow(1);");
			OutputStream.WriteLine("\t\tvar cell1 = row.insertCell(0);");
			OutputStream.WriteLine("\t\tvar cell2 = row.insertCell(1);");
			OutputStream.WriteLine("\t\tvar cell3 = row.insertCell(2);");
			OutputStream.WriteLine("\t\tvar cell4 = row.insertCell(3);\n");
			OutputStream.WriteLine("\t\trow.className = inMessageType;");
			OutputStream.WriteLine("\t\trow.className += ' ' + inCategory;");
			OutputStream.WriteLine("\t\trow.className += ' ' + inSubcategory;");
			OutputStream.WriteLine("\t\tcell1.innerHTML = inTimestamp;");
			OutputStream.WriteLine("\t\tcell2.innerHTML = inCategory;");
			OutputStream.WriteLine("\t\tcell3.innerHTML = inSubcategory;");
			OutputStream.WriteLine("\t\tcell4.innerHTML = inMessage;");
			OutputStream.WriteLine("\t\tcell1.style.backgroundColor = LookupMessageTypeColor(inMessageType);");
			OutputStream.WriteLine("\t\tcell2.style.backgroundColor = LookupCategoryColor(inCategory);");
			OutputStream.WriteLine("\t\tcell3.style.backgroundColor = LookupSubcategoryColor(inSubcategory);");
			OutputStream.WriteLine("\t} // AddMessage");
			OutputStream.WriteLine("");
			OutputStream.WriteLine("\tfunction AddScreenShot(inMessageType, inTimestamp, inCategory, inSubcategory, inMessage)");
			OutputStream.WriteLine("\t{");
			OutputStream.WriteLine("\t\tvar table = document.getElementById('MessagesTable');");
			OutputStream.WriteLine("\t\tvar row = table.insertRow(1);");
			OutputStream.WriteLine("\t\tvar cell1 = row.insertCell(0);");
			OutputStream.WriteLine("\t\tvar cell2 = row.insertCell(1);");
			OutputStream.WriteLine("\t\tvar cell3 = row.insertCell(2);");
			OutputStream.WriteLine("\t\tvar cell4 = row.insertCell(3);\n");
			OutputStream.WriteLine("\t\trow.className = inMessageType;");
			OutputStream.WriteLine("\t\trow.className += ' ' + inCategory;");
			OutputStream.WriteLine("\t\trow.className += ' ' + inSubcategory;");
			OutputStream.WriteLine("\t\tcell1.innerHTML = inTimestamp;");
			OutputStream.WriteLine("\t\tcell2.innerHTML = inCategory;");
			OutputStream.WriteLine("\t\tcell3.innerHTML = inSubcategory;");
			OutputStream.WriteLine("\t\tcell4.innerHTML = inMessage;");
			OutputStream.WriteLine("\t\tcell1.style.backgroundColor = LookupMessageTypeColor(inMessageType);");
			OutputStream.WriteLine("\t\tcell2.style.backgroundColor = LookupCategoryColor(inCategory);");
			OutputStream.WriteLine("\t\tcell3.style.backgroundColor = LookupSubcategoryColor(inSubcategory);");
			OutputStream.WriteLine("\t} // AddScreenShot");
			OutputStream.WriteLine("");
			OutputStream.WriteLine("\tfunction AddChart(inMessageType, inTimestamp, inCategory, inSubcategory, inMessage)");
			OutputStream.WriteLine("\t{");
			OutputStream.WriteLine("\t\tvar table = document.getElementById('MessagesTable');");
			OutputStream.WriteLine("\t\tvar row = table.insertRow(1);");
			OutputStream.WriteLine("\t\tvar cell1 = row.insertCell(0);");
			OutputStream.WriteLine("\t\tvar cell2 = row.insertCell(1);");
			OutputStream.WriteLine("\t\tvar cell3 = row.insertCell(2);");
			OutputStream.WriteLine("\t\tvar cell4 = row.insertCell(3);");
			OutputStream.WriteLine("\t\tvar canvas = document.createElement('canvas');");
			OutputStream.WriteLine("\t\tvar canvasContext = canvas.getContext('2d');");
			OutputStream.WriteLine("\t\tvar canvasData = JSON.parse(inMessage);");
			OutputStream.WriteLine("\t\tvar xTickWidth = (canvasData.width - 12) / (canvasData.xAxisTickCount + 1);");
			OutputStream.WriteLine("\t\tvar xCurrentTick = 12 + xTickWidth;");
			OutputStream.WriteLine("\t\tvar yTickHeight = (canvasData.height - 24) / (canvasData.yAxisTickCount + 1);");
			OutputStream.WriteLine("\t\tvar yCurrentTick = 12 + yTickHeight;\n");
			OutputStream.WriteLine("\t\tcanvas.width = canvasData.width;");
			OutputStream.WriteLine("\t\tcanvas.height = canvasData.height;");
			OutputStream.WriteLine("\t\tcanvasContext.fillStyle = 'AntiqueWhite';");
			OutputStream.WriteLine("\t\tcanvasContext.fillRect(0, 0, canvasData.width, canvasData.height);");
			OutputStream.WriteLine("\t\tcanvasContext.font = '10px Arial';");
			OutputStream.WriteLine("\t\tcanvasContext.fillStyle = '#000000';");
			OutputStream.WriteLine("\t\tcanvasContext.fillText(canvasData.chartName, 10, 10);");
			OutputStream.WriteLine("\t\tcanvasContext.fillText(canvasData.xAxisName, 10, canvasData.height);");
			OutputStream.WriteLine("\t\tcanvasContext.rotate((-90) * Math.PI / 180);");
			OutputStream.WriteLine("\t\tcanvasContext.fillText(canvasData.yAxisName, 10 - canvasData.height, 5);");
			OutputStream.WriteLine("\t\tcanvasContext.rotate(90 * Math.PI / 180);");
			OutputStream.WriteLine("\t\tcanvasContext.beginPath();");
			OutputStream.WriteLine("\t\tcanvasContext.strokeStyle = 'black';");
			OutputStream.WriteLine("\t\tcanvasContext.moveTo(12, canvasData.height - 12);");
			OutputStream.WriteLine("\t\tcanvasContext.lineTo(12, 12);");
			OutputStream.WriteLine("\t\tcanvasContext.stroke();");
			OutputStream.WriteLine("\t\tcanvasContext.moveTo(12, 12);");
			OutputStream.WriteLine("\t\tcanvasContext.lineTo(canvasData.width, 12);");
			OutputStream.WriteLine("\t\tcanvasContext.stroke();");
			OutputStream.WriteLine("\t\tcanvasContext.moveTo(12, canvasData.height - 12);");
			OutputStream.WriteLine("\t\tcanvasContext.lineTo(canvasData.width, canvasData.height - 12);");
			OutputStream.WriteLine("\t\tcanvasContext.stroke();");
			OutputStream.WriteLine("\t\tfor (var xTicks = 0; xTicks < canvasData.xAxisTickCount; xTicks++)\n\t\t{");
			OutputStream.WriteLine("\t\t\tcanvasContext.moveTo(xCurrentTick, canvasData.height - 12);");
			OutputStream.WriteLine("\t\t\tcanvasContext.lineTo(xCurrentTick, canvasData.height - 17);");
			OutputStream.WriteLine("\t\t\tcanvasContext.stroke();");
			OutputStream.WriteLine("\t\t\txCurrentTick += xTickWidth;");
			OutputStream.WriteLine("\t\t} // for");
			OutputStream.WriteLine("\t\tfor (var yTicks = 0; yTicks < canvasData.yAxisTickCount; yTicks++)\n\t\t{");
			OutputStream.WriteLine("\t\t\tcanvasContext.moveTo(12, yCurrentTick);");
			OutputStream.WriteLine("\t\t\tcanvasContext.lineTo(17, yCurrentTick);");
			OutputStream.WriteLine("\t\t\tcanvasContext.stroke();");
			OutputStream.WriteLine("\t\t\tyCurrentTick += yTickHeight;");
			OutputStream.WriteLine("\t\t} // for");
			OutputStream.WriteLine("\t\tfor (var dataElementIndex = 0; dataElementIndex < canvasData.dataElements.length; dataElementIndex++)\n\t\t{");
			OutputStream.WriteLine("\t\t\tvar dataElement = canvasData.dataElements[dataElementIndex];");
			OutputStream.WriteLine("\t\t\tvar dataCount = dataElement.data.length;");
			OutputStream.WriteLine("\t\t\tvar dataRange = canvasData.maxValue - canvasData.minValue;");
			OutputStream.WriteLine("\t\t\tvar prettyAdjustment1 = 40;");
			OutputStream.WriteLine("\t\t\tvar dataRatio = (canvasData.height - prettyAdjustment1) / (dataRange + 1);");
			OutputStream.WriteLine("\t\t\tvar dataXPosition = 12 + xTickWidth;");
			OutputStream.WriteLine("\t\t\tvar previousDataXPosition = 0;");
			OutputStream.WriteLine("\t\t\tvar previousDataYPosition = 0;\n");
			OutputStream.WriteLine("\t\t\tfor (var dataIndex = 0; dataIndex < dataElement.data.length; dataIndex++)\n\t\t\t{");
			OutputStream.WriteLine("\t\t\t\tvar dataValue = dataElement.data[dataIndex];");
			OutputStream.WriteLine("\t\t\t\tvar minValueNotZeroAdjustment = (canvasData.minValue * dataRatio);");
			OutputStream.WriteLine("\t\t\t\tvar prettyAdjustment2 = 20;");
			OutputStream.WriteLine("\t\t\t\tvar dataYPosition = ((canvasData.height - prettyAdjustment2) - (dataValue * dataRatio)) + minValueNotZeroAdjustment;\n");
			OutputStream.WriteLine("\t\t\t\tif ((0 == dataRange)");
			OutputStream.WriteLine("\t\t\t\t|| ((-0.001 < dataRange) && (0.001 > dataRange)))\n\t\t\t\t{");
			OutputStream.WriteLine("\t\t\t\t\tdataYPosition = canvasData.height / 2;");
			OutputStream.WriteLine("\t\t\t\t} // if");
			OutputStream.WriteLine("\t\t\t\tcanvasContext.fillStyle = dataElement.color;");
			OutputStream.WriteLine("\t\t\t\tcanvasContext.fillText('' + dataValue, dataXPosition, dataYPosition);");
			OutputStream.WriteLine("\t\t\t\tif (0 < dataIndex)\n\t\t\t\t{\n");
			OutputStream.WriteLine("\t\t\t\t\tcanvasContext.beginPath();");
			OutputStream.WriteLine("\t\t\t\t\tcanvasContext.strokeStyle = dataElement.color;");
			OutputStream.WriteLine("\t\t\t\t\tcanvasContext.moveTo(previousDataXPosition, previousDataYPosition);");
			OutputStream.WriteLine("\t\t\t\t\tcanvasContext.lineTo(dataXPosition, dataYPosition);");
			OutputStream.WriteLine("\t\t\t\t\tcanvasContext.stroke();");
			OutputStream.WriteLine("\t\t\t\t} // if");
			OutputStream.WriteLine("\t\t\t\tpreviousDataXPosition = dataXPosition;");
			OutputStream.WriteLine("\t\t\t\tpreviousDataYPosition = dataYPosition;");
			OutputStream.WriteLine("\t\t\t\tdataXPosition += xTickWidth;");
			OutputStream.WriteLine("\t\t\t} // for");
			OutputStream.WriteLine("\t\t} // for");
			OutputStream.WriteLine("\t\tcell4.appendChild(canvas);");
			OutputStream.WriteLine("\t\tif (canvasData.drawLegend)\n\t\t{");
			OutputStream.WriteLine("\t\t\tvar legend=document.createElement('table');");
			OutputStream.WriteLine("\t\t\tvar legendHeader = legend.createTHead();");
			OutputStream.WriteLine("\t\t\tvar legendHeaderRow = legendHeader.insertRow(0);");
			OutputStream.WriteLine("\t\t\tvar legendHeaderCell = legendHeaderRow.insertCell(0);\n");
			OutputStream.WriteLine("\t\t\tlegendHeaderCell.innerHTML = '<b>Legend</b>';");
			OutputStream.WriteLine("\t\t\tvar legendColorRow = legendHeader.insertRow(1);");
			OutputStream.WriteLine("\t\t\tvar legendTitleRow = legendHeader.insertRow(1);");
			OutputStream.WriteLine("\t\t\tfor (var dataElementIndex2 = 0; dataElementIndex2 < canvasData.dataElements.length; dataElementIndex2++)\n\t\t\t{");
			OutputStream.WriteLine("\t\t\t\tvar dataElement = canvasData.dataElements[dataElementIndex2];");
			OutputStream.WriteLine("\t\t\t\tvar legendColorRowCell = legendColorRow.insertCell(0);");
			OutputStream.WriteLine("\t\t\t\tvar legendTitleRowCell = legendTitleRow.insertCell(0);\n");
			OutputStream.WriteLine("\t\t\t\tlegendColorRowCell.style.backgroundColor = dataElement.color;");
			OutputStream.WriteLine("\t\t\t\tlegendColorRowCell.style.height = '20px';");
			OutputStream.WriteLine("\t\t\t\tlegendTitleRowCell.innerHTML = dataElement.name;");
			OutputStream.WriteLine("\t\t\t} // for");
			OutputStream.WriteLine("\t\t\tcell4.appendChild(legend);");
			OutputStream.WriteLine("\t\t} // if");
			OutputStream.WriteLine("\t\trow.className = inMessageType;");
			OutputStream.WriteLine("\t\trow.className += ' ' + inCategory;");
			OutputStream.WriteLine("\t\trow.className += ' ' + inSubcategory;");
			OutputStream.WriteLine("\t\tcell1.innerHTML = inTimestamp;");
			OutputStream.WriteLine("\t\tcell2.innerHTML = inCategory;");
			OutputStream.WriteLine("\t\tcell3.innerHTML = inSubcategory;");
			OutputStream.WriteLine("\t\tcell1.style.backgroundColor = LookupMessageTypeColor(inMessageType);");
			OutputStream.WriteLine("\t\tcell2.style.backgroundColor = LookupCategoryColor(inCategory);");
			OutputStream.WriteLine("\t\tcell3.style.backgroundColor = LookupSubcategoryColor(inSubcategory);");
			OutputStream.WriteLine("\t} // AddChart");
			OutputStream.WriteLine("");
			OutputStream.WriteLine("\tfunction AddCategory(inCategory)");
			OutputStream.WriteLine("\t{");
			OutputStream.WriteLine("\t\tvar table = document.getElementById('CategoriesTable');");
			OutputStream.WriteLine("\t\tvar row = null;\n");
			OutputStream.WriteLine("\t\tif (1 == table.rows.length)\n\t\t{\n\t\t\trow = table.insertRow(1);\n\t\t} // if");
			OutputStream.WriteLine("\t\telse if (8 <= table.rows[1].cells.length)\n\t\t{\n\t\t\trow = table.insertRow(1);\n\t\t} // else if");
			OutputStream.WriteLine("\t\telse\n\t\t{\n\t\t\trow = table.rows[1];\n\t\t} // else\n");
			OutputStream.WriteLine("\t\tvar cell1 = row.insertCell(0);");
			OutputStream.WriteLine("\t\tcell1.innerHTML = inCategory;");
			OutputStream.WriteLine("\t\tcell1.style.backgroundColor = LookupCategoryColor(inCategory);");
			OutputStream.WriteLine("\t\tcell1.onclick = function() {ToggleCategory(inCategory);}");
			OutputStream.WriteLine("\t\tcategoryVisibleTable.push(inCategory);");
			OutputStream.WriteLine("\t} // AddCategory");
			OutputStream.WriteLine("");
			OutputStream.WriteLine("\tfunction AddSubcategory(inSubcategory)");
			OutputStream.WriteLine("\t{");
			OutputStream.WriteLine("\t\tvar table = document.getElementById('SubcategoriesTable');");
			OutputStream.WriteLine("\t\tvar row = null;\n");
			OutputStream.WriteLine("\t\tif (1 == table.rows.length)\n\t\t{\n\t\t\trow = table.insertRow(1);\n\t\t} // if");
			OutputStream.WriteLine("\t\telse if (8 <= table.rows[1].cells.length)\n\t\t{\n\t\t\trow = table.insertRow(1);\n\t\t} // else if");
			OutputStream.WriteLine("\t\telse\n\t\t{\n\t\t\trow = table.rows[1];\n\t\t} // else\n");
			OutputStream.WriteLine("\t\tvar cell1 = row.insertCell(0);");
			OutputStream.WriteLine("\t\tcell1.innerHTML = inSubcategory;");
			OutputStream.WriteLine("\t\tcell1.style.backgroundColor = LookupSubcategoryColor(inSubcategory);");
			OutputStream.WriteLine("\t\tcell1.onclick = function() {ToggleSubcategory(inSubcategory);}");
			OutputStream.WriteLine("\t\tsubcategoryVisibleTable.push(inSubcategory);");
			OutputStream.WriteLine("\t} // AddSubcategory");
			OutputStream.WriteLine("");
			OutputStream.WriteLine("\tfunction LookupMessageTypeColor(inMessageType)");
			OutputStream.WriteLine("\t{");
			OutputStream.WriteLine("\t\tif ('LOG' == inMessageType)\n\t\t{\n\t\t\treturn 'rgba(64,64,255,1)';\n\t\t} // if");
			OutputStream.WriteLine("\t\telse if ('WARN' == inMessageType)\n\t\t{\n\t\t\treturn 'rgba(255,255,0,1)';\n\t\t} // else if");
			OutputStream.WriteLine("\t\telse if ('ERROR' == inMessageType)\n\t\t{\n\t\t\treturn 'rgba(255,0,0,1)';\n\t\t} // else if");
			OutputStream.WriteLine("\t\telse if ('STACK' == inMessageType)\n\t\t{\n\t\t\treturn 'rgba(0,255,0,1)';\n\t\t} // else if");
			OutputStream.WriteLine("\t\telse if ('SCREEN' == inMessageType)\n\t\t{\n\t\t\treturn 'rgba(0,255,255,1)';\n\t\t} // else if");
			OutputStream.WriteLine("\t\telse if ('CHART' == inMessageType)\n\t\t{\n\t\t\treturn 'rgba(255,0,255,1)';\n\t\t} // else if");
			OutputStream.WriteLine("\t\telse if ('EXCEPTION' == inMessageType)\n\t\t{\n\t\t\treturn 'rgba(255,165,0,1)';\n\t\t} // else if");
			OutputStream.WriteLine("\t\treturn 'rgba(255,255,255,1)';");
			OutputStream.WriteLine("\t} // LookupMessageTypeColor");
			OutputStream.WriteLine("");
			OutputStream.WriteLine("\tfunction ToggleType(inClassName)");
			OutputStream.WriteLine("\t{");
			OutputStream.WriteLine("\t\tvar classIndex = messageTypeVisibleTable.indexOf(inClassName);\n");
			OutputStream.WriteLine("\t\tif (-1 == classIndex)\n\t\t{");
			OutputStream.WriteLine("\t\t\tmessageTypeVisibleTable.push(inClassName);");
			OutputStream.WriteLine("\t\t} // if");
			OutputStream.WriteLine("\t\telse\n\t\t{");
			OutputStream.WriteLine("\t\t\tmessageTypeVisibleTable.splice(classIndex, 1);");
			OutputStream.WriteLine("\t\t} // else");
			OutputStream.WriteLine("\t\tToggleGroup(inClassName);");
			OutputStream.WriteLine("\t} // ToggleType\n");
			OutputStream.WriteLine("\tfunction ToggleCategory(inClassName)");
			OutputStream.WriteLine("\t{");
			OutputStream.WriteLine("\t\tvar classIndex = categoryVisibleTable.indexOf(inClassName);\n");
			OutputStream.WriteLine("\t\tif (-1 == classIndex)\n\t\t{");
			OutputStream.WriteLine("\t\t\tcategoryVisibleTable.push(inClassName);");
			OutputStream.WriteLine("\t\t} // if");
			OutputStream.WriteLine("\t\telse\n\t\t{");
			OutputStream.WriteLine("\t\t\tcategoryVisibleTable.splice(classIndex, 1);");
			OutputStream.WriteLine("\t\t} // else");
			OutputStream.WriteLine("\t\tToggleGroup(inClassName);");
			OutputStream.WriteLine("\t} // ToggleCategory\n");
			OutputStream.WriteLine("\tfunction ToggleSubcategory(inClassName)");
			OutputStream.WriteLine("\t{");
			OutputStream.WriteLine("\t\tvar classIndex = subcategoryVisibleTable.indexOf(inClassName);\n");
			OutputStream.WriteLine("\t\tif (-1 == classIndex)\n\t\t{");
			OutputStream.WriteLine("\t\t\tsubcategoryVisibleTable.push(inClassName);");
			OutputStream.WriteLine("\t\t} // if");
			OutputStream.WriteLine("\t\telse\n\t\t{");
			OutputStream.WriteLine("\t\t\tsubcategoryVisibleTable.splice(classIndex, 1);");
			OutputStream.WriteLine("\t\t} // else");
			OutputStream.WriteLine("\t\tToggleGroup(inClassName);");
			OutputStream.WriteLine("\t} // ToggleSubcategory\n");
			OutputStream.WriteLine("\tfunction ToggleGroup(inClassName)");
			OutputStream.WriteLine("\t{");
			OutputStream.WriteLine("\t\tvar elements = document.getElementsByClassName(inClassName);\n");
			OutputStream.WriteLine("\t\tfor (elementsLoop = 0; elementsLoop < elements.length; elementsLoop++)\n\t\t{");
			OutputStream.WriteLine("\t\t\tvar currentElement = elements[elementsLoop];");
			OutputStream.WriteLine("\t\t\tvar elementClasses = currentElement.className.split(' ');");
			OutputStream.WriteLine("\t\t\tvar elementTypeClass = elementClasses[0];");
			OutputStream.WriteLine("\t\t\tvar elementCategoryClass = elementClasses[1];");
			OutputStream.WriteLine("\t\t\tvar elementSubcategoryClass = elementClasses[2];");
			OutputStream.WriteLine("\t\t\tvar currentClassVisible = true;\n");
			OutputStream.WriteLine("\t\t\tif ((-1 == messageTypeVisibleTable.indexOf(elementTypeClass))");
			OutputStream.WriteLine("\t\t\t|| (-1 == categoryVisibleTable.indexOf(elementCategoryClass))");
			OutputStream.WriteLine("\t\t\t|| (-1 == subcategoryVisibleTable.indexOf(elementSubcategoryClass)))\n\t\t\t{");
			OutputStream.WriteLine("\t\t\t\tcurrentClassVisible = false;");
			OutputStream.WriteLine("\t\t\t} // if");
			OutputStream.WriteLine("\t\t\tif (false == currentClassVisible)");
			OutputStream.WriteLine("\t\t\t{");
			OutputStream.WriteLine("\t\t\t\tcurrentElement.style.display = 'none';");
			OutputStream.WriteLine("\t\t\t} // if");
			OutputStream.WriteLine("\t\t\telse");
			OutputStream.WriteLine("\t\t\t{");
			OutputStream.WriteLine("\t\t\t\tcurrentElement.style.display = 'table-row';");
			OutputStream.WriteLine("\t\t\t} // else");
			OutputStream.WriteLine("\t\t} // for");
			OutputStream.WriteLine("\t} // ToggleGroup");
			OutputStream.WriteLine("");
			OutputStream.WriteLine("\tfunction LookupCategoryColor(inCategory)");
			OutputStream.WriteLine("\t{");
			OutputStream.WriteLine("\t\tvar index = DoColorLookup(inCategory, categoryTable)");
			OutputStream.WriteLine("\t\treturn colorLookupTable[index];");
			OutputStream.WriteLine("\t} // LookupCategoryColor");
			OutputStream.WriteLine("");
			OutputStream.WriteLine("\tfunction LookupSubcategoryColor(inSubcategory)");
			OutputStream.WriteLine("\t{");
			OutputStream.WriteLine("\t\tvar index = DoColorLookup(inSubcategory, subcategoryTable)");
			OutputStream.WriteLine("\t\treturn colorLookupTable[(colorLookupTable.length - 1) - index];");
			OutputStream.WriteLine("\t} // LookupSubcategoryColor");
			OutputStream.WriteLine("");
			OutputStream.WriteLine("\tfunction DoColorLookup(inCategory, inLookupTable)");
			OutputStream.WriteLine("\t{");
			OutputStream.WriteLine("\t\tif (-1 == inLookupTable.indexOf(inCategory)) inLookupTable.push(inCategory);\n");
			OutputStream.WriteLine("\t\tvar index = inLookupTable.indexOf(inCategory);\n");
			OutputStream.WriteLine("\t\tif (index > colorLookupTable.length - 1) index = index % (colorLookupTable.length - 1);");
			OutputStream.WriteLine("\t\treturn index;");
			OutputStream.WriteLine("\t} // DoColorLookup");
			OutputStream.WriteLine("");
			OutputStream.WriteLine("\tfunction StickyRelocate()\n\t{");
			OutputStream.WriteLine("\t\tvar topOfWindow = $(window).scrollTop();");
			OutputStream.WriteLine("\t\tvar topOfDiv = $('#LogHeaderAnchor').offset().top;\n");
			OutputStream.WriteLine("\t\tif (topOfWindow > topOfDiv)\n\t\t{");
			OutputStream.WriteLine("\t\t\t$('#LogHeader').addClass('stick');\n\t\t} // if");
			OutputStream.WriteLine("\t\telse\n\t\t{");
			OutputStream.WriteLine("\t\t\t$('#LogHeader').removeClass('stick');\n\t\t} // else");
			OutputStream.WriteLine("\t} // StickyRelocate");
			OutputStream.WriteLine("");
			OutputStream.WriteLine("</script>");
			OutputStream.WriteLine("</head>");
			OutputStream.WriteLine("<body>");
			OutputStream.WriteLine("<div id='LogHeaderAnchor'/>");
			OutputStream.WriteLine("<div id='LogHeader' align='right' style='btop:0; right:20px;'>");
			OutputStream.WriteLine("\t<div id='MessageTypeSection'>");
			OutputStream.WriteLine("\t\t<table id='MessageTypeTable' style='background-color: white'>");
			OutputStream.WriteLine("\t\t\t<tr><th>Message Types</th></tr>");
			
			string tableRow = "\t\t\t<tr>\n";
			
			tableRow += "\t\t\t\t<td style='background-color: rgba(64,64,255,1)' onClick='ToggleType(" + "\"LOG\"" + ")'>Log</td>\n";
			tableRow += "\t\t\t\t<td style='background-color: rgba(255,255,0,1)' onClick='ToggleType(" + "\"WARN\"" + ")'>Warning</td>\n";
			tableRow += "\t\t\t\t<td style='background-color: rgba(255,0,0,1)' onClick='ToggleType(" + "\"ERROR\"" + ")'>Error</td>\n";
			tableRow += "\t\t\t\t<td style='background-color: rgba(255,165,0,1)' onClick='ToggleType(" + "\"EXCEPTION\"" + ")'>Exception</td>\n";
			tableRow += "\t\t\t\t<td style='background-color: rgba(255,255,255,1)' onClick='ToggleType(" + "\"DETAIL\"" + ")'>Detail</td>\n";
			tableRow += "\t\t\t\t<td style='background-color: rgba(0,255,0,1)' onClick='ToggleType(" + "\"STACK\"" + ")'>Stack</td>\n";
			tableRow += "\t\t\t\t<td style='background-color: rgba(0,255,255,1)' onClick='ToggleType(" + "\"SCREEN\"" + ")'>Screen</td>\n";
			tableRow += "\t\t\t\t<td style='background-color: rgba(255,0,255,1)' onClick='ToggleType(" + "\"CHART\"" + ")'>Chart</td>\n";
			tableRow += "\t\t\t</tr>\n";
			OutputStream.WriteLine(tableRow);
			OutputStream.WriteLine("\t\t</table>");
			OutputStream.WriteLine("\t</div>");
			OutputStream.WriteLine("\t<div id='CategoriesSection'>");
			OutputStream.WriteLine("\t\t<table id='CategoriesTable' style='background-color: white'>");
			OutputStream.WriteLine("\t\t\t<tr><th>Categories</th></tr>\n");
			OutputStream.WriteLine("\t\t</table>");
			OutputStream.WriteLine("\t</div>");
			OutputStream.WriteLine("\t<div id='SubcategoriesSection'>");
			OutputStream.WriteLine("\t\t<table id='SubcategoriesTable' style='background-color: white'>");
			OutputStream.WriteLine("\t\t\t<tr><th>Subcategories</th></tr>\n");
			OutputStream.WriteLine("\t\t</table>");
			OutputStream.WriteLine("\t</div>");
			OutputStream.WriteLine("</div>");
			OutputStream.WriteLine("<script>");
			OutputStream.WriteLine("\t$(function () {$(window).scroll(StickyRelocate);StickyRelocate();});");
			OutputStream.WriteLine("");
			OutputStream.WriteLine("</script>");
			OutputStream.WriteLine("<div id='MessagesSection'>");
			OutputStream.WriteLine("\t<table id='MessagesTable'>");
			OutputStream.WriteLine("\t\t<tr><th>Time</th><th>Category</th><th>Subcategory</th><th>Message</th></tr>");
#endif
		} // StartOutput
	} // class

	public class ChartData
	{
		public ChartData()
		{
			name = "Data";
			color = Color.black;
		} // ChartData
		public ChartData(string inName, int inDataSize, Color inColor)
		{
			name = inName;
			color = inColor;
			data = new float[inDataSize];
		} // ChartData
		public string name = "Data";
		public Color color = Color.black;
		public float[] data;
		public string ToJSON()
		{
			System.Text.StringBuilder json = new StringBuilder();
			
			json.Append("{");
			json.Append("\"name\" : \"" + name + "\", ");
			json.Append("\"color\" : \"rgba(" + Mathf.RoundToInt(color.r * 255) + "," + Mathf.RoundToInt(color.g * 255) + "," + Mathf.RoundToInt(color.b * 255) + "," + color.a + ")\", ");
			if (null != data)
			{
				float minValue = float.MaxValue;
				float maxValue = float.MinValue;
				
				json.Append("\"data\" : [");
				for (int loop = 0; loop < data.Length; loop++)
				{
					if (0 < loop)
					{
						json.Append(",");
					} // if
					json.Append(data[loop].ToString());
					if (minValue > data[loop])
					{
						minValue = data[loop];
					} // if
					if (maxValue < data[loop])
					{
						maxValue = data[loop];
					} // if
				} // for
				json.Append("],");
				json.Append("\"minValue\" : " + minValue + ", ");
				json.Append("\"maxValue\" : " + maxValue);
			} // if
			json.Append("}");
			
			return json.ToString();
		} // ToJSON
		public void ClearData()
		{
			data = new float[data.Length];
		} // ClearData
		public float GetMinValue()
		{
			float minValue = float.MaxValue;
			
			for (int loop = 0; loop < data.Length; loop++)
			{
				if (minValue > data[loop])
				{
					minValue = data[loop];
				} // if
			} // for
			return minValue;
		} // GetMinValue
		public float GetMaxValue()
		{
			float maxValue = float.MinValue;
			
			for (int loop = 0; loop < data.Length; loop++)
			{
				if (maxValue < data[loop])
				{
					maxValue = data[loop];
				} // if
			} // for
			return maxValue;
		} // GetMaxValue
	} // class 
	
	public class Chart
	{
		public Chart()
		{
			chartName = "Chart";
			xAxisName = "X Axis";
			yAxisName = "Y Axis";
			width = 450;
			height = 150;
			xAxisTickCount = 10;
			yAxisTickCount = 5;
			drawLegend = false;
		} // Chart
		public Chart(ChartData[] inChartData, bool inDrawLegend = false, int inWidth = 450, int inHeight = 150, int inXAxisTickCount = 10, int inYAxisTickCount = 5, string inChartName = "Chart", string inXAxisName = "X Axis", string inYAxisName = "Y Axis")
		{
			chartName = inChartName;
			xAxisName = inXAxisName;
			yAxisName = inYAxisName;
			width = inWidth;
			height = inHeight;
			xAxisTickCount = inXAxisTickCount;
			yAxisTickCount = inYAxisTickCount;
			drawLegend = inDrawLegend;
			chartDataElements = inChartData;
		} // Chart
		
		public string chartName = "Chart";
		public string xAxisName = "X Axis";
		public string yAxisName = "Y Axis";
		public int width = 450;
		public int height = 150;
		public int xAxisTickCount = 10;
		public int yAxisTickCount = 5;
		public bool drawLegend = false;
		public ChartData[] chartDataElements;
		public string ToJSON()
		{
			System.Text.StringBuilder json = new StringBuilder();
			
			json.Append("{");
			json.Append("\"chartName\" : \"" + chartName + "\", ");
			json.Append("\"xAxisName\" : \"" + xAxisName + "\", ");
			json.Append("\"yAxisName\" : \"" + yAxisName + "\", ");
			json.Append("\"width\" : " + width + ", ");
			json.Append("\"height\" : " + height + ", ");
			json.Append("\"xAxisTickCount\" : " + xAxisTickCount + ", ");
			json.Append("\"yAxisTickCount\" : " + yAxisTickCount + ", ");
			json.Append("\"drawLegend\" : " + drawLegend.ToString().ToLower() + " ");
			if (null != chartDataElements)
			{
				json.Append(",");
				json.Append("\"dataElements\" : [");
				for (int loop = 0; loop < chartDataElements.Length; loop++)
				{
					if (0 < loop)
					{
						json.Append(",");
					} // if
					json.Append(chartDataElements[loop].ToJSON());
				} // for
				json.Append("],");
				json.Append("\"minValue\" : " + GetMinValue() + ", ");
				json.Append("\"maxValue\" : " + GetMaxValue());
			} // if
			json.Append("}");
			
			return json.ToString();
		} // ToJSON
		public void ClearData()
		{
			for (int loop = 0; loop < chartDataElements.Length; loop++)
			{
				chartDataElements[loop].ClearData();
			} // for
		} // ClearData
		public float GetMinValue()
		{
			float minValue = float.MaxValue;
			
			for (int loop = 0; loop < chartDataElements.Length; loop++)
			{
				if (minValue > chartDataElements[loop].GetMinValue())
				{
					minValue = chartDataElements[loop].GetMinValue();
				} // if
			} // for
			return minValue;
		} // GetMinValue
		public float GetMaxValue()
		{
			float maxValue = float.MinValue;
			
			for (int loop = 0; loop < chartDataElements.Length; loop++)
			{
				if (maxValue < chartDataElements[loop].GetMaxValue())
				{
					maxValue = chartDataElements[loop].GetMaxValue();
				} // if
			} // for
			return maxValue;
		} // GetMaxValue
	} // class
} // namespace
