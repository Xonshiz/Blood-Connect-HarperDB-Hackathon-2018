using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BloodConnect
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewRequest : ContentPage
	{
        private string primaryDomain_main, user_blood_type_main, user_name_main, user_id_main, user_state_main, user_age_main, user_country_main, user_area_code_main, user_phone_main, user_email_main;
        private HttpClient _client = new HttpClient();

        public NewRequest ()
		{
			InitializeComponent ();
            primaryDomain_main = MainPage.primaryDomain;
            user_id_main = Convert.ToString(MainPage.user_id);
            donation_BloodType.Text = Convert.ToString(MainPage.user_blood_type);
            //donation_Country.Text = Convert.ToString(MainPage.user_country);
            //donation_State.Text = Convert.ToString(MainPage.user_state);
            donation_AreaCode.Text = Convert.ToString(MainPage.user_area_code);
            donation_PhoneNumber.Text = Convert.ToString(MainPage.user_phone);

        }

        private async void submit_Clicked(object sender, EventArgs e)
        {
            //DisplayAlert("Register Clicked", "You Clicked Regsiter", "Ok");
            await DataPoster();
            await Navigation.PopModalAsync();
        }

        private void dataPicker_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            //DisplayAlert("Item Index", dataPicker.SelectedIndex.ToString(), "OK");

            if (donation_Country != null && donation_Country.SelectedIndex <= donation_Country.Items.Count)
            {
                if (donation_Country.SelectedIndex == 0)
                    return;

                if (donation_Country.SelectedIndex == 1)
                {
                    //var selecteditem_countryPicker = country_register.Items[country_register.SelectedIndex];
                    donation_State.Items.Clear();
                    donation_State.Items.Add("Select Your State");
                    donation_State.Items.Add("New Delhi");
                    donation_State.Items.Add("Punjab");
                }
                else if (donation_Country.SelectedIndex == 2)
                {
                    //var selecteditem_countryPicker = country_register.Items[country_register.SelectedIndex];
                    donation_State.Items.Clear();
                    donation_State.Items.Add("Select Your State");
                    donation_State.Items.Add("New York");
                    donation_State.Items.Add("California");
                }

                donation_State.IsEnabled = true;
                donation_State.SelectedIndex = 0;
            }
        }

        public async Task DataPoster()
        {
            try
            {
                var data_to_send = new FormUrlEncodedContent(new[]
                        {
                        new KeyValuePair<string, string>("donation_requester_id", user_id_main),
                        new KeyValuePair<string, string>("donation_blood_type", donation_BloodType.Text),
                        new KeyValuePair<string, string>("donation_country", donation_Country.SelectedItem.ToString()),
                        new KeyValuePair<string, string>("donation_state", donation_State.SelectedItem.ToString()),
                        new KeyValuePair<string, string>("donation_area_code", donation_AreaCode.Text),
                        new KeyValuePair<string, string>("donation_phone", donation_PhoneNumber.Text)
                    });

                var content = await _client.PostAsync(primaryDomain_main + "/api/data_operations/blood_request.php", data_to_send);

                Console.WriteLine("Link Main : " + primaryDomain_main + "/api/data_operations/blood_request.php");

                var temp_response = await content.Content.ReadAsStringAsync();
                Console.WriteLine("temp_response : " + temp_response);

                try
                {
                    // HarperDB is inconsistent, it's not giving the Object while inserting this. Weird shit.
                    // Workaround is that we enclose the returnded JSON within [].
                    dynamic dynObj = JsonConvert.DeserializeObject("[" + temp_response + "]");
                    
                    foreach (var data in dynObj)
                    {
                        if (data.message == "inserted 1 records")
                            await DisplayAlert("Successful", "Requested!", "Ok");
                        else
                            await DisplayAlert("Failed", "Some Failure Occurred. Try again later.", "Ok");
                    }

                }
                catch (Exception BadUserID)
                {
                    Console.WriteLine("Fucked It Up : " + BadUserID);
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