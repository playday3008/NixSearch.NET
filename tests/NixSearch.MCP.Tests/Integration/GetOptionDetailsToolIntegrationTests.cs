// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System.Threading.Tasks;

using FluentAssertions;

using NixSearch.Core.Models;

namespace NixSearch.MCP.Tests.Integration;

/// <summary>
/// Integration tests for <see cref="Tools.GetOptionDetailsTool"/>.
/// </summary>
public class GetOptionDetailsToolIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests that GetOptionDetails with exact option name returns option.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task GetOptionDetails_WithExactOptionName_ShouldReturnOption()
    {
        // Act
        NixOption? result = await this.GetOptionDetailsTool.GetOptionDetails(
            "services.openssh.enable",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("services.openssh.enable");
    }

    /// <summary>
    /// Tests that GetOptionDetails with nonexistent option returns null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task GetOptionDetails_WithNonexistentOption_ShouldReturnNull()
    {
        // Act
        NixOption? result = await this.GetOptionDetailsTool.GetOptionDetails(
            "this.option.definitely.does.not.exist.xyz123",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Tests that GetOptionDetails with stable channel works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task GetOptionDetails_WithStableChannel_ShouldReturnOption()
    {
        // Act
        NixOption? result = await this.GetOptionDetailsTool.GetOptionDetails(
            "networking.hostName",
            channel: "stable",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("networking.hostName");
    }

    /// <summary>
    /// Tests that GetOptionDetails returns expected fields.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task GetOptionDetails_ShouldReturnExpectedFields()
    {
        // Act
        NixOption? result = await this.GetOptionDetailsTool.GetOptionDetails(
            "boot.loader.grub.enable",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().NotBeNullOrEmpty();
        result.Name.Should().Be("boot.loader.grub.enable");
    }

    /// <summary>
    /// Tests that GetOptionDetails is case-insensitive.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task GetOptionDetails_ShouldBeCaseInsensitive()
    {
        // Act
        NixOption? resultLower = await this.GetOptionDetailsTool.GetOptionDetails(
            "services.nginx.enable",
            cancellationToken: TestContext.Current.CancellationToken);
        NixOption? resultUpper = await this.GetOptionDetailsTool.GetOptionDetails(
            "SERVICES.NGINX.ENABLE",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        resultLower.Should().NotBeNull();
        resultUpper.Should().NotBeNull();

        // Both should return the same option (case-insensitive search)
        resultLower!.Name.Should().BeEquivalentTo(resultUpper!.Name);
    }

    /// <summary>
    /// Tests that GetOptionDetails with common service option works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task GetOptionDetails_WithCommonServiceOption_ShouldReturnOption()
    {
        // Act
        NixOption? result = await this.GetOptionDetailsTool.GetOptionDetails(
            "services.postgresql.enable",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("services.postgresql.enable");
        result.Type.Should().NotBeNullOrEmpty();
    }
}