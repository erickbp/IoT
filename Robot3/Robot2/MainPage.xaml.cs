using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Robot2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private BackgroundWorker _worker;
        private BackgroundWorker _worker2;
        private CoreDispatcher _dispatcher;
        private TwoMotorsDriver _twoMotorsDriver;
        private Uln2003Driver _uln2003Driver;
        private UltrasonicDistanceSensor _ultrasonicDistanceSensor;
        private bool _isMovingForward;

        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;

            Unloaded += MainPage_Unloaded;
        }

        private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            _twoMotorsDriver = new TwoMotorsDriver(new Motor(6, 5), new Motor(13, 26));
            _uln2003Driver = new Uln2003Driver(16, 12, 25, 24);
            _ultrasonicDistanceSensor = new UltrasonicDistanceSensor(4, 27);

            _worker = new BackgroundWorker();
            _worker.DoWork += QueueListener_DoWork;
            _worker.RunWorkerAsync();

            _worker2 = new BackgroundWorker();
            _worker2.DoWork += AvoidCrash_DoWork;
            _worker2.RunWorkerAsync();
        }

        private async void QueueListener_DoWork(object sender, DoWorkEventArgs e)
        {
            while (_worker != null && !_worker.CancellationPending)
            {
                try
                {
                    var result = await ServiceBusHelper.ReceiveAndDeleteMessage();
                    var json = JsonObject.Parse(result);
                    var action = json.GetNamedString("action");

                    switch (action)
                    {
                        case "Stop":
                            _twoMotorsDriver.Stop();
                            _isMovingForward = false;
                            break;
                        case "MoveForward":
                            _twoMotorsDriver.MoveForward();
                            _isMovingForward = true;
                            break;
                        case "MoveBack":
                            _twoMotorsDriver.MoveBackward();
                            _isMovingForward = false;
                            break;

                        case "Turn":
                            var angle = int.Parse(json.GetNamedString("param"));

                            await _uln2003Driver.TurnAsync(angle, TurnDirection.Right);
                            //await _twoMotorsDriver.TurnRightAsync(angle);

                            _isMovingForward = false;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    await WriteLog(ex.Message);
                }
                await Task.Delay(1000);
            }
        }

        private async void AvoidCrash_DoWork(object sender, DoWorkEventArgs e)
        {
            while (_worker2 != null && !_worker2.CancellationPending)
            {
                await Task.Delay(100);

                if (!_isMovingForward)
                    continue;

                var distance = await _ultrasonicDistanceSensor.GetDistanceInCmAsync(1000);
                if (distance > 35.0)
                    continue;

                _twoMotorsDriver.Stop();
            }
        }

        private void MainPage_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _worker.CancelAsync();
            _worker = null;
            _worker2.CancelAsync();
            _worker2 = null;

            _twoMotorsDriver.Dispose();
            _uln2003Driver.Dispose();
            _ultrasonicDistanceSensor.Dispose();
        }

        public async Task WriteLog(string text)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Log.Text += $"{text} | ";
            });
        }

    }
}
