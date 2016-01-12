using System.Windows;
using EstateAgent.ViewModels;

namespace EstateAgent.Views
{
    /// <summary>
    /// Interaction logic for DashBoard.xaml
    /// </summary>
    public partial class DashBoard : Window
    {
        public DashBoard(DashBoardViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
