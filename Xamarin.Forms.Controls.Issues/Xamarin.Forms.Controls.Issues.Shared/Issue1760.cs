using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1760, "Content set after an await is not visible", PlatformAffected.Android)]
	public class Issue1760 : TestMasterDetailPage
	{
		protected override void Init()
		{
			Master = new _1760Master();
			Detail = new _1760TestPage();
		}

		[Preserve(AllMembers = true)]
		public class _1760Master : ContentPage
		{
			public _1760Master()
			{
				var menuView = new ListView(ListViewCachingStrategy.RetainElement)
				{
					ItemsSource = new List<string> { "Test Page" }
				};

				menuView.ItemSelected += OnMenuClicked;

				Content = menuView;
				Title = "GH 1760 Test App";
			}

			void OnMenuClicked(object sender, SelectedItemChangedEventArgs e)
			{
				var mainPage = (MasterDetailPage)Parent;
				mainPage.Detail = new _1760TestPage();
				mainPage.IsPresented = false;
			}
		}

		[Preserve(AllMembers = true)]
		public class _1760TestPage : ContentPage
		{
			public 
				//async Task 
				void
				DisplayPage()
			{
				IsBusy = true;
				HeaderPageContent = new Label {Text = "Before the await", TextColor = Color.Black};

				//await Task.Delay(3000);
				HeaderPageContent = new Label { Text = "After the await", TextColor = Color.Black}; 
			}

			ContentView _headerPageContent;
			public View HeaderPageContent
			{
				set => _headerPageContent.Content = value;
			}

			public _1760TestPage()
			{
				CreateHeaderPage();
				DisplayPage();
			}

			void CreateHeaderPage()
			{
				_headerPageContent = new ContentView
				{
					Content = new Label { Text = "_1760 Test Page Content" },
					BackgroundColor = Color.White,
					Margin = 40
				};

				Title = "_1760 Test Page";  

				// works
				//Content = new StackLayout
				//{
				//	Children = { _headerPageContent }
				//};

				// doesn't work
				Content = new ScrollView
				{
					Content = _headerPageContent
				};

			}
		}
	}
}