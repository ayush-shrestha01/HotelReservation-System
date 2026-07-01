namespace Hrs.Website.Shared.Dtos;

public class HotelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public int TotalRooms { get; set; }
}

public class CreateHotelDto
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int StarRating { get; set; }
}

public class UpdateHotelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int StarRating { get; set; }
}
