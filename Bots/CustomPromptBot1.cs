// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.Number;
using PromptUsersForInput.Flows;
using PromptUsersForInput.Helpers;
using PromptUsersForInput.Models;

namespace PromptUsersForInput.Bots
{
    // This IBot implementation can run any type of Dialog. The use of type parameterization is to allows multiple different bots
    // to be run at different endpoints within the same project. This can be achieved by defining distinct Controller types
    // each with dependency on distinct IBot types, this way ASP Dependency Injection can glue everything together without ambiguity.
    public class CustomPromptBot1 : ActivityHandler
    {
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            // Send a welcome message to the user and tell them what actions they may perform to use this bot
            await turnContext.SendActivityAsync("Welcome to the chat bot!");
        }

        private readonly BotState _userState;
        private readonly BotState _conversationState;

        public CustomPromptBot1(ConversationState conversationState, UserState userState)
        {
            _conversationState = conversationState;
            _userState = userState;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var conversationStateAccessors =
                _conversationState.CreateProperty<ServerRecipientActivitiesFlow>(nameof(ServerRecipientActivitiesFlow));
            var flow = await conversationStateAccessors.GetAsync(turnContext,
                () => new ServerRecipientActivitiesFlow());

            var userStateAccessors =
                _userState.CreateProperty<ServerRecipientActivitiesModel>(nameof(ServerRecipientActivitiesModel));
            var serverRecipientActivities =
                await userStateAccessors.GetAsync(turnContext, () => new ServerRecipientActivitiesModel());

            await FillOutServerRecipientActivities(flow, serverRecipientActivities, turnContext);

            // Save changes.
            await _conversationState.SaveChangesAsync(turnContext);
            await _userState.SaveChangesAsync(turnContext);
        }

