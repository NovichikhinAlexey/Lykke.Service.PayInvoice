using System;
using AutoMapper;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Domain.PaymentRequest;
using Lykke.Service.PayInvoice.Repositories.InvoiceDisputes;
using Lykke.Service.PayInvoice.Repositories.PaymentRequestHistory;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<FileInfoEntity, FileInfo>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RowKey));

            CreateMap<FileInfo, FileInfoEntity>(MemberList.Source)
                .ForSourceMember(src => src.Id, opt => opt.Ignore());

            CreateMap<InvoiceEntity, Invoice>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RowKey))
                .ForMember(dest => dest.LeftAmountToPayInSettlementAsset, opt => opt.Ignore())
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => (InvoiceStatus) Enum.Parse(typeof(InvoiceStatus), src.Status)));

            CreateMap<Invoice, InvoiceEntity>(MemberList.Source)
                .ForSourceMember(src => src.LeftAmountToPayInSettlementAsset, opt => opt.Ignore())
                .ForSourceMember(src => src.Id, opt => opt.Ignore());

            CreateMap<EmployeeEntity, Employee>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RowKey));

            CreateMap<Employee, EmployeeEntity>(MemberList.Source)
                .ForSourceMember(src => src.Id, opt => opt.Ignore());

            CreateMap<HistoryItemEntity, HistoryItem>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RowKey));

            CreateMap<HistoryItem, HistoryItemEntity>(MemberList.Source)
                .ForSourceMember(src => src.Id, opt => opt.Ignore());

            CreateMap<MerchantSettingEntity, MerchantSetting>(MemberList.Destination)
                .ForMember(dest => dest.MerchantId, opt => opt.MapFrom(src => src.RowKey));

            CreateMap<MerchantSetting, MerchantSettingEntity>(MemberList.Source)
                .ForSourceMember(src => src.MerchantId, opt => opt.Ignore());

            CreateMap<PaymentRequestHistoryItemEntity, PaymentRequestHistoryItem>(MemberList.Destination)
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.PartitionKey))
                .ForMember(dest => dest.PaymentRequestId, opt => opt.MapFrom(src => src.RowKey));

            CreateMap<PaymentRequestHistoryItem, PaymentRequestHistoryItemEntity>(MemberList.Source)
                .ForSourceMember(src => src.InvoiceId, opt => opt.Ignore())
                .ForSourceMember(src => src.PaymentRequestId, opt => opt.Ignore());

            CreateMap<InvoiceDisputeEntity, InvoiceDispute>(MemberList.Destination);

            CreateMap<InvoiceDispute, InvoiceDisputeEntity>(MemberList.Source)
                .ForSourceMember(src => src.InvoiceId, opt => opt.Ignore());
        }
    }
}
