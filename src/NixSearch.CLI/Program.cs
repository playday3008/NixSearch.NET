// SPDX-License-Identifier: MIT

using System.CommandLine;

namespace NixSearch.CLI;

/// <summary>
/// Main program entry point.
/// </summary>
public static class Program
{
    /// <summary>
    /// Main entry point for the CLI application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>Exit code.</returns>
    public static int Main(string[] args)
    {
        RootCommand rootCommand = new("NixSearch CLI - Search for NixOS packages and options")
        {
            PackagesCommand.Create(),
            OptionsCommand.Create(),
        };

        return rootCommand.Parse(args).Invoke();
    }
}