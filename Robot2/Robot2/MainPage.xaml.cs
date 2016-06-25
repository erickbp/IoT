using System;
using System.ComponentModel;
using System.Threading.Tasks;
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
        private bool isMovingForward;

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
            _worker.DoWork += DoWork;
            _worker.RunWorkerAsync();

            _worker2 = new BackgroundWorker();
            _worker2.DoWork += DoWork2;
            _worker2.RunWorkerAsync();
        }

        private async void DoWork(object sender, DoWorkEventArgs e)
        {
            while (_worker != null && !_worker.CancellationPending)
            {
                var result = await ServiceBusHelper.ReceiveAndDeleteMessage();
               switch (result)
               {
                    case "SMR":
                       await _uln2003Driver.TurnAsync(90, TurnDirection.Right);
                        break;
                    case "SML":
                        await _uln2003Driver.TurnAsync(90, TurnDirection.Left);
                        break;
                    case "F":
                       _twoMotorsDriver.MoveForward();
                       isMovingForward = true;
                       break;
                   case "S":
                       _twoMotorsDriver.Stop();
                       isMovingForward = false;
                        break;
                    case "R":
                        await _twoMotorsDriver.TurnRightAsync();
                       isMovingForward = false;
                        break;
                    case "L":
                        await _twoMotorsDriver.TurnLeftAsync();
                       isMovingForward = false;
                        break;
                    case "B":
                        _twoMotorsDriver.MoveBackward();
                       isMovingForward = false;
                        break;

                    case "TR":
                        await _twoMotorsDriver.TurnRightAsync(45);
                       isMovingForward = false;
                        break;
                    case "TL":
                        await _twoMotorsDriver.TurnLeftAsync(45);
                       isMovingForward = false;
                        break;
                    case "BR":
                        await _twoMotorsDriver.TurnRightAsync(100);
                       isMovingForward = false;
                        break;
                    case "BL":
                        await _twoMotorsDriver.TurnLeftAsync(100);
                       isMovingForward = false;
                        break;

                }

                await Task.Delay(1000);
            }
        }

        private async void DoWork2(object sender, DoWorkEventArgs e)
        {
            while (_worker2 != null && !_worker2.CancellationPending)
            {
                await Task.Delay(100);

                if (!isMovingForward)
                    continue;

                var distance = await _ultrasonicDistanceSensor.GetDistanceInCmAsync(1000);
                if (distance > 35.0)
                    continue;

                _twoMotorsDriver.Stop();
            }
        }

        private async void DoWork3(object sender, DoWorkEventArgs e)
        {
            var driver = new TwoMotorsDriver(new Motor(27, 22), new Motor(5, 6));
            var ultrasonicDistanceSensor = new UltrasonicDistanceSensor(23, 24);

            await WriteLog("Moving forward");

            while (_worker2 != null && !_worker2.CancellationPending)
            {
                driver.MoveForward();

                await Task.Delay(200);

                var distance = await ultrasonicDistanceSensor.GetDistanceInCmAsync(1000);
                if (distance > 35.0)
                    continue;

                await WriteLog($"Obstacle found at {distance} cm or less. Turning right");

                await driver.TurnRightAsync();

                await WriteLog("Moving forward");
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

        private async Task WriteLog(string text)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Log.Text += $"{text} | ";
            });
        }

    }
}
