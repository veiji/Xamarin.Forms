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
			class Item
			{
				public string Name { get; set; }

				public int Key { get; set; }
			}

			public _1760Master()
			{
				var items = new List<Item>
				{
					new Item { Name = "Test Page", Key = 1 }
				};

				var cell = new DataTemplate(typeof(TextCell));
				cell.SetBinding(TextCell.TextProperty, "Name");

				var menuView = new ListView(ListViewCachingStrategy.RetainElement)
				{
					ItemTemplate = cell,
					ItemsSource = items,
					HasUnevenRows = true
				};
				menuView.ItemSelected += OnMenuClicked;

				Content = menuView;
				Title = "Test App";
			}

			void OnMenuClicked(object sender, SelectedItemChangedEventArgs e)
			{
				var item = (Item)e.SelectedItem;
				Page next;
				switch (item.Key)
				{
					case 1:
						next = new _1760TestPage();
						break;
					default:
						next = new _1760TestPage();
						break;
				}

				//var detail = new NavigationPage(next);
				var mainPage = ((MasterDetailPage)Parent);
				mainPage.Detail = next;
				mainPage.IsPresented = false;
			}
		}

		[Preserve(AllMembers = true)]
		public class _1760TestPage : ContentPage
		{
			public async Task DisplayPage()
			{
				IsBusy = true;
				HeaderPageContent = new Label {Text = "Before the await", TextColor = Color.Black};

				await Task.Delay(5000);
				HeaderPageContent = new Label { Text = "After the await", TextColor = Color.Black}; 
			}

			private bool _useTitleImage = true;
			//private bool _useTitleImage = false;
         
			private Label _headerLabel;
			public string HeaderLabel
			{
				get { return _headerLabel.Text; }
				set { _headerLabel.Text = value; }
			}

			private ContentView _headerPageContent;
			public View HeaderPageContent
			{
				set { _headerPageContent.Content = value; }
			}

			public new Color BackgroundColor
			{
				set { _headerPageContent.BackgroundColor = value; }
			}


			public _1760TestPage()
			{
				CreateHeaderPage(null, string.Empty, true);
				DisplayPage();
			}

			private void CreateHeaderPage(Layout page, string headerText, bool scrollEnabled)
			{

				Icon = "hamenu.png"; 
				Title = "Memory Test";  

				_headerPageContent = new ContentView
				{
					Content = page,
					BackgroundColor = Color.White,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Margin = 20
				};

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