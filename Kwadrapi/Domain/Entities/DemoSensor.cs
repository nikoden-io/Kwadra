namespace Domain.Entities;

public class DemoSensor
{
    public required string Id { get; set; }

    public int Temperature { get; set; }

    public int Humidity { get; set; }
}