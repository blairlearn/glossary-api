using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Friendly names for the specific audience a glossary term is intended for.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AudienceType
    {
        /// <summary>
        /// AudienceType value Patient
        /// </summary>
        Patient,

        /// <summary>
        /// AudienceType value HealthProfessional
        /// </summary>
        HealthProfessional
    }
}