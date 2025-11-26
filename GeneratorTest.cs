public static class GeneratorTest
{
    public static void Run()
    {
        string basePath = @"C:\GeneratedMultiFolders";

        // Generate folderCount number of folders, each with filesPerFolder number of files
        Generator.Generate(
            basePath,
            folderCount: 3,
            filesPerFolder: 4
        );
    }
}