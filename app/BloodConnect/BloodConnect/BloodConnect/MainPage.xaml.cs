using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BloodConnect
{
	public partial class MainPage : CarouselPage
	{
        private const string primary_url  = "https://xonshiz.heliohost.org/test_sample/bloodConnect.json";
        public static string primaryDomain;
        public static string user_blood_type, user_name, user_id, user_state, user_age, user_country, user_area_code, user_phone, user_email;
        private HttpClient _client = new HttpClient();

        public MainPage()
		{
			InitializeComponent();
            if (Application.Current.Properties.ContainsKey("primaryDomain"))
                primaryDomain = Application.Current.Properties["primaryDomain"].ToString();

            userEmail_Login.Text = "kanojia24.10@gmail.com";
            userPassword_Login.Text = "testing";

        }

        protected override async void OnAppearing()
        {
            try
            {
                //await DisplayAlert("Login Clicked", "You Clicked Log in", "Ok");
                var content = await _client.GetStringAsync(primary_url);

                dynamic json_object = Newtonsoft.Json.Linq.JObject.Parse(content);
                //Console.WriteLine("Domain  : " + json_object.domain);
                //await DisplayAlert("Login Clicked", "You Clicked Log in", "Ok");

                //Save the primaryDomain as the settings, so that we can use it in the application later on.
                Application.Current.Properties["primaryDomain"] = Convert.ToString(json_object.domain);
                await Application.Current.SavePropertiesAsync();
            }
            catch (Exception)
            {
                await DisplayAlert("Failed", "Could Not Connect To Servers.", "Ok");
            }
        }

        private async void loginButton_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new MainScreen());
            if (string.IsNullOrEmpty(primaryDomain))
            {
                await DisplayAlert("Wait...", "Connecitng To Servers.", "Ok");
            }
            else
            {
                //If both the fields aren't empty, perform login.
                if (!string.IsNullOrEmpty(userEmail_Login.Text) && !string.IsNullOrEmpty(userPassword_Login.Text))
                {
                    
                    var data_to_send = new FormUrlEncodedContent(new[]
                        {
                        new KeyValuePair<string, string>("user_email", userEmail_Login.Text),
                        new KeyValuePair<string, string>("user_password", userPassword_Login.Text)
                    });
                    var content = await _client.PostAsync(primaryDomain + "/api/data_operations/user_login.php", data_to_send);
                    
                    var temp_response = await content.Content.ReadAsStringAsync();

                    try
                    {
                        dynamic dynObj = JsonConvert.DeserializeObject(temp_response);
                        foreach (var data in dynObj)
                        {
                            user_blood_type = data.blood_type;
                            user_name = data.name;
                            user_id = data.user_id;
                            user_state = data.state;
                            user_age = data.age;
                            user_country = data.country;
                            user_area_code = data.area_code;
                            user_phone = data.phone;
                            user_email = data.email;
                        }

                        await Navigation.PushAsync(new MainScreen());
                    }
                    catch (Exception BadLogin)
                    {
                        await DisplayAlert("Bad Login", "Please Check Your Credentials Again", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Empty Fields", "Make Sure The Fields Are Filled.", "Ok");
                }
            }

        }

        private async void registerButton_Clicked(object sender, EventArgs e)
        {
            //DisplayAlert("Register Clicked", "You Clicked Regsiter", "Ok");
            if (string.IsNullOrEmpty(primaryDomain))
            {
                await DisplayAlert("Wait...", "Connecitng To Servers.", "Ok");
            }
            else
            {
                //If both the fields aren't empty, perform login.
                if (!string.IsNullOrEmpty(name_register.Text) && !string.IsNullOrEmpty(age_register.Text) && !string.IsNullOrEmpty(areaCode_register.Text)
                     && !string.IsNullOrEmpty(email_register.Text) && !string.IsNullOrEmpty(phone_register.Text)&& !string.IsNullOrEmpty(password_register.Text)
                     && country_register.SelectedIndex != 0 && state_register.SelectedIndex != 0 && bloodType_register.SelectedIndex != 0)
                {

                    var data_to_send = new FormUrlEncodedContent(new[]
                        {
                        new KeyValuePair<string, string>("user_name", name_register.Text),
                        new KeyValuePair<string, string>("user_age", age_register.Text),
                        new KeyValuePair<string, string>("user_area_code", areaCode_register.Text),
                        new KeyValuePair<string, string>("user_country", Convert.ToString(country_register.SelectedItem.ToString())),
                        new KeyValuePair<string, string>("user_state", Convert.ToString(state_register.SelectedItem.ToString())),
                        new KeyValuePair<string, string>("user_blood_type", Convert.ToString(bloodType_register.SelectedItem.ToString())),
                        new KeyValuePair<string, string>("user_email", email_register.Text),
                        new KeyValuePair<string, string>("user_phone", phone_register.Text),
                        new KeyValuePair<string, string>("user_password", password_register.Text)
                    });
                    var content = await _client.PostAsync(primaryDomain + "/api/data_operations/data_insert_user_table.php", data_to_send);

                    var temp_response = await content.Content.ReadAsStringAsync();

                    try
                    {
                        dynamic dynObj = JsonConvert.DeserializeObject("[" + temp_response + "]");
                        foreach (var data in dynObj)
                        {
                            if (data.message == "inserted 1 records")
                            {
                                await DisplayAlert("Registered", "Successfuly Registered, You can Log In Now.", "Ok");
                                name_register.Text = "";
                                age_register.Text = "";
                                areaCode_register.Text = "";
                                email_register.Text = "";
                                phone_register.Text = "";
                                password_register.Text = "";
                                country_register.SelectedIndex = 0;
                                state_register.SelectedIndex = 0;
                                bloodType_register.SelectedIndex = 0;
                            }
                            else
                                await DisplayAlert("Failed", "Couldn't Register. Try Again Later.", "Ok");

                        }

                        //await Navigation.PushAsync(new MainScreen());
                        //base.OnBackButtonPressed();
                    }
                    catch (Exception BadLogin)
                    {
                        await DisplayAlert("Bad Login", "Please Check Your Credentials Again", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Empty Fields", "Make Sure The Fields Are Filled.", "Ok");
                }
            }
        }

        private void countryPicker_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            // Fires event any time a country is selected
            if (country_register != null && country_register.SelectedIndex <= country_register.Items.Count)
            {
                if (country_register.SelectedIndex == 0)
                    return;

                if (country_register.SelectedIndex == 1)
                {
                    //var selecteditem_countryPicker = country_register.Items[country_register.SelectedIndex];
                    state_register.Items.Clear();
                    state_register.Items.Add("Select Your State");
                    state_register.Items.Add("New Delhi");
                    state_register.Items.Add("Punjab");
                }
                else if (country_register.SelectedIndex == 2)
                {
                    //var selecteditem_countryPicker = country_register.Items[country_register.SelectedIndex];
                    state_register.Items.Clear();
                    state_register.Items.Add("Select Your State");
                    state_register.Items.Add("New York");
                    state_register.Items.Add("California");
                }

                state_register.IsEnabled = true;
                state_register.SelectedIndex = 0;

            }
        }

    }
}
