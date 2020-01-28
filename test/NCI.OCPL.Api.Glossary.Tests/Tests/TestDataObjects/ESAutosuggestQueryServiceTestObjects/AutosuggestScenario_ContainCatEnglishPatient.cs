using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData
{
    /// <summary>
    /// Expected to match test data from
    /// TestData/ESAutosuggestQuery/contain_cat_cancer.gov_en_patient.json
    /// </summary>
    public class AutosuggestScenario_ContainCatEnglishPatient : BaseAutosuggestTestData
    {
        public override string TestFilename => "contain_cat_cancer.gov_en_patient.json";

        public override Suggestion[] ExpectedData => new Suggestion[]
        {
            new Suggestion()
            {
                TermName = "anti-CD22 immunotoxin CAT-8015",
                TermId = 562550
            },
            new Suggestion()
            {
                TermName = "balloon catheter radiation",
                TermId = 674942
            },
            new Suggestion()
            {
                TermName = "central venous access catheter",
                TermId = 45962
            },
            new Suggestion()
            {
                TermName = "external right atrial catheter",
                TermId = 689016
            },
            new Suggestion()
            {
                TermName = "peripheral venous catheter",
                TermId = 463728
            },
            new Suggestion()
            {
                TermName = "peripherally inserted central catheter",
                TermId = 689084
            },
            new Suggestion()
            {
                TermName = "transient receptor potential cation channel, subfamily V, member 6",
                TermId = 655135
            },
            new Suggestion()
            {
                TermName = "venous catheter",
                TermId = 689105
            }
        };
    }
}