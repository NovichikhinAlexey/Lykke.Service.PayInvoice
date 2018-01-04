namespace Lykke.Pay.Service.Invoces.Core.Domain
{
    public interface IFileEntity : IFileMetaEntity
    {
        string FileBodyBase64 { get; set; }
    }
}