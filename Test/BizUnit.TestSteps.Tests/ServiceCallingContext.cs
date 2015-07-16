
namespace BizUnit.TestSteps.Tests
{
    using System;
    using System.Runtime.Serialization;

    // TODO: Change namespace....
    [DataContract(Namespace = "http://schemas.virgin-atlantic.com/Services/ServiceCallingContext/2009")]
    public class ServiceCallingContext
    {
        [DataMember(Name = "ApplicationName", IsRequired = true)]
        public string ApplicationName { get; set; }

        [DataMember(Name = "GUID", IsRequired = true)]
        public string GUid { get; set; }

        [DataMember(Name = "UTCTransactionStartDate", EmitDefaultValue = false, IsRequired = false)]
        public DateTime UTCTransactionStartDate { get; set; }

        [DataMember(Name = "UTCTransactionStartTime", EmitDefaultValue = false, IsRequired = false)]
        public string UTCTransactionStartTime { get; set; }

        [DataMember(Name = "RequestId", EmitDefaultValue = false, IsRequired = false)]
        public string RequestId { get; set; }

        [DataMember(Name = "UserId", EmitDefaultValue = false, IsRequired = false)]
        public string UserId { get; set; }
    }
}
