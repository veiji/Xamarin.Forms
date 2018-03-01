using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 1, "Issue Description", PlatformAffected.Default)]
	public class Bugzilla1 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			// Initialize ui here instead of ctor
			Content = new Label
			{
				AutomationId = "IssuePageLabel",
				Text = "See if I'm here"
			};
		}

#if UITEST
		[Test]
		public void Issue1Test ()
		{
			RunningApp.Screenshot ("I am at Issue 1");
			RunningApp.WaitForElement (q => q.Marked ("IssuePageLabel"));
			RunningApp.Screenshot ("I see the Label");
		}
#endif
	}

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1355, "Setting Main Page in quick succession causes crash on Android",
		PlatformAffected.Android)]
	public class GitHub1355 : TestContentPage
	{
		int _runCount = 0;
		private int _maxRunCount = 2;

		protected override void Init()
		{
			Appearing += OnAppearing;
		}

		private void OnAppearing(object o, EventArgs eventArgs)
		{
			Application.Current.MainPage = CreatePage();
		}

		ContentPage CreatePage()
		{
			var page = new ContentPage();
			
			page.Content = new Label { Text = $"Iteration: {_runCount}"};
			page.Title = $"CreatePage Iteration: {_runCount}";

			page.Appearing += (sender, args) =>
			{
				_runCount += 1;
				if (_runCount <= _maxRunCount)
				{
					Application.Current.MainPage = new NavigationPage(CreatePage());
				}
			};

			return page;
		}
	}
}