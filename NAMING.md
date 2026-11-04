# Naming Convention

- Name: Ahmed Ismail

## Tests

Test projects, files and classes use `<ClassUnderTest>Tests`.

I compared these method styles:

| Style | Example | Notes |
| --- | --- | --- |
| `Method_Scenario_ExpectedResult` | `Calculate_WhenTicketIsLost_AppliesMinimum` | Explicit about the method and result. |
| `Should_X_When_Y` | `ShouldApplyMinimum_WhenTicketIsLost` | Reads naturally, but the method under test is less obvious. |
| `Given_When_Then` | `GivenLostTicket_WhenCalculated_ThenMinimumIsApplied` | Useful for BDD, but verbose for this project. |

I use `Method_Scenario_ExpectedResult`. DriveNow examples are
`CalculateAge_WhenBirthdayHasNotOccurred_ReturnsPreviousAge`,
`CalculateRentalDays_WhenGraceHourIsExceeded_AddsOneDay`, and
`CreateBooking_WhenVehicleOverlaps_RejectsBooking`.

Hand-written doubles use their role and behavior: `DummyEmailSender`,
`StubCustomerRepository`, `SpySmsSender`, `MockPaymentGateway`, and
`FakeBookingRepository`. Bogus builders end in `Faker`, customizations end in
`Customization`, and AutoFixture data attributes end in `DataAttribute`.

## Production

- Interfaces start with `I`.
- Repository classes end with `Repository` and live in a `Repositories` namespace.
- Service classes end with `Service` and live in a `Services` namespace.
- Exceptions end with `Exception` and live in the domain `Exceptions` namespace.
