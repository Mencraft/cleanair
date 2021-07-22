using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net.Http;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BotAppli.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var message = context.MakeMessage();
            var attachment = GetHeroCard();
            message.Attachments.Add(attachment);
            await context.PostAsync(message);
            context.Wait(this.ShowAnnuvalConferenceTicket);
        }

        private  Attachment GetHeroCard()
        {
            var heroCard = new HeroCard
            {
                Title = "MDX Air quality ChatBot",
                Subtitle = "Created by ..",
                Text = "Mdx Bsc Buisness and information technology final IS Project",
                Images = new List<CardImage> {new CardImage("https://ec.europa.eu/malta/sites/malta/files/clean-air-logos.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "About Us", value: "http://damclick.com/") }
            };

            return heroCard.ToAttachment();
        }

        public enum AnnuvalConferencePass
        {
            City,
            Country,
            Location
        }

        public virtual async Task ShowAnnuvalConferenceTicket(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            var message = await activity;

            PromptDialog.Choice(
                context:context,
                resume: ChoiceReceivedAsync,
                options: (IEnumerable<AnnuvalConferencePass>)Enum.GetValues(typeof(AnnuvalConferencePass)),
                prompt: "Hi. Please Select Air quality search parameters Option",
                retry: "Selected plan not availabel at this time. Please try again",
                promptStyle: PromptStyle.Auto
                );
        }

        public virtual async Task ChoiceReceivedAsync(IDialogContext context, IAwaitable<AnnuvalConferencePass> activity)
        {
            AnnuvalConferencePass response = await activity;
            context.Call<Object>
                (new AnnualPlanDialog(response.ToString()), ChildDialogComplete);
        }
  
        public virtual async Task ChildDialogComplete(IDialogContext context, IAwaitable<object> response)
        {
            await context.PostAsync("Thanks for choosing the Air quality bot");
            context.Done(this);
        }


}

   

