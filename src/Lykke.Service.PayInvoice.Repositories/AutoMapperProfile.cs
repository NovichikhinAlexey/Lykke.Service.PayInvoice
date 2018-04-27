using System;
using AutoMapper;
using Lykke.Service.PayInvoice.Core.Domain;

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
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => (InvoiceStatus) Enum.Parse(typeof(InvoiceStatus), src.Status)));

            CreateMap<Invoice, InvoiceEntity>(MemberList.Source)
                .ForSourceMember(src => src.Id, opt => opt.Ignore());

            CreateMap<EmployeeEntity, Employee>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RowKey));

            CreateMap<Employee, EmployeeEntity>(MemberList.Source)
                .ForSourceMember(src => src.Id, opt => opt.Ignore());

            CreateMap<HistoryItemEntity, HistoryItem>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RowKey));

            CreateMap<HistoryItem, HistoryItemEntity>(MemberList.Source)
                .ForSourceMember(src => src.Id, opt => opt.Ignore());
        }
    }
}
