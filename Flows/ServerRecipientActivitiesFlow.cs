
namespace PromptUsersForInput.Flows
{
    public class ServerRecipientActivitiesFlow
    {
        public enum Question
        {
            NonFTFAssessment,
            FTFAssessment,
            TelephoneElectronicAssessment,
            TelephoneElectronicEvaluate,
            GroupPatientTeaching60,
            GroupPatientTeaching90,
            Done,
            None, // Our last action did not involve a question.
        }

        // The last question asked.
        public Question LastQuestionAsked { get; set; } = Question.None;
    }
}