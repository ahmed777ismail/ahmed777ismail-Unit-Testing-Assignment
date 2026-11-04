using ParkSmart.Application.Services;
using ParkSmart.Domain;

namespace ParkSmart.UnitTests;

public class ParkingFeeCalculatorTests
{
    private readonly ParkingFeeCalculator _calculator = new();
    private static readonly DateTime Monday = new(2026, 11, 2, 9, 0, 0);

    [Theory]
    [InlineData(0, 0)]
    [InlineData(15, 0)]
    [InlineData(16, 10)]
    [InlineData(60, 10)]
    [InlineData(61, 20)]
    [InlineData(180, 30)]
    [InlineData(600, 80)]
    [InlineData(1800, 140)]
    [InlineData(2880, 160)]
    [InlineData(2890, 170)]
    public void Calculate_GivenCarDuration_ReturnsExpectedFee(int minutes, decimal expected)
    {
        var session = Session(Monday, minutes);

        var result = _calculator.Calculate(session);

        result.Should().Be(expected);
    }

    [Fact]
    public void Calculate_WhenExitIsBeforeEntry_ThrowsArgumentException()
    {
        var session = Session(Monday, -1);

        var action = () => _calculator.Calculate(session);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Exit time must be after entry time.");
    }

    [Theory]
    [InlineData(VehicleType.Motorcycle, 180, 15)]
    [InlineData(VehicleType.Truck, 600, 200)]
    [InlineData(VehicleType.Truck, 1800, 350)]
    public void Calculate_GivenVehicleType_AppliesItsRates(VehicleType type, int minutes, decimal expected)
    {
        var session = Session(Monday, minutes, type);

        var result = _calculator.Calculate(session);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(23, 0, 7, 0, 30)]
    [InlineData(22, 0, 23, 0, 10)]
    [InlineData(21, 59, 7, 0, 80)]
    [InlineData(23, 0, 8, 1, 80)]
    public void Calculate_GivenOvernightSession_AppliesNightRateBoundaries(
        int entryHour, int entryMinute, int exitHour, int exitMinute, decimal expected)
    {
        var entry = Monday.Date.AddHours(entryHour).AddMinutes(entryMinute);
        var exit = Monday.Date.AddDays(1).AddHours(exitHour).AddMinutes(exitMinute);
        var session = new ParkingSession { EntryTime = entry, ExitTime = exit };

        var result = _calculator.Calculate(session);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(VehicleType.Car, 5, 150)]
    [InlineData(VehicleType.Car, 1800, 150)]
    [InlineData(VehicleType.Motorcycle, 5, 75)]
    [InlineData(VehicleType.Truck, 1800, 350)]
    public void Calculate_WhenTicketIsLost_AppliesMinimumAfterNormalFee(
        VehicleType type, int minutes, decimal expected)
    {
        var session = Session(Monday, minutes, type);
        session.TicketLost = true;

        var result = _calculator.Calculate(session);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(120, 0)]
    [InlineData(121, 30)]
    public void Calculate_WhenEntryIsFriday_AppliesTwoHourFreePeriod(int minutes, decimal expected)
    {
        var friday = new DateTime(2026, 11, 6, 9, 0, 0);
        var session = Session(friday, minutes);

        var result = _calculator.Calculate(session);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(DayOfWeek.Monday, 9, 17, false, 0)]
    [InlineData(DayOfWeek.Monday, 18, 20, false, 10)]
    [InlineData(DayOfWeek.Saturday, 9, 12, false, 15)]
    [InlineData(DayOfWeek.Monday, 9, 10, true, 150)]
    public void Calculate_WhenSubscriber_AppliesFreeWindowOrDiscount(
        DayOfWeek day, int entryHour, int exitHour, bool lost, decimal expected)
    {
        var date = DateFor(day);
        var session = new ParkingSession
        {
            EntryTime = date.AddHours(entryHour),
            ExitTime = date.AddHours(exitHour),
            IsSubscriber = true,
            TicketLost = lost
        };

        var result = _calculator.Calculate(session);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(false, 120, 10, 55)]
    [InlineData(true, 60, 4, 14)]
    public void Calculate_WithCharging_AddsChargingAfterParkingRules(
        bool subscriber, int minutes, decimal kwh, decimal expected)
    {
        var session = Session(Monday, minutes);
        session.IsSubscriber = subscriber;
        session.ChargedKwh = kwh;

        var result = _calculator.Calculate(session);

        result.Should().Be(expected);
    }

    [Fact]
    public void Calculate_WithNegativeCharging_ThrowsArgumentException()
    {
        var session = Session(Monday, 60);
        session.ChargedKwh = -1;

        var action = () => _calculator.Calculate(session);

        action.Should().Throw<ArgumentException>();
    }

    private static ParkingSession Session(DateTime entry, int minutes, VehicleType type = VehicleType.Car) =>
        new() { EntryTime = entry, ExitTime = entry.AddMinutes(minutes), Vehicle = type };

    private static DateTime DateFor(DayOfWeek day)
    {
        var date = Monday.Date;
        while (date.DayOfWeek != day)
            date = date.AddDays(1);
        return date;
    }
}
