using ProtoBuf;

namespace Lykke.Service.PayInvoice.Contract.Commands
{
    [ProtoContract]
    public class RegisterEmployeeCommand
    {
        [ProtoMember(1, IsRequired = true)]
        public string MerchantId { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string Email { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string FirstName { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string LastName { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public string Password { get; set; }
    }
}
