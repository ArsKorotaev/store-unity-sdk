﻿#if UNITY_EDITOR
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla.Core
{
	public class BuildPostProcess : IPostprocessBuildWithReport
	{
		public int callbackOrder { get; }

		public void OnPostprocessBuild(BuildReport report)
		{
			AddSteamAppIdFile(report);
			PackInAppBrowser(report);
		}

		private void AddSteamAppIdFile(BuildReport report)
		{
			if (report.summary.platformGroup != BuildTargetGroup.Standalone)
				return;
			
			if (!XsollaSettings.UseSteamAuth)
				return;

			var filePath = Path.GetDirectoryName(report.summary.outputPath);
			filePath = Path.Combine(filePath, "steam_appid.txt");
			File.WriteAllText(filePath, XsollaSettings.SteamAppId);
		}

		private void PackInAppBrowser(BuildReport report)
		{
			if (report.summary.platformGroup != BuildTargetGroup.Standalone)
				return;

			if (!XsollaSettings.InAppBrowserEnabled || !XsollaSettings.PackInAppBrowserInBuild)
				return;

			const int browserRevision = BrowserFetcher.DefaultRevision;

			var browserPlatform = Platform.Unknown;
			switch (report.summary.platform)
			{
				case BuildTarget.StandaloneOSX:
					browserPlatform = Platform.MacOS;
					break;
				case BuildTarget.StandaloneWindows:
					browserPlatform = Platform.Win32;
					break;
				case BuildTarget.StandaloneWindows64:
					browserPlatform = Platform.Win64;
					break;
			}

			if (browserPlatform == Platform.Unknown)
			{
				Debug.LogWarning($"Build target \"{report.summary.platform}\" is not supported. Packing browser in the build is skipped");
				return;
			}

			var buildBrowserDirectory = Path.GetDirectoryName(report.summary.outputPath);
			buildBrowserDirectory = Path.Combine(buildBrowserDirectory, ".local-chromium");

			if (Directory.Exists(buildBrowserDirectory))
				Directory.Delete(buildBrowserDirectory, true);

			var projectBrowserDirectory = Path.Combine(Directory.GetCurrentDirectory(), ".local-chromium");
			projectBrowserDirectory = Path.Combine(projectBrowserDirectory, $"{browserPlatform}-{browserRevision}");

			if (Directory.Exists(projectBrowserDirectory))
			{
				buildBrowserDirectory = Path.Combine(buildBrowserDirectory, $"{browserPlatform}-{browserRevision}");

				foreach (var dirPath in Directory.GetDirectories(projectBrowserDirectory, "*", SearchOption.AllDirectories))
					Directory.CreateDirectory(dirPath.Replace(projectBrowserDirectory, buildBrowserDirectory));

				foreach (var filePath in Directory.GetFiles(projectBrowserDirectory, "*.*", SearchOption.AllDirectories))
					File.Copy(filePath, filePath.Replace(projectBrowserDirectory, buildBrowserDirectory), true);
			}
			else
			{
				if (Application.internetReachability == NetworkReachability.NotReachable)
				{
					Debug.LogWarning("Internet connection is unavailable. Packing browser in the build is skipped");
					return;
				}

				var fetcherOptions = new BrowserFetcherOptions
				{
					Platform = browserPlatform,
					Path = buildBrowserDirectory
				};

				var browserFetcher = new BrowserFetcher(fetcherOptions);
				Task.Run(async () => await browserFetcher.DownloadAsync(browserRevision)).Wait();
			}
		}
	}
}

#endif