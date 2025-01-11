namespace Campaign.Infrastructure.Utils.Other;

public readonly struct GeoCoordinate
{
    public GeoCoordinate(double latitude, double longitude)
    {
        this.Latitude = latitude;
        this.Longitude = longitude;
    }

    public double Latitude { get; }
    public double Longitude { get; }

    public override string ToString()
    {
        return $"{Latitude},{Longitude}";
    }
}