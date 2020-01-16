using Android.Telephony;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace PetFeeder
{
	public class StartPageModel : INotifyPropertyChanged
    {
        public ICommand RegisterBut { get; }

        public StartPageModel ()
		{
            int n = 0;
            //n = DependencyService.Get<DeviceDetails>().GetNumber();

            RegisterBut = new Command(Register);
        }

        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private async void Register()
        {
            Application.Current.Properties["RegisteredId"] = 32;
            Application.Current.MainPage = new MainPage();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}