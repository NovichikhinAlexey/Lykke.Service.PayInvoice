using AutoMapper;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Models.Employee;
using Lykke.Service.PayInvoice.Models.File;
using Lykke.Service.PayInvoice.Models.Invoice;

namespace Lykke.Service.PayInvoice
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Invoice, InvoiceModel>(MemberList.Source);

            CreateMap<CreateInvoiceModel, Invoice>(MemberList.Destination)
                .ForMember(src => src.Id, opt => opt.Ignore())
                .ForMember(src => src.Status, opt => opt.Ignore())
                .ForMember(src => src.PaymentRequestId, opt => opt.Ignore())
                .ForMember(src => src.WalletAddress, opt => opt.Ignore())
                .ForMember(src => src.MerchantId, opt => opt.Ignore())
                .ForMember(src => src.CreatedDate, opt => opt.Ignore());

            CreateMap<CreateDraftInvoiceModel, Invoice>(MemberList.Destination)
                .ForMember(src => src.Id, opt => opt.Ignore())
                .ForMember(src => src.Status, opt => opt.Ignore())
                .ForMember(src => src.PaymentRequestId, opt => opt.Ignore())
                .ForMember(src => src.WalletAddress, opt => opt.Ignore())
                .ForMember(src => src.MerchantId, opt => opt.Ignore())
                .ForMember(src => src.CreatedDate, opt => opt.Ignore());

            CreateMap<FileInfo, FileInfoModel>(MemberList.Source)
                .ForSourceMember(src => src.InvoiceId, opt => opt.Ignore());

            CreateMap<InvoiceDetails, InvoiceDetailsModel>(MemberList.Source);

            CreateMap<Employee, EmployeeModel>(MemberList.Source);

            CreateMap<CreateEmployeeModel, Employee>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
