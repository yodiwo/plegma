using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;

namespace Yodiwo
{
    public static class MessageBox
    {
        public static async Task Show(string Message)
        {
            var box = new MessageDialog(Message, "Message");
            await box.ShowAsync();
        }

        public static async Task Show(string Message, string Title)
        {
            var box = new MessageDialog(Message, Title);
            await box.ShowAsync();
        }
    }
}
