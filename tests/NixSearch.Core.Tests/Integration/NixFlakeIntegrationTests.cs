// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Nest;

using NixSearch.Core.Models;
using NixSearch.Core.Models.Flake;
using NixSearch.Core.Search;

namespace NixSearch.Core.Tests.Integration;

/// <summary>
/// Integration tests for NixFlake and flake search using real backend.
/// </summary>
public sealed class NixFlakeIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests that searching for flakes with a query returns valid results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchFlakes_WithQuery_ReturnsResults()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .ForChannel(NixChannel.Flakes)
            .WithQuery(".")
            .Page(0, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.IsValid.Should().BeTrue();
        response.Documents.Should().NotBeEmpty();
    }

    /// <summary>
    /// Tests that flake properties are populated correctly in search results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchFlakes_VerifyFlakeProperties()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .ForChannel(NixChannel.Flakes)
            .WithQuery(".")
            .Page(0, 50)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a flake with properties populated
        NixPackage? flakePackage = response.Documents
            .FirstOrDefault(p => !string.IsNullOrEmpty(p.FlakeName));

        Assert.SkipWhen(flakePackage == null, "No flake packages with populated properties found in the search results.");

        flakePackage.Should().NotBeNull();
        flakePackage.FlakeName.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that flake description field is populated correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchFlakes_VerifyFlakeDescription()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .ForChannel(NixChannel.Flakes)
            .WithQuery(".")
            .Page(0, 50)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a flake with description
        NixPackage? flakeWithDescription = response.Documents
            .FirstOrDefault(p => !string.IsNullOrEmpty(p.FlakeDescription));

        Assert.SkipWhen(flakeWithDescription == null, "No flake packages with descriptions found in the search results.");

        flakeWithDescription.Should().NotBeNull();
        flakeWithDescription.FlakeDescription.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that flake revision field is populated correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchFlakes_VerifyFlakeRevision()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .ForChannel(NixChannel.Flakes)
            .WithQuery(".")
            .Page(0, 50)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a flake with revision
        NixPackage? flakeWithRevision = response.Documents
            .FirstOrDefault(p => !string.IsNullOrEmpty(p.FlakeRevision));

        Assert.SkipWhen(flakeWithRevision == null, "No flake packages with revisions found in the search results.");

        flakeWithRevision.Should().NotBeNull();
        flakeWithRevision.FlakeRevision.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that the Repo model is correctly deserialized from flake search results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchFlakes_VerifyRepoModel()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .ForChannel(NixChannel.Flakes)
            .WithQuery(".")
            .Page(0, 50)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a flake with resolved repository
        NixPackage? flakeWithRepo = response.Documents
            .FirstOrDefault(p => p.FlakeResolved != null);

        Assert.SkipWhen(flakeWithRepo == null, "No flake packages with resolved repositories found in the search results.");

        flakeWithRepo.Should().NotBeNull();
        flakeWithRepo.FlakeResolved.Should().NotBeNull();

        Repo repo = flakeWithRepo.FlakeResolved;
        repo.Type.Should().BeDefined();
    }

    /// <summary>
    /// Tests that Git repository type can be accessed and converted correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchFlakes_VerifyGitRepoType()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .ForChannel(NixChannel.Flakes)
            .WithQuery("archivewebpage")
            .Page(0, 10)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a flake with Git repository (if any exists)
        NixPackage? flakeWithGit = response.Documents
            .FirstOrDefault(p => p.FlakeResolved?.Type == RepoType.Git);

        Assert.SkipWhen(flakeWithGit == null, "No flake packages with Git repositories found in the search results.");

        flakeWithGit.Should().NotBeNull();
        flakeWithGit.FlakeResolved.Should().NotBeNull();
        flakeWithGit.FlakeResolved.Type.Should().Be(RepoType.Git);

        GitRepo gitRepo = flakeWithGit.FlakeResolved.WithType<GitRepo>();
        gitRepo.Should().NotBeNull();
        gitRepo.Url.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that GitHub repository type can be accessed and converted correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchFlakes_VerifyGitHubRepoType()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .ForChannel(NixChannel.Flakes)
            .WithQuery(".")
            .Page(0, 50)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a flake with GitHub repository
        NixPackage? flakeWithGitHub = response.Documents
            .FirstOrDefault(p => p.FlakeResolved?.Type == RepoType.GitHub);

        Assert.SkipWhen(flakeWithGitHub == null, "No flake packages with GitHub repositories found in the search results.");

        flakeWithGitHub.Should().NotBeNull();
        flakeWithGitHub.FlakeResolved.Should().NotBeNull();
        flakeWithGitHub.FlakeResolved.Type.Should().Be(RepoType.GitHub);

        ForgeRepo gitHubRepo = flakeWithGitHub.FlakeResolved.WithType<ForgeRepo>();
        gitHubRepo.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that GitLab repository type can be accessed and converted correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchFlakes_VerifyGitLabRepoType()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .ForChannel(NixChannel.Flakes)
            .WithQuery("neuropil")
            .Page(0, size: 10)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a flake with GitLab repository (if any exists)
        NixPackage? flakeWithGitLab = response.Documents
            .FirstOrDefault(p => p.FlakeResolved?.Type == RepoType.GitLab);

        Assert.SkipWhen(flakeWithGitLab == null, "No flake packages with GitLab repositories found in the search results.");

        flakeWithGitLab.Should().NotBeNull();
        flakeWithGitLab.FlakeResolved.Should().NotBeNull();
        flakeWithGitLab.FlakeResolved.Type.Should().Be(RepoType.GitLab);

        ForgeRepo gitLabRepo = flakeWithGitLab.FlakeResolved.WithType<ForgeRepo>();
        gitLabRepo.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that SourceHut repository type can be accessed and converted correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchFlakes_VerifySourceHutRepoType()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .ForChannel(NixChannel.Flakes)
            .WithQuery("rust_sway")
            .Page(0, 10)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a flake with SourceHut repository (if any exists)
        NixPackage? flakeWithSourceHut = response.Documents
            .FirstOrDefault(p => p.FlakeResolved?.Type == RepoType.SourceHut);

        Assert.SkipWhen(flakeWithSourceHut == null, "No flake packages with SourceHut repositories found in the search results.");

        flakeWithSourceHut.Should().NotBeNull();
        flakeWithSourceHut.FlakeResolved.Should().NotBeNull();
        flakeWithSourceHut.FlakeResolved.Type.Should().Be(RepoType.SourceHut);

        ForgeRepo sourceHutRepo = flakeWithSourceHut.FlakeResolved.WithType<ForgeRepo>();
        sourceHutRepo.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that repository URL field is populated correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchFlakes_VerifyRepoUrl()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .ForChannel(NixChannel.Flakes)
            .WithQuery("archivewebpage")
            .Page(0, 10)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a flake with repository URL
        NixPackage? flakeWithUrl = response.Documents
            .FirstOrDefault(p => !string.IsNullOrEmpty(p.FlakeResolved?.Url));

        Assert.SkipWhen(flakeWithUrl == null, "No flake packages with repository URLs found in the search results.");

        flakeWithUrl.Should().NotBeNull();
        flakeWithUrl.FlakeResolved.Should().NotBeNull();
        flakeWithUrl.FlakeResolved.Url.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that repository owner and name fields are populated correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchFlakes_VerifyRepoOwnerAndName()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .ForChannel(NixChannel.Flakes)
            .WithQuery(".")
            .Page(0, 50)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a flake with owner and repo name
        NixPackage? flakeWithRepoInfo = response.Documents
            .FirstOrDefault(p =>
                p.FlakeResolved != null &&
                !string.IsNullOrEmpty(p.FlakeResolved.Owner) &&
                !string.IsNullOrEmpty(p.FlakeResolved.RepoName));

        Assert.SkipWhen(flakeWithRepoInfo == null, "No flake packages with repository owner and name found in the search results.");

        flakeWithRepoInfo.Should().NotBeNull();
        flakeWithRepoInfo.FlakeResolved.Should().NotBeNull();
        flakeWithRepoInfo.FlakeResolved.Owner.Should().NotBeNullOrEmpty();
        flakeWithRepoInfo.FlakeResolved.RepoName.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that ForgeRepo base class properties are accessible for forge-based repositories.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchFlakes_VerifyForgeRepoProperties()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> response = await this.Client
            .Packages()
            .ForChannel(NixChannel.Flakes)
            .WithQuery(".")
            .Page(0, 50)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert - Find a flake with forge repository (GitHub, GitLab, or SourceHut)
        NixPackage? flakeWithForgeRepo = response.Documents
            .FirstOrDefault(p =>
                p.FlakeResolved?.Type == RepoType.GitHub ||
                p.FlakeResolved?.Type == RepoType.GitLab ||
                p.FlakeResolved?.Type == RepoType.SourceHut);

        Assert.SkipWhen(flakeWithForgeRepo == null, "No flake packages with forge repositories found in the search results.");

        flakeWithForgeRepo.Should().NotBeNull();
        flakeWithForgeRepo.FlakeResolved.Should().NotBeNull();

        // ForgeRepo is the base class for GitHub, GitLab, and SourceHut
        ForgeRepo forgeRepo = flakeWithForgeRepo.FlakeResolved.WithType<ForgeRepo>();
        forgeRepo.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that searching for flake options with a query works correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchFlakeOptions_WithQuery_ReturnsResults()
    {
        // Arrange & Act
        ISearchResponse<NixOption> response = await this.Client
            .Options()
            .ForChannel(NixChannel.Flakes)
            .WithQuery(".")
            .Page(0, 10)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.IsValid.Should().BeTrue();
        response.Documents.Should().NotBeEmpty();
    }

    /// <summary>
    /// Tests that pagination works correctly for flake search results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SearchFlakes_WithPagination_ReturnsCorrectPage()
    {
        // Arrange & Act
        ISearchResponse<NixPackage> firstPage = await this.Client
            .Packages()
            .ForChannel(NixChannel.Flakes)
            .WithQuery(".")
            .Page(0, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        ISearchResponse<NixPackage> secondPage = await this.Client
            .Packages()
            .ForChannel(NixChannel.Flakes)
            .WithQuery(".")
            .Page(5, 5)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        firstPage.Documents.Should().NotIntersectWith(secondPage.Documents);
    }
}
