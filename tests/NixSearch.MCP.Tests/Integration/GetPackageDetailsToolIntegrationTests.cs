// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System.Threading.Tasks;

using FluentAssertions;

using NixSearch.MCP.Models;

namespace NixSearch.MCP.Tests.Integration;

/// <summary>
/// Integration tests for <see cref="Tools.GetPackageDetailsTool"/>.
/// </summary>
public class GetPackageDetailsToolIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests that GetPackageDetails with exact package name returns package.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task GetPackageDetails_WithExactPackageName_ShouldReturnPackage()
    {
        // Act
        PackageResult? result = await this.GetPackageDetailsTool.GetPackageDetails(
            "firefox",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.AttrName.Should().Be("firefox");
        result.Name.Should().Be("firefox");
        result.Version.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that GetPackageDetails with nonexistent package returns null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task GetPackageDetails_WithNonexistentPackage_ShouldReturnNull()
    {
        // Act
        PackageResult? result = await this.GetPackageDetailsTool.GetPackageDetails(
            "this-package-definitely-does-not-exist-xyz123",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Tests that GetPackageDetails with stable channel works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task GetPackageDetails_WithStableChannel_ShouldReturnPackage()
    {
        // Act
        PackageResult? result = await this.GetPackageDetailsTool.GetPackageDetails(
            "zsh",
            channel: "stable",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("zsh");
    }

    /// <summary>
    /// Tests that GetPackageDetails returns all expected fields.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task GetPackageDetails_ShouldReturnAllFields()
    {
        // Act
        PackageResult? result = await this.GetPackageDetailsTool.GetPackageDetails(
            "git",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.AttrName.Should().NotBeNullOrEmpty();
        result.AttrSet.Should().NotBeNullOrEmpty();
        result.Name.Should().NotBeNullOrEmpty();
        result.Version.Should().NotBeNullOrEmpty();
        result.System.Should().NotBeNullOrEmpty();
        result.Platforms.Should().NotBeNull();
        result.LicenseNames.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that GetPackageDetails with package in package set works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task GetPackageDetails_WithPackageInPackageSet_ShouldReturnPackage()
    {
        // Act
        PackageResult? result = await this.GetPackageDetailsTool.GetPackageDetails(
            "kodiPackages.requests",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.AttrName.Should().Be("kodiPackages.requests");
        result.AttrSet.Should().Be("kodiPackages");
        result.Name.Should().Be("kodi-requests");
    }
}