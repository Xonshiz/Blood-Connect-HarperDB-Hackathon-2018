using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BloodConnect.models;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BloodConnect
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainScreen : TabbedPage
	{
        private string primaryDomain_main, user_blood_type_main, user_name_main, user_id_main, user_state_main, user_age_main, user_country_main, user_area_code_main, user_phone_main, user_email_main;
        private ObservableCollection<pastDonationData_List> _pastDonationsList_Private = new ObservableCollection<pastDonationData_List> { };
        private HttpClient _client = new HttpClient();

        public MainScreen ()
		{
			InitializeComponent();
            primaryDomain_main = MainPage.primaryDomain;
            user_id_main = MainPage.user_id;

            user_blood_type_main = MainPage.user_blood_type;
            user_name_main = MainPage.user_name;

        }

        protected override async void OnAppearing()
        {

            pastDonationList.IsRefreshing = true;
            await ListFetching();
            pastDonationList.ItemsSource = _pastDonationsList_Private;
            pastDonationList.EndRefresh();
            if (_pastDonationsList_Private.Count == 0)
            {
                pastDonationList.IsVisible = false;
                tempLable.IsVisible = true;
            }

        }

        public async Task ListFetching()
        {
            try
            {
                var data_to_send = new FormUrlEncodedContent(new[]
                        {
                        new KeyValuePair<string, string>("user_id", user_id_main)
                    });

                var content = await _client.PostAsync(primaryDomain_main + "/api/data_operations/list_of_donations.php", data_to_send);

                //await DisplayAlert("user_id_main", user_id_main, "Ok");
                var temp_response = await content.Content.ReadAsStringAsync();
                //await DisplayAlert("Temp Resp", temp_response.ToString(), "Ok");

                try
                {
                    dynamic dynObj = JsonConvert.DeserializeObject(temp_response);
                    _pastDonationsList_Private.Clear();

                    foreach (var data in dynObj)
                    {

                        _pastDonationsList_Private.Add(new pastDonationData_List { requesterName = Convert.ToString(data.requester_id), donationDate = Convert.ToString(data.donation_date), donationCountry = Convert.ToString(data.donation_country) });

                    }

                }
                catch (Exception BadUserID)
                {
                    //await DisplayAlert("Bad Request", "Wrong User ID", "Ok");
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Failed", "Could Not Connect To Servers.", "Ok");
            }

        }

        async void pastDonationListView_Refresh(object sender, System.EventArgs e)
        {
            await ListFetching();
            pastDonationList.ItemsSource = _pastDonationsList_Private;
            pastDonationList.EndRefresh();
        }

        private async void newRequest_Clicked(object sender, EventArgs e)
        {
            //DisplayAlert("Register Clicked", "You Clicked Regsiter", "Ok");
            await Navigation.PushModalAsync(new NewRequest());
        }

    }
}