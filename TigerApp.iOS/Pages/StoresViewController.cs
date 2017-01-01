using Foundation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using TigerApp.iOS.Utils;
using TigerApp.Shared;
using TigerApp.Shared.Models;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages
{
    [Register("StoresViewController")]
    public class StoresViewController : BaseReactiveViewController<IStoresViewModel>
    {
        protected List<Store> Stores { get; private set; }

        private UITableView _storiesTable;

        public StoresViewController()
        {
            TransitioningDelegate = TransitionManager.Right;

            this.WhenActivated(dis =>
            {
                dis(ViewModel.WhenAnyValue(vm => vm.Stores).Subscribe(stores =>
                {
                    if (stores != null)
                    {
                        Stores = stores;
                        ReloadStores();
                    }
                }));

                ViewModel.GetStoreList();
            });
        }

        private void ReloadStores()
        {
            _storiesTable.Source = new StoresTableSource(_storiesTable, Stores);
            _storiesTable.ReloadData();
        }

        protected override void LayoutViews()
        {
            View.BackgroundColor = UIColor.White;

            var mainStack = UICommon.CreateStackView();

            var topStripe = new UIView { BackgroundColor = Colors.HexF3F3F2 };
            topStripe.HeightAnchor.ConstraintEqualTo(64).Active = true;

            var navStack = UICommon.CreateStackView(axis: UILayoutConstraintAxis.Horizontal, alignment: UIStackViewAlignment.LastBaseline, spacing: 5);
            navStack.LayoutMargins = new UIEdgeInsets(5, 5, 2, 5);
            navStack.LayoutMarginsRelativeArrangement = true;

            var back = UICommon.CreateBackIcon();
            back.AddGestureRecognizer(new UITapGestureRecognizer(() => { DismissViewController(true, null); }));

            var title = UICommon.CreateLabel(Fonts.TigerCandy.WithSize(34), UIColor.Black, UITextAlignment.Center, lines: 1);
            title.Text = Constants.Strings.StoriesPageTitle;

            var dummy = new UIView();
            dummy.WidthAnchor.ConstraintEqualTo(41).Active = true;
            dummy.HeightAnchor.ConstraintEqualTo(27).Active = true;

            navStack.AddArrangedSubview(back);
            navStack.AddArrangedSubview(title);
            navStack.AddArrangedSubview(dummy);

            _storiesTable = UICommon.CreateTableView();
            _storiesTable.RegisterClassForCellReuse(typeof(StoresTableCell), StoresTableCell.Identifier);
            _storiesTable.RegisterClassForHeaderFooterViewReuse(typeof(StoresTableHeaderCell), StoresTableHeaderCell.Identifier);

            mainStack.AddArrangedSubview(topStripe);
            mainStack.AddArrangedSubview(navStack);
            mainStack.AddArrangedSubview(_storiesTable);

            View.Add(mainStack);

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),

                NSLayoutConstraint.Create(navStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, topStripe, NSLayoutAttribute.Bottom, 1, -44),
                NSLayoutConstraint.Create(navStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 44),

                NSLayoutConstraint.Create(title, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, navStack, NSLayoutAttribute.Bottom, 1, 12)
            });
        }

        class StoresTableSource : UITableViewSource
        {
            public class Section
            {
                public string Title { get; set; }
                public List<Item> Items { get; set; }
                public bool Collapsed { get; set; }

                public Section(string title, List<Item> items, bool collapsed = true)
                {
                    Title = title;
                    Items = items;
                    Collapsed = collapsed;
                }
            }

            public class Item
            {
                public string Title { get; set; }
                public string Contact { get; set; }
                public string WorkingHours { get; set; }
            }

            private List<Section> _sections;
            private UITableView _tableView;

            public StoresTableSource(UITableView tableView, List<Store> stores)
            {
                _tableView = tableView;
                _sections = stores.GroupBy(s => s.Location.City.Region.Name)
                      .Select(group => new Section(group.Key, group.ToList().Select(g => new Item
                      {
                          Title = g.Location.City.Name,
                          Contact = $"{g.Address} - T {g.Phone}",
                          WorkingHours = string.Join("\n", g.OpeningHoursText.Split(';').Select(s => s.Trim()))
                      }).ToList())).ToList();
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell(StoresTableCell.Identifier, indexPath) as StoresTableCell;
                var item = _sections[indexPath.Section].Items[indexPath.Row];
                cell.Bind(item.Title, item.Contact, item.WorkingHours);
                return cell;
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                var cell = tableView.DequeueReusableHeaderFooterView(StoresTableHeaderCell.Identifier) as StoresTableHeaderCell;
                var header = _sections[(int)section];
                cell.Bind(header.Title, header.Collapsed, (int)section);
                cell.CollapseHandler = ToggleCollapse;
                return cell.ContentView;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return _sections[(int)section].Collapsed ? 0 : _sections[(int)section].Items.Count;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return _sections.Count;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return EstimatedHeightForHeader(tableView, section);
            }

            public override nfloat EstimatedHeightForHeader(UITableView tableView, nint section)
            {
                return 68f;
            }

            void ToggleCollapse(int index)
            {
                _sections[index].Collapsed = !_sections[index].Collapsed;
                _tableView.ReloadSections(new NSIndexSet((nuint)index), UITableViewRowAnimation.Automatic);
            }
        }

        class StoresTableHeaderCell : UITableViewHeaderFooterView
        {
            public static readonly NSString Identifier = new NSString(nameof(StoresTableHeaderCell));

            private UILabel _title;
            private UIImageView _collapseIndicator;

            private int _index;

            public Action<int> CollapseHandler { get; set; }

            public StoresTableHeaderCell(IntPtr handle) : base(handle)
            {
                ContentView.BackgroundColor = UIColor.White;
                BackgroundColor = UIColor.White;
                LayoutMargins = UIEdgeInsets.Zero;

                var mainStack = UICommon.CreateStackView();

                var contentStack = UICommon.CreateStackView(UILayoutConstraintAxis.Horizontal, alignment: UIStackViewAlignment.Center, spacing: 10);
                contentStack.LayoutMargins = new UIEdgeInsets(20, 40, 20, 25);
                contentStack.LayoutMarginsRelativeArrangement = true;

                _title = UICommon.CreateLabel(Fonts.TigerCandy.WithSize(28), UIColor.Black);

                contentStack.AddArrangedSubview(_title);

                _collapseIndicator = new UIImageView();
                _collapseIndicator.Image = UIImage.FromBundle("DropDownIndicator");
                _collapseIndicator.WidthAnchor.ConstraintEqualTo(20).Active = true;
                _collapseIndicator.HeightAnchor.ConstraintEqualTo(13).Active = true;

                contentStack.AddArrangedSubview(_collapseIndicator);

                mainStack.AddArrangedSubview(contentStack);
                mainStack.AddArrangedSubview(UICommon.CreateDivider(2));

                mainStack.AddGestureRecognizer(new UITapGestureRecognizer(() => { CollapseHandler(_index); }));

                ContentView.AddSubview(mainStack);

                ContentView.AddConstraints(new NSLayoutConstraint[] {
                    NSLayoutConstraint.Create (mainStack, NSLayoutAttribute.Left, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Left, 1, 0),
                    NSLayoutConstraint.Create (mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Top, 1, 0),
                    NSLayoutConstraint.Create (mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Width, 1, 0),
                    NSLayoutConstraint.Create (mainStack, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Bottom, 1, 0)
                });
            }

            public void Bind(string title, bool collapsed, int index)
            {
                _title.Text = title;
                _collapseIndicator.Hidden = collapsed;
                _index = index;
            }
        }

        class StoresTableCell : UITableViewCell
        {
            public static readonly NSString Identifier = new NSString(nameof(StoresTableCell));

            private UILabel _title, _contact, _workingHours;

            public StoresTableCell(IntPtr handle) : base(handle)
            {
                SelectionStyle = UITableViewCellSelectionStyle.None;
                ContentView.BackgroundColor = Colors.HexF3F3F2;
                BackgroundColor = Colors.HexF3F3F2;
                LayoutMargins = UIEdgeInsets.Zero;

                var mainStack = UICommon.CreateStackView();
                mainStack.LayoutMargins = new UIEdgeInsets(15, 40, 15, 20);
                mainStack.LayoutMarginsRelativeArrangement = true;

                _title = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(30), UIColor.Black);
                _contact = UICommon.CreateLabel(Fonts.FrutigerRegular.WithSize(18), UIColor.Black);
                _workingHours = UICommon.CreateLabel(Fonts.FrutigerRegular.WithSize(18), UIColor.Black);

                mainStack.AddArrangedSubview(_title);
                mainStack.AddArrangedSubview(_contact);
                mainStack.AddArrangedSubview(_workingHours);

                ContentView.AddSubview(mainStack);

                ContentView.AddConstraints(new NSLayoutConstraint[] {
                    NSLayoutConstraint.Create (mainStack, NSLayoutAttribute.Left, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Left, 1, 0),
                    NSLayoutConstraint.Create (mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Top, 1, 0),
                    NSLayoutConstraint.Create (mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Width, 1, 0),
                    NSLayoutConstraint.Create (mainStack, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Bottom, 1, 0)
                });
            }

            public void Bind(string title, string contact, string workingHours)
            {
                _title.Text = title;
                _contact.Text = contact;
                _workingHours.Text = workingHours;
            }
        }
    }
}