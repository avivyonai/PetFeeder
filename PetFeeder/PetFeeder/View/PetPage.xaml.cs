using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PetFeeder
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PetPage : ContentPage
	{
		public PetPage ()
		{
			InitializeComponent ();
		}
	}
}