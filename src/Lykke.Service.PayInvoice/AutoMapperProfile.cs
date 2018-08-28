using AutoMapper;
using Lykke.Service.PayInvoice.Contract.Commands;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Models.Employee;
using Lykke.Service.PayInvoice.Models.File;
using Lykke.Service.PayInvoice.Models.Invoice;
using Lykke.Service.PayInvoice.Models.MerchantSetting;

namespace Lykke.Service.PayInvoice
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Invoice, InvoiceModel>(MemberList.Source)
                .ForSourceMember(src => src.TotalPaidAmountInSettlementAsset, opt => opt.Ignore())
                .ForSourceMember(src => src.HasMultiplePaymentRequests, opt => opt.Ignore());

            CreateMap<CreateInvoiceModel, Invoice>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.PaidAmount, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentRequestId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPaidAmountInSettlementAsset, opt => opt.Ignore())
                .ForMember(dest => dest.LeftAmountToPayInSettlementAsset, opt => opt.Ignore())
                .ForMember(dest => dest.HasMultiplePaymentRequests, opt => opt.Ignore());

            CreateMap<UpdateInvoiceModel, Invoice>(MemberList.Destination)
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.PaidAmount, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentRequestId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPaidAmountInSettlementAsset, opt => opt.Ignore())
                .ForMember(dest => dest.LeftAmountToPayInSettlementAsset, opt => opt.Ignore())
                .ForMember(dest => dest.HasMultiplePaymentRequests, opt => opt.Ignore());

            CreateMap<FileInfo, FileInfoModel>(MemberList.Source)
                .ForSourceMember(src => src.InvoiceId, opt => opt.Ignore());

            CreateMap<Employee, EmployeeModel>(MemberList.Source);

            CreateMap<CreateEmployeeModel, Employee>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<RegisterEmployeeCommand, Employee>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsBlocked, opt => opt.UseValue(false))
                .ForMember(dest => dest.IsInternalSupervisor, opt => opt.UseValue(false));

            CreateMap<UpdateEmployeeModel, Employee>(MemberList.Destination);

            CreateMap<HistoryItem, HistoryItemModel>(MemberList.Source);

            CreateMap<SetMerchantSettingModel, MerchantSetting>(MemberList.Source);
        }
    }
}
