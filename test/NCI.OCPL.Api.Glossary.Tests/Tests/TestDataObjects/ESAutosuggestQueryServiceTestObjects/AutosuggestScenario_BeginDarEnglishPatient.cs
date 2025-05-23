using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData
{
    /// <summary>
    /// Expected to match test data from
    /// TestData/ESAutosuggestQuery/begin_dar_cancer.gov_en_patient.json
    /// </summary>
    public class AutosuggestScenario_BeginDarEnglishPatient : BaseAutosuggestTestData
    {
        public override string TestFilename => "begin_dar_cancer.gov_en_patient.json";

        public override Suggestion[] ExpectedData => new Suggestion[]
        {
            new Suggestion()
            {
                TermId = 635454,
                TermName = "DAR"
            },
            new Suggestion()
            {
                TermId = 776992,
                TermName = "daratumumab"
            },
            new Suggestion()
            {
                TermId = 256550,
                TermName = "darbepoyetina alfa"
            },
            new Suggestion()
            {
                TermId = 798951,
                TermName = "darolutamida"
            },
            new Suggestion()
            {
                TermId = 776993,
                TermName = "Darzalex"
            }
        };
    }
}