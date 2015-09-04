using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Stepper_Motor
{
    public sealed partial class MainPage
    {
        private readonly Uln2003Driver _uln2003Driver;

        public MainPage()
        {
            InitializeComponent();

            _uln2003Driver = new Uln2003Driver(26, 13, 6, 5);

            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;
        }
        
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await _uln2003Driver.TurnAsync(90, TurnDirection.Left);
                await _uln2003Driver.TurnAsync(90, TurnDirection.Right);
            });
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _uln2003Driver?.Dispose();
        }
    }
}
