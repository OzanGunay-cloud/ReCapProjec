using Core.DataAccess.Abstract;
using Core.Entities; // IDto'nun olduğu yer

public class CarDetailDto : IDto
{
    public int CarId { get; set; }
    public string CarName { get; set; }
    public string BrandName { get; set; }
    public string ColorName { get; set; }
    public decimal DailyPrice { get; set; }
    public string Description { get; set; }
    public int ModelYear { get; set; }

    public List<string> ImagePath { get; set; }
    // İstersen buraya arabanın resim yolunu vs. de ekleyebilirsin

}