﻿using System;

namespace Xsolla.Core
{
	public interface IInAppBrowser
	{
		event Action OpenEvent;

		event Action CloseEvent;

		event Action<string> UrlChangeEvent;
		
		bool IsOpened { get; }
		
		void Open(string url);

		void Close(float delay = 0f);

		void AddInitHandler(Action callback);

		void AddCloseHandler(Action callback);

		void AddUrlChangeHandler(Action<string> callback);

		void UpdateSize(int width, int height);
	}
}