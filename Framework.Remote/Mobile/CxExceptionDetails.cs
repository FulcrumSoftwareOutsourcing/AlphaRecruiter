using System.Runtime.Serialization;

namespace Framework.Remote.Mobile
{
    /// <summary>
    /// Represents Exception that will be send to Client.
    /// </summary>
    [DataContract(Name = "CxExceptionDetails", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
    public partial class CxExceptionDetails
    {
        //----------------------------------------------------------------------------
        /// <summary>
        /// Gets the inner Exception details. 
        /// </summary>
        [DataMember]
        public CxExceptionDetails InnerException { get; set; }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Exception message.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Exception Stack Trace.
        /// </summary>
        [DataMember]
        public string StackTrace { get; set; }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Exception class type name.
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        //-------------------------------------------------------------------------
        /// <summary>
        /// Represents the exception casted as a string.
        /// </summary>
        [DataMember]
        public string AsString { get; set; }
    }
}
