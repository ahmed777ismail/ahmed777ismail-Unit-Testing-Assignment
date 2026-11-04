using System.Reflection;
using NetArchTest.Rules;
using ParkSmart.Domain;

namespace ParkSmart.ArchitectureTests;

public class ArchitectureTests
{
    [Fact]
    public void Domain_ShouldNotDependOnOtherParkSmartProjects()
    {
        var result = Types.InAssembly(typeof(ParkingSession).Assembly)
            .ShouldNot().HaveDependencyOnAny(
                "ParkSmart.Application",
                "ParkSmart.Infrastructure",
                "ParkSmart.ConsoleApp")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldOnlyDependOnDomain()
    {
        var assembly = Assembly.Load("ParkSmart.Application");
        var result = Types.InAssembly(assembly)
            .ShouldNot().HaveDependencyOnAny("ParkSmart.Infrastructure", "ParkSmart.ConsoleApp")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnConsoleApp()
    {
        var assembly = Assembly.Load("ParkSmart.Infrastructure");
        var result = Types.InAssembly(assembly)
            .ShouldNot().HaveDependencyOn("ParkSmart.ConsoleApp")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ServiceTypes_ShouldLiveInServicesNamespace()
    {
        var assembly = Assembly.Load("ParkSmart.Application");
        var serviceTypes = assembly.GetTypes()
            .Where(type => type.Name.EndsWith("Service"));

        serviceTypes.Should().OnlyContain(type =>
            type.Namespace is not null && type.Namespace.EndsWith(".Services"));
    }

    [Fact]
    public void TestClasses_ShouldEndWithTests()
    {
        var testTypes = typeof(ArchitectureTests).Assembly.GetTypes()
            .Where(type => type.GetMethods().Any(method =>
                method.GetCustomAttributes().Any(attribute =>
                    attribute.GetType().Name is "FactAttribute" or "TheoryAttribute")));

        testTypes.Should().OnlyContain(type => type.Name.EndsWith("Tests"));
    }
}
