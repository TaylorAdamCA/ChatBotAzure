using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace PromptUsersForInput.Helpers
{
    public static class SuggestionActions
    {
        public static async Task SendIntSuggestionsActionsAsync(ITurnContext turnContext,
            CancellationToken cancellationToken, string message, List<int> choices, bool yesOrNo = false)
        {
            var reply = MessageFactory.Text(message);
            var actions = new List<CardAction>();

            foreach (var choice in choices)
            {
                if (!yesOrNo)
                    actions.Add(new CardAction()
                    { Title = choice.ToString(), Type = ActionTypes.ImBack, Value = choice.ToString() });
                else
                {
                    var choiceText = choice == 0 ? "No" : "Yes";
                    actions.Add(new CardAction() { Title = choiceText, Type = ActionTypes.ImBack, Value = choiceText });
                }
            }

            reply.SuggestedActions = new Microsoft.Bot.Schema.SuggestedActions()
            {
                Actions = actions
            };
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}