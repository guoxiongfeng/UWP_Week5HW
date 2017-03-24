using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Todos.ViewModels
{
    class TodoItemViewModel
    {
        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();
        public ObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }

        private Models.TodoItem selectedItem = default(Models.TodoItem);
        public Models.TodoItem SelectedItem { get { return selectedItem; } set { this.selectedItem = value; }  }

        public TodoItemViewModel()
        {
            // 加入两个用来测试的item
            this.allItems.Add(new Models.TodoItem("123", "123",new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/background.jpg")), DateTime.Now));
            this.allItems.Add(new Models.TodoItem("456", "456", new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/background.jpg")), DateTime.Now));
        }

        public void AddTodoItem(string title, string description, ImageSource image,  DateTime date)
        {
            this.allItems.Add(new Models.TodoItem(title, description, image, date));
            this.selectedItem = new Models.TodoItem(title, description, image, date);
        }

        public void RemoveTodoItem(string id)
        {
            // DIY
            foreach (var k in this.allItems)
                if (k.Getid() == id)
                {
                    this.allItems.Remove(k);
                    break;
                }
            // set selectedItem to null after remove
            this.selectedItem = null;
        }

        public void UpdateTodoItem(string id, string title, string description, ImageSource img, DateTime date)
        {
            // DIY
            foreach (var k in this.allItems)
                if (k.Getid() == id)
                {
                    k.title = title;
                    k.description = description;
                    k.date = date;
                    k.image = img;
                    break;
                }
            // set selectedItem to null after update
           // this.selectedItem = null;
        }

    }
}
