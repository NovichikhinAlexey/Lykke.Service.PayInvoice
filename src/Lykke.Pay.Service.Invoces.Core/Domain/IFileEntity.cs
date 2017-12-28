namespace Lykke.Pay.Service.Invoces.Core.Domain
{
    public interface IFileEntity : IFileMetaEntity
    {
        byte[] FileBody { get; set; }
    }
}