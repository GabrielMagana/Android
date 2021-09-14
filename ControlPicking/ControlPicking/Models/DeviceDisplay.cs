using Xamarin.Essentials;

namespace ControlPicking.Models
{
    public class DisplayInfoTest
    {
        public DisplayInfoTest()
        {
            DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChange;
        }
        void OnMainDisplayInfoChange(object sender, DisplayInfoChangedEventArgs e)
        {
            var displayInfo = e.DisplayInfo;
        }
    }

    public class KeepScreenLOnTest
    {
        public void ToggleScreenLock()
        { DeviceDisplay.KeepScreenOn = !DeviceDisplay.KeepScreenOn; }
    }
}
