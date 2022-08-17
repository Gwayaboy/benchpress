﻿using Generators;
using Generators.LanguageProviders;
using Generators.ResourceTypes;
using System.CommandLine;
using System.Linq;

var fileOption = new Option<FileInfo?>(name: "--import", description: "The bicep file to import and scaffold tests for");

var languageProviderOption = new Option<LanguageProviderOptions>(name: "--provider", description: "Language provider that will be used to generate test files");

var rootCommand = new RootCommand("Test Generator for Bicep and ARM Templates");

rootCommand.AddOption(fileOption);
rootCommand.AddOption(languageProviderOption);

rootCommand.SetHandler((fileInfo, languageProvider) =>
  {
    if (fileInfo is null) return;
    if (languageProvider == LanguageProviderOptions.Undefined) return;

    ILanguageProvider provider = languageProvider switch
    {
      LanguageProviderOptions.Powershell => new PowershellLanguageProvider(),
      _ => throw new NotImplementedException(),
    };

    var generator = new TestGenerator(provider);

    var metadataList = AzureDeploymentImporter.Import(fileInfo);

    var testList = new List<TestDefinition>();
    var testGroups = new List<IEnumerable<TestDefinition>>();

    foreach (var metadata in metadataList)
    {
      testList.Add(new TestDefinition(metadata, TestType.ResourceExists));
      testList.Add(new TestDefinition(metadata, TestType.Location));
    }

    AppDomain.CurrentDomain.GetAssemblies()
      .SelectMany(domainAssembly => domainAssembly.GetTypes())
      .Where(type => typeof(ResourceType).IsAssignableFrom(type) && !type.IsAbstract)
      .ToList()
      .ForEach(type =>
      {
        testGroups.Add(testList.Where(t => t.Metadata.ResourceType.GetType() == type));
      });

    testGroups.Add(testList.Where(t => t.Metadata.ResourceType.GetType() == typeof(ResourceGroup)));

    foreach (var group in testGroups)
    {
      if (!group.Any()) continue;

      var testsOutput = generator.Generate(group, "./templates/powershell/template.ps1");

      var testFileName = group.First().Metadata.ResourceType.Prefix + "Tests.ps1";
      var testFilePath = "output";

      var testFileFullName = Path.Join(testFilePath, testFileName);

      if (!Directory.Exists(testFilePath))
      {
        Directory.CreateDirectory(testFilePath);
      }

      File.WriteAllText(testFileFullName, testsOutput);
    }
  },
  fileOption, languageProviderOption
);

await rootCommand.InvokeAsync(args);

public enum LanguageProviderOptions
{
  Undefined,
  Powershell,
  NodeJs,
}
