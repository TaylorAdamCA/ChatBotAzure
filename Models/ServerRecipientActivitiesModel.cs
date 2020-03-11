using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptUsersForInput.Models
{
    public class ServerRecipientActivitiesModel
    {
        public AssessmentModel Assessment { get; private set; } = new AssessmentModel();
        public TherapeuticInterventionModel TherapeuticIntervention { get; private set; } = new TherapeuticInterventionModel();
    }
}