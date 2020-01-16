using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

[assembly: Dependency(typeof(PetFeeder.Droid.Device_Android))]
namespace PetFeeder.Droid
{
    public class Device_Android : DeviceInfo
    {
        public int GetNumber()
        {
            TelephonyManager mTelephonyMgr;

            mTelephonyMgr = (TelephonyManager)Android.App.Application.Context.GetSystemService(Context.TelephonyService);

            var Number = mTelephonyMgr.Line1Number;

            return 777;// int.Parse(Number);
        }
    }
}