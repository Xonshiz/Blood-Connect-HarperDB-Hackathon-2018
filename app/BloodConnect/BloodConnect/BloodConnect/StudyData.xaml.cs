using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microcharts.Forms;
using Entry = Microcharts.Entry;
using SkiaSharp;
using Microcharts;

namespace BloodConnect
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StudyData : ContentPage
	{
        private string primaryDomain_main;
        private int blood_A, blood_B, blood_AB, blood_O, blood_total;
        private HttpClient _client = new HttpClient();

        public StudyData ()
		{
			InitializeComponent();
            primaryDomain_main = MainPage.primaryDomain;
        }

        private void ChartEntries()
        {
            if (blood_total == 0)
            {
                if (chartView.IsVisible)
                {
                    chartView.IsVisible = false;
                    chartView.IsEnabled = false;
                }
                DisplayAlert("No Data", "Not Enough Data Available To Show Graphs", "Ok");
                return;
            }

            else
            {
                if (!chartView.IsVisible || !chartView.IsEnabled)
                {
                    chartView.IsVisible = true;
                    chartView.IsEnabled = true;
                }

                var entries = new[]
                {
                new Entry(blood_A)
                {
                    Label = "A",
                    ValueLabel = Convert.ToString(blood_A),
                    Color = SKColor.Parse("#266489")
                },
                new Entry(blood_B)
                {
                    Label = "A",
                    ValueLabel = Convert.ToString(blood_B),
                    Color = SKColor.Parse("#68B9C0")
                },
                new Entry(blood_AB)
                {
                    Label = "AB",
                    ValueLabel = Convert.ToString(blood_AB),
                    Color = SKColor.Parse("#90D585")
                },
                new Entry(blood_O)
                {
                    Label = "O",
                    ValueLabel = Convert.ToString(blood_O),
                    Color = SKColor.Parse("#90D585")
                }
            };

                var chart = new RadarChart() { Entries = entries };
                chartView.Chart = chart;
            }
        }

        private async void dataPicker_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            //DisplayAlert("Item Index", dataPicker.SelectedIndex.ToString(), "OK");

            if (dataPicker != null && dataPicker.SelectedIndex <= dataPicker.Items.Count)
            {
                if (dataPicker.SelectedIndex == 0)
                    return;

                if (dataPicker.SelectedIndex == 1)
                {
                    if (countryPicker.Items.Count > 0)
                    {
                        countryPicker.Items.Clear();
                        countryPicker.Items.Add("Select An Option");
                        countryPicker.Items.Add("India");
                        countryPicker.Items.Add("United States Of America");
                    }
                    else
                    {
                        countryPicker.Items.Add("Select An Option");
                        countryPicker.Items.Add("India");
                        countryPicker.Items.Add("United States Of America");
                    }
                    countryPicker.IsEnabled = true;
                    countryPicker.SelectedIndex = 0;
                }
                else if (dataPicker.SelectedIndex == 2)
                {
                    if (countryPicker.Items.Count > 0)
                    {
                        countryPicker.Items.Clear();
                        countryPicker.Items.Add("Select An Option");
                        countryPicker.Items.Add("New Delhi, India");
                        countryPicker.Items.Add("Punjab, India");
                        countryPicker.Items.Add("New York, USA");
                        countryPicker.Items.Add("California, USA");
                    }
                    else
                    {
                        countryPicker.Items.Add("Select An Option");
                        countryPicker.Items.Add("New Delhi, India");
                        countryPicker.Items.Add("Punjab, India");
                        countryPicker.Items.Add("New York, USA");
                        countryPicker.Items.Add("California, USA");
                    }
                    countryPicker.IsEnabled = true;
                    countryPicker.SelectedIndex = 0;
                }
                else if (dataPicker.SelectedIndex == 3)
                {
                    await DataFetcherWorld("/api/analytics/bloodTypes_In_World.php"); // We need to split on "," and send the first word, i.e., STATE
                    ChartEntries();
                }
            }
        }

        private async void countryPicker_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            /* What the actual Fuck?
             * Need to add some delay when this event is fired up. If not given, then system crashes with "System.NullReferenceException" Exception.
             */
            await Task.Delay(1000);
            // Fires event any time a country is selected
            if (countryPicker != null && countryPicker.SelectedIndex <= countryPicker.Items.Count)
            {
                if (countryPicker.SelectedIndex == 0)
                    return;

                if (dataPicker.SelectedIndex == 1)
                {
                    var selecteditem_countryPicker = countryPicker.SelectedItem.ToString();
                    await DataFetcher("/api/analytics/bloodTypes_In_Country.php", "country_name", selecteditem_countryPicker);
                }
                else if (dataPicker.SelectedIndex == 2)
                {
                    var selecteditem_countryPicker = countryPicker.SelectedItem.ToString();
                    await DataFetcher("/api/analytics/bloodTypes_In_State.php", "state_name", selecteditem_countryPicker.Split(',')[0]); // We need to split on "," and send the first word, i.e., STATE
                }
                //else if (dataPicker.SelectedIndex == 3)
                //{
                //    var selecteditem_countryPicker = countryPicker.SelectedItem.ToString();
                //    await DataFetcherWorld("/api/analytics/bloodTypes_In_World.php"); // We need to split on "," and send the first word, i.e., STATE
                //}

                ChartEntries();

            }
            else
                return;
        }

        public async Task DataFetcher(string url, string type,  string value)
        {
            try
            {
                var data_to_send = new FormUrlEncodedContent(new[]
                        {
                        new KeyValuePair<string, string>(type, value)
                    });

                var content = await _client.PostAsync(primaryDomain_main + url, data_to_send);

                var temp_response = await content.Content.ReadAsStringAsync();
                //await DisplayAlert("Response", temp_response.ToString(), "Ok");

                if (temp_response == "[]")
                {
                    blood_A = 0;
                    blood_B = 0;
                    blood_AB = 0;
                    blood_O = 0;
                }

                else
                {
                    try
                    {
                        dynamic dynObj = JsonConvert.DeserializeObject(temp_response);

                        foreach (var data in dynObj)
                        {

                            if (data.donation_blood_type == "A")
                            {
                                blood_A = data.totalcount;
                            }
                            if (data.donation_blood_type == "B" || data.donation_blood_type == "B+")
                            {
                                blood_B = data.totalcount;
                            }
                            if (data.donation_blood_type == "AB")
                            {
                                blood_AB = data.totalcount;
                            }
                            if (data.donation_blood_type == "O")
                            {
                                blood_O = data.totalcount;
                            }

                        }

                    }
                    catch (Exception BadUserID)
                    {
                        //Console.WriteLine("BadUserID Selection : " + BadUserID);
                        //await DisplayAlert("Bad Request", BadUserID.ToString(), "Ok");
                        await DisplayAlert("Bad Request", "Wrong User ID", "Ok");
                    }
                }

                blood_total = blood_A + blood_B + blood_AB + blood_O;
            }
            catch (Exception)
            {
                await DisplayAlert("Failed", "Could Not Connect To Servers.", "Ok");
            }

        }

        public async Task DataFetcherWorld(string url)
        {
            try
            {
                var content = await _client.GetAsync(primaryDomain_main + url);

                var temp_response = await content.Content.ReadAsStringAsync();
                //await DisplayAlert("Response", temp_response.ToString(), "Ok");

                if (temp_response == "[]")
                {
                    blood_A = 0;
                    blood_B = 0;
                    blood_AB = 0;
                    blood_O = 0;
                }

                else
                {
                    try
                    {
                        dynamic dynObj = JsonConvert.DeserializeObject(temp_response);

                        foreach (var data in dynObj)
                        {

                            if (data.donation_blood_type == "A")
                            {
                                blood_A = data.totalcount;
                            }
                            if (data.donation_blood_type == "B" || data.donation_blood_type == "B+")
                            {
                                blood_B = data.totalcount;
                            }
                            if (data.donation_blood_type == "AB")
                            {
                                blood_AB = data.totalcount;
                            }
                            if (data.donation_blood_type == "O")
                            {
                                blood_O = data.totalcount;
                            }

                        }

                    }
                    catch (Exception BadUserID)
                    {
                        //Console.WriteLine("BadUserID Selection : " + BadUserID);
                        //await DisplayAlert("Bad Request", BadUserID.ToString(), "Ok");
                        await DisplayAlert("Bad Request", "Wrong User ID", "Ok");
                    }
                }

                blood_total = blood_A + blood_B + blood_AB + blood_O;
            }
            catch (Exception)
            {
                await DisplayAlert("Failed", "Could Not Connect To Servers.", "Ok");
            }

        }
    }
}