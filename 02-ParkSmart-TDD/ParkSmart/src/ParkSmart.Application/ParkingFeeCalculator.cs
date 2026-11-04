using ParkSmart.Domain;

namespace ParkSmart.Application.Services;

public class ParkingFeeCalculator
{
    public decimal Calculate(ParkingSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        if (session.ExitTime < session.EntryTime)
            throw new ArgumentException("Exit time must be after entry time.");

        if (session.ChargedKwh < 0)
            throw new ArgumentException("Charged kWh cannot be negative.");

        var duration = session.ExitTime - session.EntryTime;
        var freeMinutes = session.EntryTime.DayOfWeek == DayOfWeek.Friday ? 120 : 15;
        decimal parkingFee;

        if (!session.TicketLost && duration.TotalMinutes <= freeMinutes)
        {
            parkingFee = 0;
        }
        else
        {
            parkingFee = NormalFee(duration, session.Vehicle);

            if (QualifiesForNightRate(session))
                parkingFee = Math.Min(parkingFee, NightRate(session.Vehicle));
        }

        if (session.IsSubscriber)
        {
            parkingFee = QualifiesForSubscriberFreeWindow(session)
                ? 0
                : parkingFee * 0.5m;
        }

        if (session.TicketLost)
            parkingFee = Math.Max(parkingFee, LostTicketMinimum(session.Vehicle));

        var total = parkingFee + session.ChargedKwh * 3.50m;
        return Math.Round(total, 2, MidpointRounding.AwayFromZero);
    }

    private static decimal NormalFee(TimeSpan duration, VehicleType vehicle)
    {
        var (hourlyRate, dailyCap) = Rates(vehicle);
        var fullDays = (int)(duration.TotalMinutes / (24 * 60));
        var remainingMinutes = duration.TotalMinutes - fullDays * 24 * 60;
        var remainingHours = (int)Math.Ceiling(remainingMinutes / 60);

        return fullDays * dailyCap + Math.Min(remainingHours * hourlyRate, dailyCap);
    }

    private static bool QualifiesForNightRate(ParkingSession session)
    {
        var nextMorning = session.EntryTime.Date.AddDays(1).AddHours(8);
        return session.EntryTime.TimeOfDay >= TimeSpan.FromHours(22)
            && session.ExitTime <= nextMorning;
    }

    private static bool QualifiesForSubscriberFreeWindow(ParkingSession session)
    {
        var weekday = session.EntryTime.DayOfWeek is >= DayOfWeek.Sunday and <= DayOfWeek.Thursday;
        return weekday
            && session.EntryTime.Date == session.ExitTime.Date
            && session.EntryTime.TimeOfDay >= TimeSpan.FromHours(7)
            && session.ExitTime.TimeOfDay <= TimeSpan.FromHours(19);
    }

    private static (decimal Hourly, decimal Cap) Rates(VehicleType vehicle) => vehicle switch
    {
        VehicleType.Motorcycle => (5m, 40m),
        VehicleType.Truck => (25m, 200m),
        _ => (10m, 80m)
    };

    private static decimal NightRate(VehicleType vehicle) => vehicle switch
    {
        VehicleType.Motorcycle => 15m,
        VehicleType.Truck => 75m,
        _ => 30m
    };

    private static decimal LostTicketMinimum(VehicleType vehicle) => vehicle switch
    {
        VehicleType.Motorcycle => 75m,
        VehicleType.Truck => 300m,
        _ => 150m
    };
}
