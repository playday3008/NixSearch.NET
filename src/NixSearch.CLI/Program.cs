// SPDX-License-Identifier: MIT

using System.CommandLine;

using NixSearch.CLI;

RootCommand rootCommand = new("NixSearch CLI - Search for NixOS packages and options")
{
    PackagesCommand.Create(),
    OptionsCommand.Create(),
};

return rootCommand.Parse(args).Invoke();