        private static async Task FillOutServerRecipientActivities(ServerRecipientActivitiesFlow flow,
            ServerRecipientActivitiesModel serverRecipientActivities,
            ITurnContext turnContext)
        {
            string input = turnContext.Activity.Text?.Trim();
            string message;

            switch (flow.LastQuestionAsked)
            {
                case ServerRecipientActivitiesFlow.Question.None:
                    await SuggestionActions.SendIntSuggestionsActionsAsync(turnContext, CancellationToken.None,
                        "ASSESSMENT \n" +
                        "Non Face To Face Assessment / Develop Plan", new List<int>() { 0, 1 });
                    flow.LastQuestionAsked = ServerRecipientActivitiesFlow.Question.NonFTFAssessment;
                    break;

                case ServerRecipientActivitiesFlow.Question.NonFTFAssessment:
                    if (ValidateResponse(input, out int NonFTFResponse, out message, 0, 1))
                    {
                        serverRecipientActivities.Assessment.NonFTFAssessment = NonFTFResponse;
                        await SuggestionActions.SendIntSuggestionsActionsAsync(turnContext, CancellationToken.None,
                            "Face To Face Assessment", new List<int>() { 0, 1 });
                        flow.LastQuestionAsked = ServerRecipientActivitiesFlow.Question.FTFAssessment;
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.");
                        break;
                    }

                case ServerRecipientActivitiesFlow.Question.FTFAssessment:
                    if (ValidateResponse(input, out int FTFResponse, out message, 0, 1))
                    {
                        serverRecipientActivities.Assessment.FTFAssessment = FTFResponse;
                        await SuggestionActions.SendIntSuggestionsActionsAsync(turnContext, CancellationToken.None,
                            "Telephone / Electronic Assessment", new List<int>() { 0, 1 });
                        flow.LastQuestionAsked = ServerRecipientActivitiesFlow.Question.TelephoneElectronicAssessment;
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.");
                        break;
                    }
                case ServerRecipientActivitiesFlow.Question.TelephoneElectronicAssessment:
                    if (ValidateResponse(input, out int telephoneElectronicResponse, out message, 0, 1))
                    {
                        serverRecipientActivities.Assessment.TelephoneElectronicAssessment =
                            telephoneElectronicResponse;
                        await SuggestionActions.SendIntSuggestionsActionsAsync(turnContext, CancellationToken.None,
                            "Telephone / Electronic Implement / Evaluate Plan", new List<int>() { 0, 1 });
                        flow.LastQuestionAsked = ServerRecipientActivitiesFlow.Question.TelephoneElectronicEvaluate;
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.");
                        break;
                    }
                case ServerRecipientActivitiesFlow.Question.TelephoneElectronicEvaluate:
                    if (ValidateResponse(input, out int telephoneElectronicEvaluateResponse, out message, 0, 1))
                    {
                        serverRecipientActivities.Assessment.TelephoneElectronicPlan =
                            telephoneElectronicEvaluateResponse;
                        await SuggestionActions.SendIntSuggestionsActionsAsync(turnContext, CancellationToken.None,
                            "THERAPEUTIC INTERVENTION \n" +
                            "Group Patient Teaching: 60 Minutes", new List<int>() { 1, 60 });
                        flow.LastQuestionAsked = ServerRecipientActivitiesFlow.Question.GroupPatientTeaching60;
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.");
                        break;
                    }
                case ServerRecipientActivitiesFlow.Question.GroupPatientTeaching60:
                    if (ValidateResponse(input, out int groupPatientTeaching60Response, out message, 0, 60))
                    {
                        serverRecipientActivities.TherapeuticIntervention.GroupPatientTeaching60Minutes =
                            groupPatientTeaching60Response;
                        await SuggestionActions.SendIntSuggestionsActionsAsync(turnContext, CancellationToken.None,
                            "Group Patient Teaching: 69 Minutes", new List<int>() { 1, 90 });
                        flow.LastQuestionAsked = ServerRecipientActivitiesFlow.Question.GroupPatientTeaching90;
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.");
                        break;
                    }

                case ServerRecipientActivitiesFlow.Question.GroupPatientTeaching90:
                    if (ValidateResponse(input, out int groupPatientTeaching90Response, out message, 0, 90))
                    {
                        serverRecipientActivities.TherapeuticIntervention.GroupPatientTeaching90Minutes =
                            groupPatientTeaching90Response;
                        await turnContext.SendActivityAsync(
                            $"Non Face To Face Assessment / Develop Plan {serverRecipientActivities.Assessment.NonFTFAssessment}.");
                        await turnContext.SendActivityAsync(
                            $"Face To Face Assessment {serverRecipientActivities.Assessment.FTFAssessment}.");
                        await turnContext.SendActivityAsync(
                            $"Telephone / Electronic Assessment {serverRecipientActivities.Assessment.TelephoneElectronicAssessment}.");
                        await turnContext.SendActivityAsync(
                            $"Telephone / Electronic Implement / Evaluate Plan {serverRecipientActivities.Assessment.TelephoneElectronicPlan}.");
                        await turnContext.SendActivityAsync(
                            $"Group Patient Teaching: 60 Minutes {serverRecipientActivities.TherapeuticIntervention.GroupPatientTeaching60Minutes}.");
                        await turnContext.SendActivityAsync(
                            $"Group Patient Teaching: 90 Minutes {serverRecipientActivities.TherapeuticIntervention.GroupPatientTeaching90Minutes}.");
                        await turnContext.SendActivityAsync($"Thank you for filling in this form");
                        // Probably post the result to our API
                        flow.LastQuestionAsked = ServerRecipientActivitiesFlow.Question.Done;
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.");
                        break;
                    }
                case ServerRecipientActivitiesFlow.Question.Done:
                    //TODO: find some way to end the conversation
                    flow.LastQuestionAsked = ServerRecipientActivitiesFlow.Question.None;
                    serverRecipientActivities = new ServerRecipientActivitiesModel();
                    break;
            }
        }

        private static bool ValidateResponse(string input, out int response, out string message,
            int lowerBoundInclusive, int upperBoundInclusive)
        {
            response = 0;
            message = null;

            // Try to recognize the input as a number. This works for responses such as "twelve" as well as "12".
            try
            {
                // Attempt to convert the Recognizer result to an integer. This works for "a dozen", "twelve", "12", and so on.
                // The recognizer returns a list of potential recognition results, if any.

                var results = NumberRecognizer.RecognizeNumber(input, Culture.English);

                foreach (var result in results)
                {
                    // The result resolution is a dictionary, where the "value" entry contains the processed string.
                    if (result.Resolution.TryGetValue("value", out object value))
                    {
                        response = Convert.ToInt32(value);
                        if (response >= lowerBoundInclusive && response <= upperBoundInclusive)
                        {
                            return true;
                        }
                    }
                }

                message = "Please enter a number between " + lowerBoundInclusive + " and " + upperBoundInclusive + ".";
            }
            catch
            {
                message = "I'm sorry, I could not interpret that as a number. Please enter a number between " +
                          lowerBoundInclusive + " and " + upperBoundInclusive + ".";
            }

            return false;
        }
    }
}