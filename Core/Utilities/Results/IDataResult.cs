namespace Core.Utilities.Results
{
    public interface IDataResult<T> : IResult
    {
        T Data { get; }
    }
    //Eğer IDataResult, IResult'tan türemeseydi,
    //veri döndüren metotlarda hem mesajı hem de veriyi kontrol etmek için iki ayrı nesneyle uğraşman gerekirdi.

}