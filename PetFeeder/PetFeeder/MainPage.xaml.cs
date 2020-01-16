using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PetFeeder
{
    public partial class MainPage : MasterDetailPage
    {
        public List<MasterPageItem> menuList { get; set; }
        NavigationPage PetPageNav = new NavigationPage((Page)Activator.CreateInstance(typeof(PetPage)));
        NavigationPage SettingsPageNav = new NavigationPage((Page)Activator.CreateInstance(typeof(SettingsPage)));
        NavigationPage TimerPageNav = new NavigationPage((Page)Activator.CreateInstance(typeof(TimerPage)));

        public MainPage()
        {
            InitializeComponent();

            menuList = new List<MasterPageItem>();

            //Fot Android / IOS icons
            var page1 = new MasterPageItem() { id = 1, Title = "Your Pet", Icon = "Home.png" };
            var page2 = new MasterPageItem() { id = 2, Title = "Timer", Icon = "Configuration.png" };
            var page3 = new MasterPageItem() { id = 3, Title = "Settings", Icon = "ProfileSetting.png" };

            menuList.Add(page1);
            menuList.Add(page2);
            menuList.Add(page3);

            navigationDrawerList.ItemsSource = menuList;

            Detail = PetPageNav;
        }

        async private void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var myselecteditem = e.Item as MasterPageItem;

            switch (myselecteditem.id)
            {
                case 1:
                    Detail = PetPageNav;
                    break;
                case 2:
                    Detail = TimerPageNav;
                    break;
                case 3:
                    Detail = SettingsPageNav;
                    break;

            }
            ((ListView)sender).SelectedItem = null;
            IsPresented = false;
        }

    }
}
