using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace PromptUsersForInput.Helpers
{
    public static class SuggestionActions
    {
        public static async Task SendIntSuggestionsActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken, string message, List<int> choices)
        {
            var reply = MessageFactory.Text(message);
            var actions = new List<CardAction>();

            foreach (var choice in choices)
            {
                actions.Add(new CardAction() { Title = choice.ToString(), Type = ActionTypes.ImBack, Value = choice.ToString() });
            }
            reply.SuggestedActions = new Microsoft.Bot.Schema.SuggestedActions()
            {
                Actions = actions
            };
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}