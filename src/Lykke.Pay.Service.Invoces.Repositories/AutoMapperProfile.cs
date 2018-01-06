using AutoMapper;
using Lykke.Pay.Service.Invoces.Core.Domain;

namespace Lykke.Pay.Service.Invoces.Repositories
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<FileInfoEntity, FileInfo>(MemberList.Destination)
                .ForMember(dest => dest.FileId, opt => opt.MapFrom(src => src.RowKey));

            CreateMap<IFileInfo, FileInfoEntity>(MemberList.Source)
                .ForSourceMember(src => src.FileId, opt => opt.Ignore());

            CreateMap<InvoiceEntity, Invoice>(MemberList.Destination);

            CreateMap<IInvoice, InvoiceEntity>(MemberList.Source);
        }
    }
}
