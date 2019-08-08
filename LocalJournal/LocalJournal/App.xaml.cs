﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LocalJournal.Services;
using LocalJournal.Views;

namespace LocalJournal
{
	public partial class App : Application
	{

		public App()
		{
			InitializeComponent();

			//DependencyService.Register<RawSerializer>();
			DependencyService.Register<YamlFrontSerializer>();
			DependencyService.Register<AesBasicCrypto>();
			MainPage = new MainPage();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
