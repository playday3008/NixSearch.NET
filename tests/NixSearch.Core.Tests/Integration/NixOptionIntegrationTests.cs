// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Nest;

using NixSearch.Core.Models;
using NixSearch.Core.Search;

namespace NixSearch.Core.Tests.Integration;

/// <summary>
/// Integration tests for NixOption model using real backend.
/// </summary>
public sealed class NixOptionIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests that searching for options with a query returns valid results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchOptions_WithQuery_ReturnsResults()
    {
        // Arrange & Act
        ISearchResponse<NixOption> response = await this.Client
            .Options()
            .WithQuery("networking")
            .Page(0, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.IsValid.Should().BeTrue();
        response.Documents.Should().NotBeEmpty();
    }

    /// <summary>
    /// Tests that all required option properties are populated correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchOptions_VerifyOptionProperties()
    {
        // Arrange & Act
        ISearchResponse<NixOption> response = await this.Client
            .Options()
            .WithQuery("services.openssh")
            .Page(0, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        NixOption option = response.Documents.First();
        option.Should().NotBeNull();
        option.Name.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that option description field is populated correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchOptions_VerifyDescriptionField()
    {
        // Arrange & Act
        ISearchResponse<NixOption> response = await this.Client
            .Options()
            .WithQuery("services.openssh.enable")
            .Page(0, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find an option with description
        NixOption? optionWithDescription = response.Documents
            .FirstOrDefault(o => !string.IsNullOrEmpty(o.Description));

        optionWithDescription.Should().NotBeNull();
        optionWithDescription!.Description.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that option type field is populated correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchOptions_VerifyTypeField()
    {
        // Arrange & Act
        ISearchResponse<NixOption> response = await this.Client
            .Options()
            .WithQuery("services.openssh.enable")
            .Page(0, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find an option with type information
        NixOption? optionWithType = response.Documents
            .FirstOrDefault(o => !string.IsNullOrEmpty(o.Type));

        optionWithType.Should().NotBeNull();
        optionWithType!.Type.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that option default value field is populated correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchOptions_VerifyDefaultField()
    {
        // Arrange & Act
        ISearchResponse<NixOption> response = await this.Client
            .Options()
            .WithQuery("services")
            .Page(0, 20)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find an option with default value
        NixOption? optionWithDefault = response.Documents
            .FirstOrDefault(o => !string.IsNullOrEmpty(o.Default));

        optionWithDefault.Should().NotBeNull();
        optionWithDefault!.Default.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that option example value field is populated correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchOptions_VerifyExampleField()
    {
        // Arrange & Act
        ISearchResponse<NixOption> response = await this.Client
            .Options()
            .WithQuery("services")
            .Page(0, 20)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find an option with example value
        NixOption? optionWithExample = response.Documents
            .FirstOrDefault(o => !string.IsNullOrEmpty(o.Example));

        optionWithExample.Should().NotBeNull();
        optionWithExample!.Example.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that option source file location field is populated correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchOptions_VerifySourceField()
    {
        // Arrange & Act
        ISearchResponse<NixOption> response = await this.Client
            .Options()
            .WithQuery("services.openssh")
            .Page(0, 10)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find an option with source information
        NixOption? optionWithSource = response.Documents
            .FirstOrDefault(o => !string.IsNullOrEmpty(o.Source));

        optionWithSource.Should().NotBeNull();
        optionWithSource!.Source.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that option flake information field can be accessed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchOptions_VerifyFlakeField()
    {
        // Arrange & Act
        ISearchResponse<NixOption> response = await this.Client
            .Options()
            .ForChannel(NixChannel.Flakes)
            .WithQuery(".")
            .Page(0, 50)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find an option with flake information
        NixOption? optionWithFlake = response.Documents
            .FirstOrDefault(o => o.Flake != null && o.Flake.Match(f => f != null, s => s != null));

        Assert.SkipWhen(optionWithFlake == null, "No option with flake information found in the results.");

        optionWithFlake.Should().NotBeNull();
        optionWithFlake.Flake.Should().NotBeNull();

        optionWithFlake!.Flake!.Match(
            single => single.Should().NotBeNullOrEmpty(),
            multiple =>
            {
                multiple.Should().NotBeNull();
                multiple.Length.Should().BeGreaterThan(0);
            });
    }

    /// <summary>
    /// Tests that NixOption inherits from NixFlake and can be accessed as such.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchOptions_VerifyFlakeProperties()
    {
        // Arrange & Act
        ISearchResponse<NixOption> response = await this.Client
            .Options()
            .WithQuery("networking")
            .Page(0, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - NixOption inherits from NixFlake
        NixOption option = response.Documents.First();

        // FlakeDescription, FlakeResolved, FlakeName, FlakeRevision are optional
        // Just verify the option can be accessed as a NixFlake
        NixFlake flake = option;
        flake.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that searching with channel filters returns appropriate results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchOptions_WithChannel_ReturnsResults()
    {
        // Arrange & Act
        ISearchResponse<NixOption> response = await this.Client
            .Options()
            .WithQuery("services.nginx")
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
    public async Task SearchOptions_WithPagination_ReturnsCorrectPage()
    {
        // Arrange & Act
        ISearchResponse<NixOption> firstPage = await this.Client
            .Options()
            .WithQuery("services")
            .Page(0, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        ISearchResponse<NixOption> secondPage = await this.Client
            .Options()
            .WithQuery("services")
            .Page(5, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        firstPage.Documents.Should().NotBeEmpty();
        secondPage.Documents.Should().NotBeEmpty();
        firstPage.Documents.Should().NotIntersectWith(secondPage.Documents);
    }

    /// <summary>
    /// Tests that multiple option fields are populated together in comprehensive results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchOptions_VerifyAllFieldsTogether()
    {
        // Arrange & Act
        ISearchResponse<NixOption> response = await this.Client
            .Options()
            .WithQuery("services.openssh")
            .Page(0, 10)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a comprehensive option with multiple fields populated
        NixOption? comprehensiveOption = response.Documents
            .FirstOrDefault(o =>
                !string.IsNullOrEmpty(o.Name) &&
                !string.IsNullOrEmpty(o.Description) &&
                !string.IsNullOrEmpty(o.Type));

        comprehensiveOption.Should().NotBeNull();
        comprehensiveOption!.Name.Should().NotBeNullOrEmpty();
        comprehensiveOption.Description.Should().NotBeNullOrEmpty();
        comprehensiveOption.Type.Should().NotBeNullOrEmpty();
    }
}
