using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.ConferenceBuddy.Common.Models;
using Microsoft.ConferenceBuddy.Common.Utilities;
using Microsoft.ConferenceBuddy.Core.Services;

namespace Microsoft.ConferenceBuddy.Bot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private enum BotTask
        {
            AskQuestion,
            AskWho,
            AskLearnMore
        }

        private static string[] DoNotUnderstandPreMsgs = new string[]
        {
            "Sorry, I don't think that I understand your question.",
            "Hummm, not sure that I understand your question."
        };

        private static string DoNotUnderstandMsg = "Can you rephrase it and try again?";

        private static string[] GreetingPreMsgs = new string[]
        {
            "How are you?",
            "How’s it going?"
        };

        public static string GreetingMsg = "I'm your buddy for today's conference. " +
            "I'm still in training but I'd love to help you learn more.";

        public static string GreetingActionsMsg =
            "Feel free to ask any questions like... \"Who is...?\" or \"I want to learn more about...\" " +
            "or even anything about today's session and I will help to redirect it to the speaker.";

        private static Random RandomNumGenerator = new Random((int)DateTime.UtcNow.Ticks);

        #region Services

        private static LUISService luis;

        protected static LUISService LUIS
        {
            get
            {
                if (luis == null)
                {
                    luis = new LUISService(
                        serviceUrl: LUISUrl,
                        appId: LUISAppId,
                        subscriptionKey: LUISSubscriptionKey,
                        spellcheckSubscriptionKey: LUISSpellcheckSubscriptionKey
                        );
                }
                return luis;
            }
        }

        #endregion

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var activity = await result as Activity;
                string message = WebUtility.HtmlDecode(activity.Text);

                if (string.IsNullOrEmpty(message) == true)
                {
                    return;
                }

                // Handle the explicit invocation case in Skype
                string channelId = GetChannelId(activity);

                if (channelId == "skype" && message.StartsWith(activity.Recipient.Name) == true)
                {
                    message = message.Substring(activity.Recipient.Name.Length).Trim();
                }
                else if (channelId == "skype" && message.StartsWith("@" + activity.Recipient.Name) == true)
                {
                    message = message.Substring(activity.Recipient.Name.Length + 1).Trim();
                }

                // Handle intents
                LUISResult luisResult = await LUIS.QueryAsync(message);
                string intent = luisResult.TopScoringIntent?.Intent;
                string[] entities = luisResult.Entities?.Select(e => e.entity)?.ToArray() ?? new string[0];

                if (intent == "greeting")
                {
                    await ProcessGreetingIntent(context, activity);
                }
                else if (intent == "who")
                {
                    await ProcessQueryIntent(context, activity, BotTask.AskWho, message, entities);
                }
                else if (intent == "learnmore")
                {
                    await ProcessQueryIntent(context, activity, BotTask.AskLearnMore, message, entities);
                }
                else
                {
                    await ProcessQueryIntent(context, activity, BotTask.AskQuestion, message, entities);
                }

                context.Wait(MessageReceivedAsync);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MessageReceivedAsync throws expcetion: " + ex);
            }
        }

        private static async Task<string> ProcessGreetingIntent(IDialogContext context, Activity activity)
        {
            await PostAsync(context, activity, GreetingPreMsg + " " +  GreetingActionsMsg);
            return "success";
        }

        private static async Task<string> ProcessQueryIntent(IDialogContext context, Activity activity, BotTask task, string query, string[] topics)
        {
            // Prepare the request to invoke a bot task within the bot brain
            AskQuestionRequest request = new AskQuestionRequest()
            {
                ConversationId = activity.Conversation.Id,
                Question = query,
                SessionId = DefaultSessionId,
                Topics = topics != null ? topics.ToArray() : new string[0],
                UserId = string.IsNullOrEmpty(activity.From.Name) == false ? activity.From.Name : activity.From.Id
            };

            // Invoke the bot task to process the request
            AskQuestionResponse askQuestionResponse =
                await HttpClientUtility.PostAsJsonAsync<AskQuestionResponse>(new Uri(BotBrainUrl + task.ToString()), RequestHeaders, request);

            // Handle the response returned from the bot task to be shown as cards depending on channel
            if (askQuestionResponse.Results?.Count() > 0 == true)
            {
                IMessageActivity foundMsg = context.MakeMessage();

                AskQuestionResult result = askQuestionResponse.Results[0];

                if (string.IsNullOrEmpty(result.Source) == false)
                {
                    foundMsg.Text = string.Format("Got it. Meanwhile, from {0}:", result.Source);
                }
                else
                {
                    foundMsg.Text = "Got it. Meanwhile, here's what I found:";
                }

                await context.PostAsync(foundMsg);

                IMessageActivity cardMessage;

                string channelId = GetChannelId(activity);

                if (channelId == "directline" || channelId == "emulator")
                {
                    cardMessage = GetAdaptiveCardMessage(context, request, result);
                }
                else
                {
                    cardMessage = GetHeroCardMessage(context, request, result);
                }

                await context.PostAsync(cardMessage);
            }
            else
            {
                await PostAsync(context, activity, DoNotUnderstandPreMsg + " " + DoNotUnderstandMsg);
            }

            return "success";
        }

        private static async Task PostAsync(IDialogContext context, Activity activity, string text)
        {
            if (string.IsNullOrEmpty(text) == true)
            {
                return;
            }

            // Create message
            IMessageActivity message = context.MakeMessage();

            message.Text = text;

            // Post message
            await context.PostAsync(message);
        }

        private static IMessageActivity GetAdaptiveCardMessage(IDialogContext context, AskQuestionRequest request, AskQuestionResult result)
        {
            // Create message
            IMessageActivity message = context.MakeMessage();

            // Create adaptive card
            AdaptiveCard card = new AdaptiveCard();

            if (string.IsNullOrEmpty(result.Title) == false)
            {
                card.Body.Add(new AdaptiveTextBlock()
                {
                    Text = result.Title,
                    Size = AdaptiveTextSize.Large,
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
                    IsSubtle = false,
                    Weight = AdaptiveTextWeight.Bolder,
                    Wrap = true
                });
            }

            if (string.IsNullOrEmpty(result.Answer) == false)
            {
                card.Body.Add(new AdaptiveTextBlock()
                {
                    Text = result.Answer,
                    Size = AdaptiveTextSize.Medium,
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
                    IsSubtle = false,
                    Wrap = true
                });
            }

            if (string.IsNullOrEmpty(result.ImageUrl) == false)
            {
                AdaptiveImage currentImage = new AdaptiveImage();
                currentImage.Url = new Uri(result.ImageUrl);
                currentImage.Size = AdaptiveImageSize.Stretch;
                card.Body.Add(currentImage);
            }

            if (string.IsNullOrEmpty(result.Url) == false)
            {
                card.Actions.Add(new AdaptiveOpenUrlAction()
                {
                    Title = string.IsNullOrEmpty(result.UrlDisplayName) == false ? result.UrlDisplayName : "Open Url...",
                    Url = new Uri(result.Url)

                });
            }

            // Attach card to message
            Attachment attachment = new Attachment()
            {
                Content = card,
                ContentType = "application/vnd.microsoft.card.adaptive"
            };
            message.Attachments.Add(attachment);

            return message;
        }

        private static IMessageActivity GetHeroCardMessage(IDialogContext context, AskQuestionRequest request, AskQuestionResult result)
        {
            // Create message
            IMessageActivity message = context.MakeMessage();

            // Create hero card
            HeroCard card = new HeroCard();

            if (string.IsNullOrEmpty(result.Title) == false)
            {
                card.Title = result.Title;
            }

            if (string.IsNullOrEmpty(result.Answer) == false)
            {
                card.Subtitle = result.Answer;
            }

            if (string.IsNullOrEmpty(result.ImageUrl) == false)
            {
                card.Images = new List<CardImage> { new CardImage(url: result.ImageUrl) };
            }

            if (string.IsNullOrEmpty(result.Url) == false)
            {
                card.Buttons = new List<CardAction> {
                            new CardAction()
                            {
                                Value = result.Url,
                                Type = "openUrl",
                                Title = string.IsNullOrEmpty(result.UrlDisplayName) == false ? result.UrlDisplayName : "Open Url"
                            }
                        };
            }

            // Attach card to message
            message.Attachments.Add(card.ToAttachment());

            return message;
        }

        private static string GetChannelId(Activity activity)
        {
            string channelId = string.IsNullOrEmpty(activity.ChannelId) == false ? activity.ChannelId.ToLower() : string.Empty;
            return channelId;
        }

        private static string DoNotUnderstandPreMsg
        {
            get
            {
                return DoNotUnderstandPreMsgs[RandomNumGenerator.Next(0, DoNotUnderstandPreMsgs.Length)];
            }
        }

        private static string GreetingPreMsg
        {
            get
            {
                return GreetingPreMsgs[RandomNumGenerator.Next(0, GreetingPreMsgs.Length)];
            }
        }
        
        private static string BotBrainUrl => ConfigurationManager.AppSettings["BotBrainUrl"]?.ToString();

        private static string BotBrainClientId => ConfigurationManager.AppSettings["BotBrainClientId"]?.ToString();

        private static string BotBrainAuthenticationKey => ConfigurationManager.AppSettings["BotBrainAuthenticationKey"]?.ToString();

        private static string DefaultSessionId => ConfigurationManager.AppSettings["DefaultSessionId"]?.ToString();

        private static string LUISUrl => ConfigurationManager.AppSettings["LUISUrl"]?.ToString();

        private static string LUISAppId => ConfigurationManager.AppSettings["LUISAppId"]?.ToString();

        private static string LUISSubscriptionKey => ConfigurationManager.AppSettings["LUISSubscriptionKey"]?.ToString();

        private static string LUISSpellcheckSubscriptionKey => ConfigurationManager.AppSettings["LUISSpellcheckSubscriptionKey"]?.ToString();

        private static IDictionary<string, string> RequestHeaders => new Dictionary<string, string>()
            {
                { "x-functions-clientid", BotBrainClientId },
                { "x-functions-key", BotBrainAuthenticationKey }
            };
    }
}