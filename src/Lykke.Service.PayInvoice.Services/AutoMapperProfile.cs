using AutoMapper;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Invoice, InvoiceDetails>(MemberList.Source);

            CreateMap<Invoice, HistoryItem>(MemberList.Source)
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PaymentAmount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.ModifiedById, opt => opt.MapFrom(src => src.EmployeeId))
                .ForSourceMember(src => src.MerchantId, opt => opt.Ignore())
                .ForSourceMember(src => src.Note, opt => opt.Ignore())
                .ForSourceMember(src => src.CreatedDate, opt => opt.Ignore());
        }
    }
}
