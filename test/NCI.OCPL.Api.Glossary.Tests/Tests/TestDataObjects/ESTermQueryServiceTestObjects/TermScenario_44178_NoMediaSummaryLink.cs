using System;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.BestBets.Tests.ESTermQueryTestData
{
  public class TermScenario_44178_NoMediaSummaryLink : BaseTermQueryTestData
  {
    public override string DictionaryName => "cancer.gov";
    public override AudienceType Audience => AudienceType.Patient;
    public override long TermID => 44178L;

    public override string ESTermID => "44178_cancer.gov_en_patient";
    public override string Language => "en";

    public override GlossaryTerm ExpectedData => new GlossaryTerm()
    {
      TermId = 44178L,
      Language = "en",
      Dictionary = "Cancer.gov",
      Audience = AudienceType.Patient,
      TermName = "terminal cancer",
      FirstLetter = "t",
      PrettyUrlName = "terminal-cancer",
      Definition = new Definition() {
        Text = "Cancer that cannot be cured and leads to death.  Also called end-stage cancer.",
        Html = "Cancer that cannot be cured and leads to death.  Also called end-stage cancer."
      },
      Pronunciation = new Pronunciation()
      {
        Key = "(TER-mih-nul KAN-ser)",
        Audio = "https://nci-media-dev.cancer.gov/audio/pdq/703944.mp3"
      },
      RelatedResources = new IRelatedResource[] {
        new LinkResource {
          Type = RelatedResourceType.External,
          Text = "End-of-Life Care for People Who Have Cancer",
          Url = new Uri("https://www.cancer.gov/about-cancer/advanced-cancer/care-choices/care-fact-sheet"),
        },
        new LinkResource {
          Type = RelatedResourceType.Summary,
          Text = "Planning the Transition to End-of-Life Care in Advanced Cancer",
          Url = new Uri("https://www.cancer.gov/about-cancer/advanced-cancer/planning/end-of-life-pdq"),
        }
      }
    };
  }
}
