using BeamOs.CodeGen.TestModelBuilderGenerator.TestModelGenerators;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");

var builder = new ConfigurationBuilder().AddUserSecrets<Program>();

IConfiguration configuration = builder.Build();

string speckleToken = configuration["SpeckleToken"];

TwistyBowlFramingGenerator twistyBowlFramingGenerator = new(speckleToken);
await twistyBowlFramingGenerator.Generate();
