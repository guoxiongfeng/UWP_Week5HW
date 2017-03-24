using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Todos.Models;
using System.Xml.Linq;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Todos.Services
{
    public class TileService
    {
        public static void SendTileNotification(string title, string description, ImageSource picture, DateTime date)
        {
            // Generate the notification content
            XDocument xdoc = XDocument.Load("tile.xml");
            //XmlDocument content = TileService.GenerateNotificationContent(title, description, picture, date);
            // Create the tile notification
            string temp = xdoc.ToString();
            string destXml = temp.Replace("titlestring", title);
            destXml = destXml.Replace("description", description);
            destXml = destXml.Replace("date", date.Date.ToString().Substring(0, 9));
            destXml = destXml.Replace("imagesrc", ((BitmapImage)picture).UriSource.ToString());
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(destXml);
            TileNotification notification = new TileNotification(xml);

            // Set the tag so that we can update (replace) this notification later
            notification.Tag = title;

            // Send the notification
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }



        /*
         * 不用XML的做法， 感觉挺好的啊。。。不懂为什么要求要xml
        private static XmlDocument GenerateNotificationContent(string title, string description, ImageSource picture, DateTime date)
        {
            TileBackgroundImage bg = new TileBackgroundImage();
            bg.Source = new BitmapImage(new Uri("ms-appx:///Assets/lightColorBG.jpg")).UriSource.ToString();
            var TileContent = new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    BackgroundImage = bg,
                    Children =
                            {
                        
                                new AdaptiveText()
                                {
                                    Text = title,
                                    HintAlign = AdaptiveTextAlign.Left
                                },

                                new AdaptiveText()
                                {
                                    Text = description,
                                    HintAlign = AdaptiveTextAlign.Left
                                },

                                new AdaptiveText()
                                {
                                     HintAlign = AdaptiveTextAlign.Right,
                                    

                                    Text = date.ToString()
                                },
                               new AdaptiveImage()
                                {
                                    Source = ((BitmapImage)picture).UriSource.ToString(),
                                }
                               
                            }
                }
            };
            var content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileLarge = TileContent,
                    TileMedium = TileContent,
                    //
                    TileWide = TileContent,
                    TileSmall = TileContent

                }
            };
            return content.GetXml();
        }*/
    }
}
