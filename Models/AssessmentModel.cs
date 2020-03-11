using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptUsersForInput.Models
{
    public class AssessmentModel
    {
        public int NonFTFAssessment { get; set; }
        public int FTFAssessment { get; set; }
        public int TelephoneElectronicAssessment { get; set; }
        public int TelephoneElectronicPlan { get; set; }
    }
}