using Nest;
namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Class representing the details required for Pronunciation
    /// </summary>
    public class Pronunciation
    {
        /// <summary>
        /// Gets or sets the Key for Pronunciation
        /// </summary>
        [Keyword(Name = "key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value for Audio for Pronunciation
        /// </summary>
        [Keyword(Name = "audio")]
        public string Audio { get; set; }
        /// TODO Convert string to URL class

        /// <summary>
        /// No Arg Constructor
        /// </summary>
        public Pronunciation() { }

        /// <summary>
        /// 2 Arg Constructor
        /// </summary>
        public Pronunciation(string Key, string Audio) {
            this.Key = Key;
            this.Audio = Audio;
        }
    }
}