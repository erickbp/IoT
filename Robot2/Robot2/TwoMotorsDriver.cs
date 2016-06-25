using System;
using System.Threading.Tasks;

namespace Robot2
{
    public class TwoMotorsDriver :IDisposable
    {
        private readonly Motor _leftMotor;
        private readonly Motor _rightMotor;

        public TwoMotorsDriver(Motor leftMotor, Motor rightMotor)
        {
            _leftMotor = leftMotor;
            _rightMotor = rightMotor;
        }

        public void Stop()
        {
            _leftMotor.Stop();
            _rightMotor.Stop();
        }

        public void MoveForward()
        {
            _leftMotor.MoveForward();
            _rightMotor.MoveForward();
        }

        public void MoveBackward()
        {
            _leftMotor.MoveBackward();
            _rightMotor.MoveBackward();
        }

        public async Task TurnRightAsync()
        {
            _leftMotor.MoveForward();
            _rightMotor.MoveBackward();

            await Task.Delay(TimeSpan.FromMilliseconds(250));

            _leftMotor.Stop();
            _rightMotor.Stop();
        }

        public async Task TurnLeftAsync()
        {
            _leftMotor.MoveBackward();
            _rightMotor.MoveForward();

            await Task.Delay(TimeSpan.FromMilliseconds(250));

            _leftMotor.Stop();
            _rightMotor.Stop();
        }

        public async Task TurnRightAsync(int angle)
        {
            _leftMotor.MoveForward();
            _rightMotor.MoveBackward();

            await Task.Delay(1000 / (360 / angle));

            _leftMotor.Stop();
            _rightMotor.Stop();
        }

        public async Task TurnLeftAsync(int angle)
        {
            _leftMotor.MoveBackward();
            _rightMotor.MoveForward();

            await Task.Delay(1000 / (360 / angle));

            _leftMotor.Stop();
            _rightMotor.Stop();
        }

        public void Dispose()
        {
            _leftMotor.Dispose();
            _rightMotor.Dispose();
        }
    }

}
