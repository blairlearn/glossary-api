using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.BestBets.Tests.ESAutosuggestQueryTestData
{
    /// <summary>
    /// Expected to match test data from
    /// TestData/ESAutosuggestQuery/contain_cutaneo_cancer.gov_es_patient.json
    /// </summary>
    public class AutosuggestScenario_ContainCutaneoSpanishPatient : BaseAutosuggestTestData
    {
        public override string filename => "contain_cutaneo_cancer.gov_es_patient.json";

        public override bool BeginsWith => false;

        public override string DictionaryName => "Cancer.gov";

        public override string Language => "es";

        public override AudienceType Audience => AudienceType.Patient;

        public override Suggestion[] ExpectedData => new Suggestion[]
        {
            new Suggestion()
            {
                TermName = "cáncer de mama cutáneo",
                TermId = 45018
            },
            new Suggestion()
            {
                TermName = "carcinoma de células escamosas cutáneo",
                TermId = 795104
            },
            new Suggestion()
            {
                TermName = "leiomioma cutáneo",
                TermId = 765494
            },
            new Suggestion()
            {
                TermName = "linfoma cutáneo de células T",
                TermId = 46771
            },
            new Suggestion()
            {
                TermName = "linfoma cutáneo de células T en estadio I",
                TermId = 46773
            },
            new Suggestion()
            {
                TermName = "linfoma cutáneo de células T en estadio II",
                TermId = 43966
            },
            new Suggestion()
            {
                TermName = "linfoma cutáneo de células T en estadio III",
                TermId = 44848
            },
            new Suggestion()
            {
                TermName = "linfoma cutáneo de células T en estadio IV",
                TermId = 44874
            }
        };
    }
}