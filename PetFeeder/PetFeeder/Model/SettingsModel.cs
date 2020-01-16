using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace PetFeeder
{
    class SettingsModel : INotifyPropertyChanged
    {
        public ICommand ForgetCommand { get; }

        public SettingsModel()
        {
            ForgetCommand = new Command(Forget);
        }

        private async void Forget()
        {
            Application.Current.Properties.Clear();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
