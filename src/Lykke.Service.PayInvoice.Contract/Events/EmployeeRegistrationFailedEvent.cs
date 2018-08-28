using ProtoBuf;

namespace Lykke.Service.PayInvoice.Contract.Events
{
    [ProtoContract]
    public class EmployeeRegistrationFailedEvent
    {
        [ProtoMember(1, IsRequired = true)]
        public string Email { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string Error { get; set; }
    }
}
