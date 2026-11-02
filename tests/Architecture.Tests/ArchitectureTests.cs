namespace Architecture.Tests;

/// <summary>
/// Enforces the Clean Architecture dependency rule: dependencies may only point inward.
///
///   Api (Web + Presentation)  ──►  Infrastructure  ──►  Application  ──►  Domain
/// </summary>
public class ArchitectureTests
{
    private const string DomainNamespace = "Domain";
    private const string ApplicationNamespace = "Application";
    private const string InfrastructureNamespace = "Infrastructure";
    private const string ApiNamespace = "Api";
    private const string ControllersNamespace = "Api.Controllers";

    // Legacy root namespaces still used by a few files.
    private const string LegacyInfrastructureNamespace = "Sakani.Infrastructure";
    private const string LegacyApiNamespace = "Sakani.API";
    private const string LegacyWebApiNamespace = "WebApi";

    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Domain.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            ApplicationNamespace,
            InfrastructureNamespace,
            LegacyInfrastructureNamespace,
            ApiNamespace,
            LegacyApiNamespace,
            LegacyWebApiNamespace,
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue(FailureMessage(testResult));
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Application.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            InfrastructureNamespace,
            LegacyInfrastructureNamespace,
            ApiNamespace,
            LegacyApiNamespace,
            LegacyWebApiNamespace,
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue(FailureMessage(testResult));
    }

    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Infrastructure.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            ApiNamespace,
            LegacyApiNamespace,
            LegacyWebApiNamespace,
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue(FailureMessage(testResult));
    }

    [Fact]
    public void Controllers_Should_Not_HaveDependencyOnInfrastructure()
    {
        // Arrange
        // Api is the composition root (Program.cs wires Infrastructure up), but the
        // controllers are the presentation layer and must only talk to Application.
        var assembly = typeof(Api.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            InfrastructureNamespace,
            LegacyInfrastructureNamespace,
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .That()
            .ResideInNamespaceStartingWith(ControllersNamespace)
            .Or()
            .ResideInNamespaceStartingWith("Sakani.API.Controllers")
            .Or()
            .ResideInNamespaceStartingWith("WebApi.Controllers")
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue(FailureMessage(testResult));
    }

    [Fact]
    public void Domain_Should_Not_HaveDependencyOnEntityFrameworkCore()
    {
        // Arrange
        var assembly = typeof(Domain.AssemblyReference).Assembly;

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue(FailureMessage(testResult));
    }

    private static string FailureMessage(TestResult result) =>
        "these types break the dependency rule: " +
        string.Join(", ", result.FailingTypeNames ?? []);
}
