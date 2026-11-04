using DriveNow.Domain.Services;
using FluentAssertions;
using Xunit;

namespace DriveNow.UnitTests;

public class RentalMathTests
{
    [Theory]
    [InlineData("2000-06-15", "2026-06-14", 25)]
    [InlineData("2000-06-15", "2026-06-15", 26)]
    [InlineData("2000-06-15", "2026-06-16", 26)]
    [InlineData("2000-02-29", "2025-02-27", 24)]
    [InlineData("2000-02-29", "2025-02-28", 25)]
    [Trait("Rule", "BR-01")]
    public void CalculateAge_GivenDate_ReturnsCompletedYears(string birth, string onDate, int expected)
    {
        var result = RentalMath.CalculateAge(DateTime.Parse(birth), DateTime.Parse(onDate));

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("2025-11-02", "2026-11-01", 0)]
    [InlineData("2025-11-02", "2026-11-02", 1)]
    [InlineData("2025-11-02", "2026-11-03", 1)]
    [Trait("Rule", "BR-02")]
    public void CalculateLicenseYears_GivenDate_ReturnsCompletedYears(string issued, string onDate, int expected)
    {
        var result = RentalMath.CalculateLicenseYears(DateTime.Parse(issued), DateTime.Parse(onDate));

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(180, 1)]
    [InlineData(1499, 1)]
    [InlineData(1500, 1)]
    [InlineData(1501, 2)]
    [InlineData(4320, 3)]
    [Trait("Rule", "BR-03")]
    public void CalculateRentalDays_GivenValidSpan_AppliesGraceHour(int minutes, int expected)
    {
        var pickup = new DateTime(2026, 11, 3, 10, 0, 0);

        var result = RentalMath.CalculateRentalDays(pickup, pickup.AddMinutes(minutes));

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [Trait("Rule", "BR-03")]
    public void CalculateRentalDays_WhenReturnIsNotAfterPickup_Throws(int minutes)
    {
        var pickup = new DateTime(2026, 11, 3, 10, 0, 0);
        var action = () => RentalMath.CalculateRentalDays(pickup, pickup.AddMinutes(minutes));

        action.Should().Throw<ArgumentException>()
            .WithMessage("Return must be after pickup");
    }
}
