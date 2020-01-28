using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData
{
    /// <summary>
    /// Expected to match test data from
    /// TestData/ESAutosuggestQuery/begin_aci_cancer.gov_es_patient.json
    /// </summary>
    public class AutosuggestScenario_BeginAciSpanishPatient : BaseAutosuggestTestData
    {
        public override string TestFilename => "begin_aci_cancer.gov_es_patient.json";

        public override Suggestion[] ExpectedData => new Suggestion[]
        {
            new Suggestion()
            {
                TermId = 45167,
                TermName = "aciclovir"
            },
            new Suggestion()
            {
                TermId = 642991,
                TermName = "acidez"
            },
            new Suggestion()
            {
                TermId = 642989,
                TermName = "acidificación"
            },
            new Suggestion()
            {
                TermId = 642987,
                TermName = "ácido"
            },
            new Suggestion()
            {
                TermId = 44314,
                TermName ="ácido 5-hidroxiindolacético"
            },
            new Suggestion()
            {
                TermId = 655022,
                TermName = "ácido acético"
            },
            new Suggestion()
            {
                TermId = 46184,
                TermName = "ácido acético con dextrometorfano"
            },
            new Suggestion()
            {
                TermId = 46166,
                TermName = "ácido acético de dimetilxantenona"
            },
            new Suggestion()
            {
                TermId = 644311,
                TermName = "ácido acético gosipol"
            },
            new Suggestion()
            {
                TermId = 443026,
                TermName = "ácido alfalipoico"
            }
        };

    }
}