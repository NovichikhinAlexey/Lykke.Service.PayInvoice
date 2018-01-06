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
                .ForMember(src => src.InvoiceId, opt => opt.Ignore());

            CreateMap<IFileInfo, FileInfoModel>(MemberList.Source);
        }
    }
}
