using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Todos.Services;
using Todos.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace Todos
{
    public sealed partial class NewPage : Page
    {
        public NewPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;

        }
        private TodoItem newTodo = new TodoItem();
        private ViewModels.TodoItemViewModel ViewModel;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = ((ViewModels.TodoItemViewModel)e.Parameter);
            if (ViewModel.SelectedItem == null)
            {
                createButton.Content = "Create";
                //var i = new MessageDialog("Welcome!").ShowAsync();
            }
            else
            {
                createButton.Content = "Update";
                title.Text = ViewModel.SelectedItem.title;
                Details.Text = ViewModel.SelectedItem.description;
                DateShower.Date =  ViewModel.SelectedItem.date;
                Pic.Source = ViewModel.SelectedItem.image;
                // ...
            }
        }
        private void CreateOrUpdateButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (CanCreate(sender, e))
            {
               
                if (createButton.Content.ToString() == "Create") CreateButton_Clicked(sender, e);
                else UpdateButton_Clicked(sender, e);
                    TileService.SendTileNotification(ViewModel.SelectedItem.title, ViewModel.SelectedItem.description, ViewModel.SelectedItem.image, ViewModel.SelectedItem.date);

                Frame.Navigate(typeof(MainPage), ViewModel);
            }
            
        }
        private void CreateButton_Clicked(object sender, RoutedEventArgs e)
        {
            // check the textbox and datapicker
            // if ok
            if (newTodo.image != null)
            {
                ViewModel.AddTodoItem(title.Text, Details.Text, newTodo.image, DateShower.Date.Date);
                newTodo.image = null;
            }
            else ViewModel.AddTodoItem(title.Text, Details.Text, new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/background.jpg")), DateShower.Date.Date);

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
        void ResumeDate()
        {
            DateShower.Date = System.DateTime.Now;
        }

        private void OnClickCancel(object sender, RoutedEventArgs e)
        {
            title.Text = "";
            Details.Text = "";
            ResumeDate();
        }
        private  void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.RemoveTodoItem(ViewModel.SelectedItem.Getid());
            }
            Frame.Navigate(typeof(MainPage), ViewModel);
        }


        private void UpdateButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
                {
                    ViewModel.UpdateTodoItem(ViewModel.SelectedItem.Getid(), title.Text, Details.Text, Pic.Source, DateShower.Date.Date);
                }

        }

        private async void SelectPicture(object sender, RoutedEventArgs e)
        {
            // Set up the file picker.
            Windows.Storage.Pickers.FileOpenPicker openPicker =
                new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            openPicker.ViewMode =
                Windows.Storage.Pickers.PickerViewMode.Thumbnail;

            // Filter to include a sample subset of file types.
            openPicker.FileTypeFilter.Clear();
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".jpg");

            // Open the file picker.
            Windows.Storage.StorageFile file =
                await openPicker.PickSingleFileAsync();

            // 'file' is null if user cancels the file picker.
            if (file != null)
            {
                var destFile = await file.CopyAsync(Windows.Storage.ApplicationData.Current.LocalFolder, file.Name, NameCollisionOption.ReplaceExisting);
                Uri PicUri = new Uri(new Uri("ms-appdata:///local/"), destFile.Name);
                // Open a stream for the selected file.
                // The 'using' block ensures the stream is disposed
                // after the image is loaded.
                using (Windows.Storage.Streams.IRandomAccessStream fileStream =
                    await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap.
                    Windows.UI.Xaml.Media.Imaging.BitmapImage bitmapImage =
                        new Windows.UI.Xaml.Media.Imaging.BitmapImage();

                    /*  bitmapImage.SetSource(fileStream);
                      Pic.Source = bitmapImage;*/
                    Pic.Source = new BitmapImage(PicUri);
                    if (ViewModel.SelectedItem != null)
                        ViewModel.SelectedItem.image = Pic.Source;
                    else newTodo.image = Pic.Source;
                }
            }
           
        }
    }
}
