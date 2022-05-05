using System.Windows.Controls;
using System.Windows.Media;
using TodoNotes.ViewModels;

namespace TodoNotes.Views.Window
{
    /// <summary>
    /// Interaction logic for TextChat
    /// </summary>
    public partial class TextChat : UserControl
    {
        public TextChat()
        {
            InitializeComponent();
            if (DataContext is TextChatViewModel vm)
                vm.MessageCollectionChanged += ScrollGrid;
        }
        private void ScrollGrid()
        {
            if (VisualTreeHelper.GetChild(MessagesList, 0) is Decorator border)
            {
                if (border.Child is ScrollViewer scroll) scroll.ScrollToEnd();
            }
        }
    }
}
