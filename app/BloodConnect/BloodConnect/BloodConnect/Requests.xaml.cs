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
	public partial class Requests : ContentPage
	{
        private string primaryDomain_main, user_blood_type_main, user_name_main, user_id_main, user_state_main, user_age_main, user_country_main, user_area_code_main, user_phone_main, user_email_main;
        private ObservableCollection<PendingRequests> _pendingRequestsList_Private = new ObservableCollection<PendingRequests> { };
        private HttpClient _client = new HttpClient();

        public Requests ()
		{
			InitializeComponent ();
            //pendingRequestsList.ItemsSource = GetPendingRequests();

            primaryDomain_main = MainPage.primaryDomain;
            user_id_main = MainPage.user_id;
            user_state_main = MainPage.user_state;
            user_country_main = MainPage.user_country;
            user_blood_type_main = MainPage.user_blood_type;

        }

        protected override async void OnAppearing()
        {

            pendingRequestsList.IsRefreshing = true;
            await ListFetching();
            pendingRequestsList.ItemsSource = _pendingRequestsList_Private;
            pendingRequestsList.EndRefresh();

        }

        async void pendingRequestsListView_Refresh(object sender, System.EventArgs e)
        {
            await ListFetching();
            pendingRequestsList.ItemsSource = _pendingRequestsList_Private;
            pendingRequestsList.EndRefresh();
        }

        async void pendingRequestSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var selectedRequest = e.SelectedItem as PendingRequests;
            var result = await DisplayAlert("Confirmation", "Do You Want To Donate Blood To This Entry?", "Ok", "No");
            if (result)
            {
                //Yes Pressed
                await RequestAccept(selectedRequest.donation_id);
                await ListFetching();
                pendingRequestsList.ItemsSource = _pendingRequestsList_Private;
                pendingRequestsList.EndRefresh();

            }

            pendingRequestsList.SelectedItem = null;

        }

        public async Task ListFetching()
        {
            try
            {
                var data_to_send = new FormUrlEncodedContent(new[]
                        {
                        new KeyValuePair<string, string>("user_state", user_state_main),
                        new KeyValuePair<string, string>("user_country", user_country_main),
                        new KeyValuePair<string, string>("user_blood_type", user_blood_type_main)
                    });

                var content = await _client.PostAsync(primaryDomain_main + "/api/data_operations/pending_blood_requests.php", data_to_send);

                var temp_response = await content.Content.ReadAsStringAsync();

                try
                {
                    dynamic dynObj = JsonConvert.DeserializeObject(temp_response);
                    _pendingRequestsList_Private.Clear();

                    foreach (var data in dynObj)
                    {
                        if (data.donation_status == "requesting")
                            _pendingRequestsList_Private.Add(new PendingRequests { donation_id = Convert.ToString(data.donation_id), requester_id = Convert.ToString(data.requester_id), requester_name = Convert.ToString(data.donation_area_code), requester_country = Convert.ToString(data.donation_country), requester_state = Convert.ToString(data.donation_state), requester_area_code = Convert.ToString(data.donation_area_code), requester_phone_number = Convert.ToString(data.donation_phone) });

                    }

                }
                catch (Exception BadUserID)
                {
                    await DisplayAlert("Bad Request", "Wrong User ID", "Ok");
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Failed", "Could Not Connect To Servers.", "Ok");
            }

        }

        public async Task RequestAccept(string donation_id)
        {
            try
            {
                var data_to_send = new FormUrlEncodedContent(new[]
                        {
                        new KeyValuePair<string, string>("donation_donor_id", user_id_main),
                        new KeyValuePair<string, string>("donation_id", donation_id)
                    });

                var content = await _client.PostAsync(primaryDomain_main + "/api/data_operations/data_insert_blood_donation.php", data_to_send);

                var temp_response = await content.Content.ReadAsStringAsync();
                //await DisplayAlert("Success!", temp_response.ToString(), "Ok");

                try
                {
                    dynamic dynObj = JsonConvert.DeserializeObject("[" + temp_response + "]");
                    _pendingRequestsList_Private.Clear();

                    foreach (var data in dynObj)
                    {
                        if (data.message == "updated 1 of 1 records")
                            await DisplayAlert("Success!", "Successfully Accepted The Request", "Ok");

                    }

                }
                catch (Exception BadUserID)
                {
                    await DisplayAlert("lel", BadUserID.ToString(), "Ok");
                    await DisplayAlert("Bad Request", "Wrong User ID", "Ok");
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Failed", "Could Not Connect To Servers.", "Ok");
            }

        }
    }

}