namespace PromptUsersForInput.Flows
{
    public class ConversationFlow
    {
        // Identifies the last question asked.
        public enum Question
        {
            ID,
            Time,
            Satisfaction,
            Functional,
            Admin,
            Setup,
            Quality,
            Travel,
            None, // Our last action did not involve a question.
        }

        // The last question asked.
        public Question LastQuestionAsked { get; set; } = Question.None;
    }
}