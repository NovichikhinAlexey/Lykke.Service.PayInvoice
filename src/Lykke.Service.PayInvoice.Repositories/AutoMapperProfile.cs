using AutoMapper;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Repositories
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
