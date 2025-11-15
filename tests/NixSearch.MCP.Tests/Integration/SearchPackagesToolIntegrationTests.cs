// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using NixSearch.Core.Models;
using NixSearch.MCP.Models;

namespace NixSearch.MCP.Tests.Integration;

/// <summary>
/// Integration tests for <see cref="Tools.SearchPackagesTool"/>.
/// </summary>
public class SearchPackagesToolIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests that SearchPackages with a common package name returns results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchPackages_WithCommonPackage_ShouldReturnResults()
    {
        // Act
        SearchResponse<NixPackage> result = await this.SearchPackagesTool.SearchPackages(
            "firefox",
            size: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Total.Should().BeGreaterThan(0);
        result.Results.Should().NotBeEmpty();
        result.Results.Should().Contain(p => p.Name
            .Contains("firefox", System.StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Tests that SearchPackages with pagination works correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchPackages_WithPagination_ShouldReturnCorrectPage()
    {
        // Act
        SearchResponse<NixPackage> page0 = await this.SearchPackagesTool.SearchPackages(
            "python",
            page: 0,
            size: 5,
            cancellationToken: TestContext.Current.CancellationToken);
        SearchResponse<NixPackage> page1 = await this.SearchPackagesTool.SearchPackages(
            "python",
            page: 1,
            size: 5,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        page0.Should().NotBeNull();
        page0.Results.Should().HaveCount(5);
        page0.Page.Should().Be(0);

        page1.Should().NotBeNull();
        page1.Results.Should().HaveCount(5);
        page1.Page.Should().Be(1);

        // Results should be different
        page0.Results[0].AttrName.Should().NotBe(page1.Results[0].AttrName);
    }

    /// <summary>
    /// Tests that SearchPackages with platform filter works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchPackages_WithPlatformFilter_ShouldReturnFilteredResults()
    {
        // Act
        SearchResponse<NixPackage> result = await this.SearchPackagesTool.SearchPackages(
            "git",
            platform: ["x86_64-linux"],
            size: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().NotBeEmpty();
        result.Results.Should().OnlyContain(p => p.Platforms.Contains("x86_64-linux"));
    }

    /// <summary>
    /// Tests that SearchPackages with stable channel works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchPackages_WithStableChannel_ShouldReturnResults()
    {
        // Act
        SearchResponse<NixPackage> result = await this.SearchPackagesTool.SearchPackages(
            "bash",
            channel: "stable",
            size: 5,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().NotBeEmpty();
    }

    /// <summary>
    /// Tests that SearchPackages with nonexistent package returns empty.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchPackages_WithNonexistentPackage_ShouldReturnEmpty()
    {
        // Act
        SearchResponse<NixPackage> result = await this.SearchPackagesTool.SearchPackages(
            "this-package-definitely-does-not-exist-xyz123",
            size: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Total.Should().Be(0);
        result.Results.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that SearchPackages with packageSet filter works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchPackages_WithPackageSetFilter_ShouldReturnFilteredResults()
    {
        // Act
        SearchResponse<NixPackage> result = await this.SearchPackagesTool.SearchPackages(
            "zfs",
            packageSet: ["linuxKernel"],
            size: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().NotBeEmpty();
        result.Results.Should().Contain(p => p.AttrSet == "linuxKernel");
    }

    /// <summary>
    /// Tests that SearchPackages returns all expected fields.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchPackages_ShouldReturnAllFields()
    {
        // Act
        SearchResponse<NixPackage> result = await this.SearchPackagesTool.SearchPackages(
            "nginx",
            size: 1,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().NotBeEmpty();

        NixPackage package = result.Results[0];
        package.AttrName.Should().NotBeNullOrEmpty();
        package.Name.Should().NotBeNullOrEmpty();
        package.Version.Should().NotBeNullOrEmpty();
        package.System.Should().NotBeNullOrEmpty();
        package.Platforms.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that SearchPackages with license filter works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchPackages_WithLicenseFilter_ShouldReturnFilteredResults()
    {
        // Act
        SearchResponse<NixPackage> result = await this.SearchPackagesTool.SearchPackages(
            "bash",
            license: ["gpl"],
            size: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();

        // If results are returned, they should match the license filter
        if (result.Results.Count > 0)
        {
            result.Results.Should().OnlyContain(p =>
                p.License != null &&
                p.License.Any(l => l.FullName != null && l.FullName.Contains("GPL", System.StringComparison.OrdinalIgnoreCase)));
        }
    }

    /// <summary>
    /// Tests that SearchPackages with maintainer filter works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchPackages_WithMaintainerFilter_ShouldReturnFilteredResults()
    {
        // Act
        SearchResponse<NixPackage> result = await this.SearchPackagesTool.SearchPackages(
            "vim",
            maintainer: ["viric"],
            size: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();

        // If results are returned, they should match the maintainer filter
        if (result.Results.Count > 0)
        {
            result.Results.Should().OnlyContain(p =>
                p.Maintainers != null &&
                p.Maintainers.Any(m => m.Name == "viric" || (m.Email != null && m.Email.Contains("viric"))));
        }
    }

    /// <summary>
    /// Tests that SearchPackages with team filter works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchPackages_WithTeamFilter_ShouldReturnFilteredResults()
    {
        // Act
        SearchResponse<NixPackage> result = await this.SearchPackagesTool.SearchPackages(
            "systemd",
            team: ["freedesktop"],
            size: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();

        // If results are returned, verify the filter was applied (Teams property should be populated)
        if (result.Results.Count > 0)
        {
            result.Results.Should().OnlyContain(p =>
                p.Teams != null &&
                p.Teams.Any(t => t.ShortName != null && t.ShortName.Contains("freedesktop", System.StringComparison.OrdinalIgnoreCase)));
        }
    }
}