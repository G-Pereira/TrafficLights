using System;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using Windows.System.Threading;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace TrafficLights
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        private const int RED_LED_PIN = 5;
        private const int YELLOW_LED_PIN = 6;
        private const int GREEN_LED_PIN = 13;
        private GpioPin red_pin;
        private GpioPin yellow_pin;
        private GpioPin green_pin;
        private GpioPinValue redPinValue;
        private GpioPinValue yellowPinValue;
        private GpioPinValue greenPinValue;
        private ThreadPoolTimer timer;
        private int state = 0;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            InitGPIO();
            timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromMilliseconds(500));
        }
        private void InitGPIO()
        {
            // Initializes controller
            GpioController gpio = GpioController.GetDefault();

            // Uses controller to setup pins
            red_pin = gpio.OpenPin(RED_LED_PIN);
            yellow_pin = gpio.OpenPin(YELLOW_LED_PIN);
            green_pin = gpio.OpenPin(GREEN_LED_PIN);

            // Sets initial pins values
            red_pin.Write(GpioPinValue.High);
            yellow_pin.Write(GpioPinValue.High);
            green_pin.Write(GpioPinValue.High);

            // Sets pin as output
            red_pin.SetDriveMode(GpioPinDriveMode.Output);
            yellow_pin.SetDriveMode(GpioPinDriveMode.Output);
            green_pin.SetDriveMode(GpioPinDriveMode.Output);
        }
        private void Timer_Tick(ThreadPoolTimer timer)
        {
            switch (state)
            {
                case 0:
                    redPinValue = GpioPinValue.High;
                    yellowPinValue = GpioPinValue.Low;
                    greenPinValue = GpioPinValue.Low;
                    state = 1;
                    break;
                case 1:
                    redPinValue = GpioPinValue.Low;
                    yellowPinValue = GpioPinValue.High;
                    greenPinValue = GpioPinValue.Low;
                    state = 2;
                    break;
                case 2:
                    redPinValue = GpioPinValue.Low;
                    yellowPinValue = GpioPinValue.Low;
                    greenPinValue = GpioPinValue.High;
                    state = 0;
                    break;
            }
            red_pin.Write(redPinValue);
            yellow_pin.Write(yellowPinValue);
            green_pin.Write(greenPinValue);
        }
    }
}
