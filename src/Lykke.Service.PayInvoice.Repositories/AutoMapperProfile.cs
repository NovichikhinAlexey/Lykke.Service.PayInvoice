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

            CreateMap<IFileInfo, FileInfoEntity>(MemberList.Source)
                .ForSourceMember(src => src.Id, opt => opt.Ignore());

            CreateMap<InvoiceEntity, Invoice>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RowKey))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => (InvoiceStatus) Enum.Parse(typeof(InvoiceStatus), src.Status)));

            CreateMap<IInvoice, InvoiceEntity>(MemberList.Source)
                .ForSourceMember(src => src.Id, opt => opt.Ignore());
        }
    }
}
