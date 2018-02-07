using AutoMapper;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<IInvoice, Invoice>(MemberList.Source)
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentRequestId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());

            CreateMap<IInvoice, InvoiceDetails>(MemberList.Source);
        }
    }
}
