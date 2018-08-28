using ProtoBuf;

namespace Lykke.Service.PayInvoice.Contract.Events
{
    [ProtoContract]
    public class EmployeeRegisteredEvent
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string Email { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string MerchantId { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string Password { get; set; }
    }
}
