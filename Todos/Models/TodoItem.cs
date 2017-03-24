using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Todos.Models
{
    class TodoItem : INotifyPropertyChanged
    {
        
        private string id;
        private string _title;
        public string title {
            get
            {
                return _title;
            }
                 set
            {
                if (_title != value)
                {
                    _title = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("title"));
                }
            }
                }


        public string description { get; set; }

        public bool completed { get; set; }
        public Windows.UI.Xaml.Media.ImageSource image { get; set; }

        //日期字段自己写
        public DateTime date { get; set; }

        public TodoItem(string title, string description, ImageSource image, DateTime date)
        {
            this.id = Guid.NewGuid().ToString(); //生成id
            this.title = title;
            this.description = description;
            this.completed = false; //默认为未完成
            this.date = date;
            this.image = image;
        }

        public TodoItem()
        {
        }

        public string Getid()
        {
            return id;}
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
