using AutoMapper;
using Lykke.Service.PayHistory.Client.Publisher;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Domain.HistoryOperation;

namespace Lykke.Service.PayInvoice.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Invoice, HistoryItem>(MemberList.Source)
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SettlementAmount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.ModifiedById, opt => opt.MapFrom(src => src.EmployeeId))
                .ForSourceMember(src => src.MerchantId, opt => opt.Ignore())
                .ForSourceMember(src => src.Note, opt => opt.Ignore())
                .ForSourceMember(src => src.CreatedDate, opt => opt.Ignore())
                .ForSourceMember(src => src.TotalPaidAmountInSettlementAsset, opt => opt.Ignore())
                .ForSourceMember(src => src.LeftAmountToPayInSettlementAsset, opt => opt.Ignore())
                .ForSourceMember(src => src.HasMultiplePaymentRequests, opt => opt.Ignore());

            CreateMap<HistoryOperationCommand, HistoryOperation>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.Ignore());
        }
    }
}
