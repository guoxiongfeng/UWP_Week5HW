using Todos.Services;
using Todos.Models;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Data.Xml.Dom;
using System;
using Windows.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Todos
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            this.ViewModel = new ViewModels.TodoItemViewModel();
            GenerateTiles();
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
        }



        ViewModels.TodoItemViewModel ViewModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType() == typeof(ViewModels.TodoItemViewModel))
            {
                this.ViewModel = (ViewModels.TodoItemViewModel)(e.Parameter);
            }
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
             AppViewBackButtonVisibility.Collapsed;
        }

        private void TodoItem_ItemClicked(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.TodoItem)(e.ClickedItem);
            if (InlineToDoItemViewGrid.Visibility == Visibility.Collapsed)
            {
                Frame.Navigate(typeof(NewPage), ViewModel);
            }
            else
            {
                title.Text = ViewModel.SelectedItem.title;
                Details.Text = ViewModel.SelectedItem.description;
                DateShower.Date = ViewModel.SelectedItem.date;
                OverviewImg.Source = ViewModel.SelectedItem.image;
                createButton.Content = "update";
            }
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {

            if (InlineToDoItemViewGrid.Visibility == Visibility.Collapsed)
            {
                ViewModel.SelectedItem = null;
                Frame.Navigate(typeof(NewPage), ViewModel);
            }
           
        }
        private bool CanCreate(object sender, RoutedEventArgs e)
        {
            {
                if (DateShower.Date.AddHours(1) < System.DateTime.Now)
                {
                    new MessageDialog("设定时间必须大于等于当前时间！").ShowAsync();
                    return false;
                }
                if (title.Text == "")
                {
                    new MessageDialog("必须设置一个标题！").ShowAsync();
                    return false;
                }
                if (Details.Text == "")
                {
                    new MessageDialog("必须在Detail项填入内容！").ShowAsync();
                    return false;
                }
                return true;
            }

        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            title.Text = "";
            Details.Text = "";
            DateShower.Date = System.DateTime.Now;
        }

        private void CreateOrUpdateButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (CanCreate(sender, e))
            {
                if (createButton.Content.ToString() == "Create") CreateButton_Clicked(sender, e);
                else UpdateButton_Clicked(sender, e);
                if (ViewModel.SelectedItem != null) TileService.SendTileNotification(ViewModel.SelectedItem.title, ViewModel.SelectedItem.description, ViewModel.SelectedItem.image, ViewModel.SelectedItem.date);
            }
            
        }
        private void CreateButton_Clicked(object sender, RoutedEventArgs e)
        {
            ViewModel.AddTodoItem(title.Text, Details.Text, new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/background.jpg")), DateShower.Date.Date);

        }
        private void UpdateButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.UpdateTodoItem(ViewModel.SelectedItem.Getid(), title.Text, Details.Text,OverviewImg.Source, DateShower.Date.Date);
            }
        }

        private void GenerateTiles()
        {
            foreach (var item in ViewModel.AllItems)
            {
                TileService.SendTileNotification(item.title, item.description, item.image, item.date);
            }

        }

        private void setSelectItem(object sender, RoutedEventArgs e)
        {
            dynamic item = e.OriginalSource;

            ViewModel.SelectedItem =(TodoItem) item.DataContext;

        }

        private void ShareItem(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }



        private async void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            var deferral = args.Request.GetDeferral();

            try
            {
                Uri pictureUri = ((BitmapImage)ViewModel.SelectedItem.image).UriSource;
                var photoFile = await StorageFile.GetFileFromApplicationUriAsync(pictureUri);

                request.Data.Properties.Title = ViewModel.SelectedItem.title;
                request.Data.Properties.Description = ViewModel.SelectedItem.description;

                // It's recommended to use both SetBitmap and SetStorageItems for sharing a single image
                // since the target app may only support one or the other.


                RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromFile(photoFile);
                // It is recommended that you always add a thumbnail image any time you're sharing an image
                request.Data.Properties.Thumbnail = imageStreamRef;
                request.Data.SetBitmap(imageStreamRef);

                // Set Text to share for those targets that can't accept images
                var ymd = ViewModel.SelectedItem.date;
                request.Data.SetText(ViewModel.SelectedItem.description + '\n' + ymd.Year.ToString() + "年" + ymd.Month.ToString() + "月" + ymd.Day.ToString() + "日");
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}
