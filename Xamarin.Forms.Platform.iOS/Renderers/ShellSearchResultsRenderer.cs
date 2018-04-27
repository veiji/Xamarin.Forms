﻿using Foundation;
using System;
using System.Collections.Specialized;
using UIKit;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.iOS
{
	public class ShellSearchResultsRenderer : UITableViewController, IShellSearchResultsRenderer
	{
		#region IShellSearchResultsRenderer

		SearchHandler IShellSearchResultsRenderer.SearchHandler
		{
			get { return SearchHandler; }
			set
			{
				SearchHandler = value;
				OnSearchHandlerSet();
			}
		}

		UIViewController IShellSearchResultsRenderer.ViewController => this;

		#endregion IShellSearchResultsRenderer

		private readonly IShellContext _context;
		private DataTemplate _defaultTemplate;

		public event EventHandler<object> ItemSelected;

		// If data templates were horses, this is a donkey
		DataTemplate DefaultTemplate
		{
			get
			{
				if (_defaultTemplate == null)
				{
					_defaultTemplate = new DataTemplate(() =>
					{
						var label = new Label();
						label.SetBinding(Label.TextProperty, SearchHandler.DisplayMemberName ?? ".");
						label.HorizontalTextAlignment = TextAlignment.Center;
						label.VerticalTextAlignment = TextAlignment.Center;

						return label;
					});
				}
				return _defaultTemplate;
			}
		}

		public ShellSearchResultsRenderer(IShellContext context)
		{
			_context = context;
		}

		protected UITableViewRowAnimation DeleteRowsAnimation { get; set; } = UITableViewRowAnimation.Automatic;
		protected UITableViewRowAnimation InsertRowsAnimation { get; set; } = UITableViewRowAnimation.Automatic;
		protected UITableViewRowAnimation ReloadRowsAnimation { get; set; } = UITableViewRowAnimation.Automatic;
		protected UITableViewRowAnimation ReloadSectionsAnimation { get; set; } = UITableViewRowAnimation.Automatic;
		private ISearchHandlerController SearchController => SearchHandler;
		private SearchHandler SearchHandler { get; set; }

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var proxy = SearchController.ListProxy;
			int row = indexPath.Row;
			var context = proxy[row];

			var template = SearchHandler.ItemTemplate;

			if (template == null)
				template = DefaultTemplate;

			var cellId = ((IDataTemplateController)template.SelectDataTemplate(context, _context.Shell)).IdString;

			var cell = (UIContainerCell)tableView.DequeueReusableCell(cellId);

			if (cell == null)
			{
				var view = (View)template.CreateContent(context, _context.Shell);
				view.Parent = _context.Shell;
				view.BindingContext = context;
				cell = new UIContainerCell(cellId, view);
			}
			else
			{
				cell.View.BindingContext = context;
			}

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var item = SearchController.ListProxy[indexPath.Row];
			ItemSelected?.Invoke(this, item);
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			if (SearchController.ListProxy == null)
				return 0;
			return SearchController.ListProxy.Count;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
		}

		private NSIndexPath[] GetPaths(int section, int index, int count)
		{
			var paths = new NSIndexPath[count];
			for (var i = 0; i < paths.Length; i++)
				paths[i] = NSIndexPath.FromRowSection(index + i, section);

			return paths;
		}

		private void HandleListProxyChanged(object sender, ListProxyChangedEventArgs e)
		{
			if (e.OldList != null)
			{
				((INotifyCollectionChanged)e.OldList).CollectionChanged -= HandleProxyCollectionChanged;
			}
			// Full reset
			TableView.ReloadData();

			if (e.NewList != null)
			{
				((INotifyCollectionChanged)e.NewList).CollectionChanged += HandleProxyCollectionChanged;
			}
		}

		private void HandleProxyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			int section = 0;
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:

					if (e.NewStartingIndex == -1)
						goto case NotifyCollectionChangedAction.Reset;

					TableView.BeginUpdates();
					TableView.InsertRows(GetPaths(section, e.NewStartingIndex, e.NewItems.Count), InsertRowsAnimation);
					TableView.EndUpdates();

					break;

				case NotifyCollectionChangedAction.Remove:
					if (e.OldStartingIndex == -1)
						goto case NotifyCollectionChangedAction.Reset;
					TableView.BeginUpdates();
					TableView.DeleteRows(GetPaths(section, e.OldStartingIndex, e.OldItems.Count), DeleteRowsAnimation);

					TableView.EndUpdates();
					break;

				case NotifyCollectionChangedAction.Move:
					if (e.OldStartingIndex == -1 || e.NewStartingIndex == -1)
						goto case NotifyCollectionChangedAction.Reset;
					TableView.BeginUpdates();
					for (var i = 0; i < e.OldItems.Count; i++)
					{
						var oldIndex = e.OldStartingIndex;
						var newIndex = e.NewStartingIndex;

						if (e.NewStartingIndex < e.OldStartingIndex)
						{
							oldIndex += i;
							newIndex += i;
						}

						TableView.MoveRow(NSIndexPath.FromRowSection(oldIndex, section), NSIndexPath.FromRowSection(newIndex, section));
					}
					TableView.EndUpdates();
					break;

				case NotifyCollectionChangedAction.Replace:
					if (e.OldStartingIndex == -1)
						goto case NotifyCollectionChangedAction.Reset;
					TableView.BeginUpdates();
					TableView.ReloadRows(GetPaths(section, e.OldStartingIndex, e.OldItems.Count), ReloadRowsAnimation);
					TableView.EndUpdates();
					break;

				case NotifyCollectionChangedAction.Reset:
					TableView.ReloadData();
					return;
			}
		}

		private void OnSearchHandlerSet()
		{
			SearchController.ListProxyChanged += HandleListProxyChanged;
		}
	}
}