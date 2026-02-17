// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Nest;

using NixSearch.Core.Models;
using NixSearch.Core.Models.Package;
using NixSearch.Core.Search;

namespace NixSearch.Core.Tests.Integration;

/// <summary>
/// Integration tests for NixPackage model using real backend.
/// </summary>
public sealed class NixPackageIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests that searching for packages with a query returns valid results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchPackages_WithQuery_ReturnsResults()
    {
        // Arrange
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .WithQuery("vim")
            .Page(0, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.IsValid.Should().BeTrue();
        response.Documents.Should().NotBeEmpty();
    }

    /// <summary>
    /// Tests that packages can be accessed as BaseModel instances.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchPackages_VerifyBaseModelProperties()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .WithQuery("hello")
            .Page(0, 1)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        NixPackage package = response.Documents.First();
        package.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that all required package properties are populated correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchPackages_VerifyPackageProperties()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .WithQuery("hello")
            .Page(0, 1)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        NixPackage package = response.Documents.First();
        package.AttrName.Should().NotBeNullOrEmpty();
        package.AttrSet.Should().NotBeNullOrEmpty();
        package.Name.Should().NotBeNullOrEmpty();
        package.Version.Should().NotBeNullOrEmpty();
        package.Platforms.Should().NotBeNull();
        package.Outputs.Should().NotBeNull();
        package.Programs.Should().NotBeNull();
        package.License.Should().NotBeNull();
        package.LicenseSet.Should().NotBeNull();
        package.Maintainers.Should().NotBeNull();
        package.MaintainersSet.Should().NotBeNull();
        package.Teams.Should().NotBeNull();
        package.TeamsSet.Should().NotBeNull();
        package.System.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that the License sub-model is correctly deserialized from search results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchPackages_VerifyLicenseModel()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .WithQuery("vim")
            .Page(0, 10)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a package with license information
        NixPackage? packageWithLicense = response.Documents
            .FirstOrDefault(p => p.License.Length > 0 && p.License[0].FullName != null);

        packageWithLicense.Should().NotBeNull();
        packageWithLicense.License.Should().NotBeEmpty();
        packageWithLicense.License[0].Should().NotBeNull();
    }

    /// <summary>
    /// Tests that the Maintainer sub-model is correctly deserialized from search results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchPackages_VerifyMaintainerModel()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .WithQuery("vim")
            .Page(0, 10)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a package with maintainer information
        NixPackage? packageWithMaintainer = response.Documents
            .FirstOrDefault(p => p.Maintainers.Length > 0 && p.Maintainers[0].Name != null);

        packageWithMaintainer.Should().NotBeNull();
        packageWithMaintainer.Maintainers.Should().NotBeEmpty();
        packageWithMaintainer.Maintainers[0].Should().NotBeNull();
    }

    /// <summary>
    /// Tests that the Team sub-model is correctly deserialized from search results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchPackages_VerifyTeamModel()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .WithQuery("python")
            .Page(0, 20)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a package with team information
        NixPackage? packageWithTeam = response.Documents
            .FirstOrDefault(p => p.Teams.Length > 0);

        packageWithTeam.Should().NotBeNull();
        packageWithTeam.Teams.Should().NotBeEmpty();
        packageWithTeam.Teams[0].Should().NotBeNull();
    }

    /// <summary>
    /// Tests that the Hydra build information sub-model is correctly deserialized from search results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchPackages_VerifyHydraModel()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .WithQuery("hello")
            .Page(0, 10)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a package with Hydra build information
        NixPackage? packageWithHydra = response.Documents
            .FirstOrDefault(p => p.Hydra is { Length: > 0 });

        Assert.SkipWhen(packageWithHydra == null, "No package with Hydra information found in the search results.");

        packageWithHydra.Should().NotBeNull();
        packageWithHydra.Hydra.Should().NotBeNull();
        packageWithHydra.Hydra.Should().NotBeEmpty();

        Hydra hydra = packageWithHydra.Hydra[0];
        hydra.BuildId.Should().BeGreaterThan(0);
        hydra.Platform.Should().NotBeNullOrEmpty();
        hydra.Project.Should().NotBeNullOrEmpty();
        hydra.Jobset.Should().NotBeNullOrEmpty();
        hydra.Job.Should().NotBeNullOrEmpty();
        hydra.Path.Should().NotBeNull();
        hydra.DrvPath.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that the HydraPath sub-model within Hydra is correctly deserialized from search results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchPackages_VerifyHydraPathModel()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .WithQuery("hello")
            .Page(0, 10)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a package with Hydra path information
        NixPackage? packageWithHydra = response.Documents
            .FirstOrDefault(p => p.Hydra is { Length: > 0 } && p.Hydra[0].Path.Length > 0);

        Assert.SkipWhen(packageWithHydra == null, "No package with Hydra information found in the search results.");

        packageWithHydra.Should().NotBeNull();
        packageWithHydra.Hydra.Should().NotBeNull();

        HydraPath path = packageWithHydra.Hydra[0].Path[0];
        path.Should().NotBeNull();
        path.Output.Should().NotBeNullOrEmpty();
        path.Path.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that NixPackage inherits from NixFlake and can be accessed as such.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchPackages_VerifyFlakeProperties()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .WithQuery("hello")
            .Page(0, 10)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - NixPackage inherits from NixFlake
        NixPackage package = response.Documents.First();

        // FlakeDescription, FlakeResolved, FlakeName, FlakeRevision are optional
        // Just verify the package can be accessed as a NixFlake
        NixFlake flake = package;
        flake.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that searching with channel filters returns appropriate results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchPackages_WithFilters_ReturnsFilteredResults()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .WithQuery("python")
            .ForChannel(NixChannel.Unstable)
            .Page(0, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.IsValid.Should().BeTrue();
        response.Documents.Should().NotBeEmpty();
    }

    /// <summary>
    /// Tests that pagination works correctly and returns different results for different pages.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchPackages_WithPagination_ReturnsCorrectPage()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> firstPage = await this.Client
            .Packages()
            .WithQuery("python")
            .Page(0, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        ISearchResponse<NixPackage> secondPage = await this.Client
            .Packages()
            .WithQuery("python")
            .Page(5, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        firstPage.Documents.Should().NotBeEmpty();
        secondPage.Documents.Should().NotBeEmpty();
        firstPage.Documents.Should().NotIntersectWith(secondPage.Documents);
    }

    /// <summary>
    /// Tests that package description fields are populated correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchPackages_VerifyDescriptionFields()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .WithQuery("vim")
            .Page(0, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a package with description
        NixPackage? packageWithDescription = response.Documents
            .FirstOrDefault(p => !string.IsNullOrEmpty(p.Description));

        packageWithDescription.Should().NotBeNull();
        packageWithDescription.Description.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that package homepage URLs are populated correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchPackages_VerifyHomepageField()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .WithQuery("vim")
            .Page(0, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a package with homepage
        NixPackage? packageWithHomepage = response.Documents
            .FirstOrDefault(p => p.Homepage is { Length: > 0 });

        packageWithHomepage.Should().NotBeNull();
        packageWithHomepage.Homepage.Should().NotBeNull();
        packageWithHomepage.Homepage.Should().NotBeEmpty();
    }

    /// <summary>
    /// Tests that package position in nixpkgs repository is populated correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchPackages_VerifyPositionField()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .WithQuery("hello")
            .Page(0, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a package with position
        NixPackage? packageWithPosition = response.Documents
            .FirstOrDefault(p => !string.IsNullOrEmpty(p.Position));

        packageWithPosition.Should().NotBeNull();
        packageWithPosition.Position.Should().NotBeNullOrEmpty();
    }
}
