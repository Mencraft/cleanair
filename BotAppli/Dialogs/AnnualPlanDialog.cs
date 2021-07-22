using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace BotAppli.Dialogs
{
    [Serializable]
    public class AnnualPlanDialog : IDialog<object>
    {
        string city;
        string country;
        string location;
        string plandetails;
      //  int vals;

        public AnnualPlanDialog(string plan)
        {
            plandetails = plan;
        }
        public async Task StartAsync(IDialogContext context)
        {
            

            switch (plandetails)
            {
                case "Country":
                    await context.PostAsync("Thanks for select " + plandetails + " , can i help you check air quality ? ");
                    context.Wait(MessageReceivedAsync);
                    break;
                case "City":
                    await context.PostAsync("Thanks for select " + plandetails + " , can i help you check air quality  ? ");
                    context.Wait(CityReceivedAsync);
                    break;
                case "Location":
                    await context.PostAsync("Thanks for select " + plandetails + " Plan, can i help you check air quality  ? ");
                    context.Wait(LocationReceivedAsync);
                    break;

            }

            
            }

        public async Task CityReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            var response = await activity;
            if (response.Text.ToLower().Contains("yes"))
            {

                PromptDialog.Text(
                    context: context,
                    resume: GetCity,
                    prompt: "Please Type name of city ",
                    retry: "Sorry, city does not exist."
                    );
            }
            else
            {
                context.Done(this);
            }
        }

        public async Task LocationReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            await context.PostAsync("Wowww you are finaly in the Location");
            var response = await activity;
            if (response.Text.ToLower().Contains("yes"))
            {

                PromptDialog.Text(
                    context: context,
                    resume: GeLocation,
                    prompt: "Please Type name of Location ",
                    retry: "Sorry, city does not exist."
                    );
            }
            else
            {
                context.Done(this);
            }
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            var response = await activity;
            if (response.Text.ToLower().Contains("yes"))
            {
                
                PromptDialog.Text(
                    context: context,
                    resume: GetCountry,
                    prompt: "Please Type name of country",
                    retry: "Sorry, i didnt understand that ."
                    );
            }
            else
            {
                context.Done(this);
            }
        }

        public async  Task GetCity(IDialogContext context, IAwaitable<string> cityname)
        {
            string response = await cityname;
            city = response;

            try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string RequestURI = "https://api.openaq.org/v1/measurements?city=" + city;
                        HttpResponseMessage responseMsg = await client.GetAsync(RequestURI);

                        if (responseMsg.IsSuccessStatusCode)
                        {
                            var apiResponse = await responseMsg.Content.ReadAsStringAsync();
                            var test = JObject.Parse(apiResponse)["results"][1];
                            await context.PostAsync($"Current Air quality of {test["city"]}  is {test["parameter"]}");
                            JToken token = test["value"];

                            int vals = (int)token;

                            if (vals == 0 || vals <= 50)
                            {
                                await context.PostAsync("Air quality is considered satisfactory and air pollution pose little or no risk");

                            }
                            else if (vals == 51 || vals <= 100)
                            {
                                await context.PostAsync("Air quality is acceptable: however for some pollutants there may be a moderate health concern for a very small number of people who are unusually sensitive to air pollution");
                            }
                            else if (vals == 101 || vals <= 150)
                            {
                                await context.PostAsync("Members of sensitive groups may experience health effects. The general public is not likely to be affected.");
                            }
                            else if (vals == 151 || vals <= 200)
                            {
                                await context.PostAsync("Everyone may begin to experience health effects: members of sensitivie groups may experience more serious health effects");
                            }
                            else if (vals == 201 || vals <= 300)
                            {
                                await context.PostAsync("Health alert: everyone may experience more serious health effects");
                            }
                            else
                            {
                                await context.PostAsync("Health warnings of emergency conditions. The entire population is more likely to be affected");
                            }
                        context.Done(this);
                    }
                        else
                        {
                            await context.PostAsync("Location name is incorrect.  Please input correct Location name");
                        }

                    }
                }
                catch (Exception ex)
                {
                    await context.PostAsync($"error dey ohhh {ex}");
                }
            
        }

        public async Task GetCountry(IDialogContext context, IAwaitable<string> countryname)
        {
            string response = await countryname;
            country = response;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    await context.PostAsync($"Current Air quality of {country}");
                    string RequestURI = "https://api.openaq.org/v1/measurements?country=" + country;
                    HttpResponseMessage responseMsg = await client.GetAsync(RequestURI);

                    if (responseMsg.IsSuccessStatusCode)
                    {
                        var apiResponse = await responseMsg.Content.ReadAsStringAsync();
                        var test = JObject.Parse(apiResponse)["results"][1];
                        await context.PostAsync($"Current Air quality of {test["country"]}  is {test["parameter"]}");

                        JToken token = test["value"];

                        int vals = (int)token;

                        if (vals == 0 || vals <= 50)
                        {
                            await context.PostAsync("Air quality is considered satisfactory and air pollution pose little or no risk");

                        }
                        else if (vals == 51 || vals <= 100)
                        {
                            await context.PostAsync("Air quality is acceptable: however for some pollutants there may be a moderate health concern for a very small number of people who are unusually sensitive to air pollution");
                        }
                        else if (vals == 101 || vals <= 150)
                        {
                            await context.PostAsync("Members of sensitive groups may experience health effects. The general public is not likely to be affected.");
                        }
                        else if (vals == 151 || vals <= 200)
                        {
                            await context.PostAsync("Everyone may begin to experience health effects: members of sensitivie groups may experience more serious health effects");
                        }
                        else if (vals == 201 || vals <= 300)
                        {
                            await context.PostAsync("Health alert: everyone may experience more serious health effects");
                        }
                        else
                        {
                            await context.PostAsync("Health warnings of emergency conditions. The entire population is more likely to be affected");
                        }
                        context.Done(this);
                    }
                    else
                    {
                        await context.PostAsync("Country name is incorrect. Please input correct Country name");
                    }

                }
            }
            catch (Exception ex)
            {
                await context.PostAsync($"error dey ohhh {ex}");
            }

        }

        public async Task GeLocation(IDialogContext context, IAwaitable<string> locationname)
        {
            string response = await locationname;
            location = response;

                        try
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                string RequestURI = "https://api.openaq.org/v1/measurements?location=" + location;
                                HttpResponseMessage responseMsg = await client.GetAsync(RequestURI);

                                if (responseMsg.IsSuccessStatusCode)
                                {
                                    var apiResponse = await responseMsg.Content.ReadAsStringAsync();
                                    var test = JObject.Parse(apiResponse)["results"][1];
                                    await context.PostAsync($"Current Air quality of {test["country"]}  is {test["parameter"]}");

                                    JToken token = test["value"];

                                    int vals = (int)token;

                                    if (vals == 0 || vals <= 50)
                                    {
                                        await context.PostAsync("Air quality is considered satisfactory and air pollution pose little or no risk");

                                    }
                                    else if (vals == 51 || vals <= 100)
                                    {
                                        await context.PostAsync("Air quality is acceptable: however for some pollutants there may be a moderate health concern for a very small number of people who are unusually sensitive to air pollution");
                                    }
                                    else if (vals == 101 || vals <= 150)
                                    {
                                        await context.PostAsync("Members of sensitive groups may experience health effects. The general public is not likely to be affected.");
                                    }
                                    else if (vals == 151 || vals <= 200)
                                    {
                                        await context.PostAsync("Everyone may begin to experience health effects: members of sensitivie groups may experience more serious health effects");
                                    }
                                    else if (vals == 201 || vals <= 300)
                                    {
                                        await context.PostAsync("Health alert: everyone may experience more serious health effects");
                                    }
                                    else
                                    {
                                        await context.PostAsync("Health warnings of emergency conditions. The entire population is more likely to be affected");
                                    }
                                    context.Done(this);
                                }
                                else
                                {
                                    await context.PostAsync("Location name is incorrect.  please input correct Location name");
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            await context.PostAsync($"error dey ohhh {ex}");
                        } 

        }

        public async Task AirQualityRemark(IDialogContext context, int vals)
        {

            if (vals == 0 || vals <= 50)
            {
                await context.PostAsync("Air quality is considered satisfactory and air pollution pose little or no risk");

            }
            else if(vals == 51 || vals <= 100)
            {
                await context.PostAsync("Air quality is acceptable: however for some pollutants there may be a moderate health concern for a very small number of people who are unusually sensitive to air pollution");
            }
            else if (vals == 101 || vals <= 150)
            {
                await context.PostAsync("Members of sensitive groups may experience health effects. The general public is not likely to be affected.");
            }
            else if (vals == 151 || vals <= 200)
            {
                await context.PostAsync("Everyone may begin to experience health effects: members of sensitivie groups may experience more serious health effects");
            }
            else if (vals == 201 || vals <= 300)
            {
                await context.PostAsync("Health alert: everyone may experience more serious health effects");
            }
            else 
            {
                await context.PostAsync("Health warnings of emergency conditions. The entire population is more likely to be affected");
            }
        }
        /*
        public virtual async Task ResumeGetName(IDialogContext context, IAwaitable<string> Username)
        {
            string response = await Username;
             name = response;

            PromptDialog.Text(
            context: context,
            resume: ResumeGetEmail,
            prompt: "Please share your Email iD",
            retry: "Sorry, i didnt understand that ."
            );

        }

        public virtual async Task ResumeGetEmail(IDialogContext context, IAwaitable<string> UserEmail)
        {
            string response = await UserEmail;
            email = response;

            PromptDialog.Text(
            context: context,
            resume: ResumeGetPhone,
            prompt: "Please share your Mobile number",
            retry: "Sorry, i didnt understand that ."
            );

        }

        public virtual async Task ResumeGetPhone(IDialogContext context, IAwaitable<string> mobile)
        {
            string response = await mobile;
            phone = response;

            await context.PostAsync(String.Format("Hello {O} , Congratulation :) your C# Corner Annual Conference 2018 Registration Successfully completed with Name = {0} Email = {1} Moobile Number {2} . you will get Confirmation email and SMS", name, email, phone));

            context.Done(this);

        }

        */

    }
}