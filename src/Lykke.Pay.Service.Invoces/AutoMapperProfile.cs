using AutoMapper;
using Lykke.Pay.Service.Invoces.Core.Domain;
using Lykke.Pay.Service.Invoces.Models.File;
using Lykke.Pay.Service.Invoces.Models.Invoice;

namespace Lykke.Pay.Service.Invoces
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<IInvoice, InvoiceModel>(MemberList.Source);
            CreateMap<InvoiceModel, Invoice>(MemberList.Destination);

            CreateMap<NewInvoiceModel, Invoice>(MemberList.Destination)
                .ForMember(src => src.InvoiceId, opt => opt.Ignore())
                .ForMember(src => src.ClientId, opt => opt.Ignore())
                .ForMember(src => src.ClientUserId, opt => opt.Ignore())
                .ForMember(src => src.Label, opt => opt.Ignore())
                .ForMember(src => src.Status, opt => opt.Ignore())
                .ForMember(src => src.WalletAddress, opt => opt.Ignore())
                .ForMember(src => src.StartDate, opt => opt.Ignore())
                .ForMember(src => src.Transaction, opt => opt.Ignore());

            CreateMap<UpdateInvoiceModel, Invoice>(MemberList.Destination)
                .ForMember(src => src.ClientId, opt => opt.Ignore())
                .ForMember(src => src.ClientUserId, opt => opt.Ignore())
                .ForMember(src => src.Label, opt => opt.Ignore())
                .ForMember(src => src.Status, opt => opt.Ignore())
                .ForMember(src => src.WalletAddress, opt => opt.Ignore())
                .ForMember(src => src.StartDate, opt => opt.Ignore())
                .ForMember(src => src.Transaction, opt => opt.Ignore());

            CreateMap<NewDraftInvoiceModel, Invoice>(MemberList.Destination)
                .ForMember(src => src.InvoiceId, opt => opt.Ignore())
                .ForMember(src => src.ClientId, opt => opt.Ignore())
                .ForMember(src => src.ClientUserId, opt => opt.Ignore())
                .ForMember(src => src.Label, opt => opt.Ignore())
                .ForMember(src => src.Status, opt => opt.Ignore())
                .ForMember(src => src.WalletAddress, opt => opt.Ignore())
                .ForMember(src => src.StartDate, opt => opt.Ignore())
                .ForMember(src => src.Transaction, opt => opt.Ignore());

            CreateMap<UpdateDraftInvoiceModel, Invoice>(MemberList.Destination)
                .ForMember(src => src.ClientId, opt => opt.Ignore())
                .ForMember(src => src.ClientUserId, opt => opt.Ignore())
                .ForMember(src => src.Label, opt => opt.Ignore())
                .ForMember(src => src.Status, opt => opt.Ignore())
                .ForMember(src => src.WalletAddress, opt => opt.Ignore())
                .ForMember(src => src.StartDate, opt => opt.Ignore())
                .ForMember(src => src.Transaction, opt => opt.Ignore());

            CreateMap<IFileInfo, FileInfoModel>(MemberList.Source);

            CreateMap<IInvoice, InvoiceSummaryModel>(MemberList.Destination)
                .ForMember(dest => dest.InvoiceAmount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.InvoiceCurrency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.OrderAmount, opt => opt.Ignore())
                .ForMember(dest => dest.OrderCurrency, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionTime, opt => opt.Ignore());

            CreateMap<IOrder, InvoiceSummaryModel>(MemberList.Destination)
                .ForMember(dest => dest.OrderAmount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.OrderCurrency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.InvoiceId, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceAmount, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceCurrency, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceNumber, opt => opt.Ignore())
                .ForMember(dest => dest.ClientName, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.WalletAddress, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionTime, opt => opt.MapFrom(src => src.TransactionWaitingTime));
        }
    }
}
