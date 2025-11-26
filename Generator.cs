public static class Generator
{
    /// <summary>
    /// Creates multiple directories, each with N text files.
    /// </summary>
    /// <param name="basePath">The root directory where new folders will be created.</param>
    /// <param name="folderCount">Number of folders to create.</param>
    /// <param name="filesPerFolder">Number of text files per folder.</param>
     
    public static void Generate(
        string basePath,
        int folderCount,
        int filesPerFolder)
    {
        if (!Directory.Exists(basePath))
            Directory.CreateDirectory(basePath);
        for (int i = 1; i <= folderCount; i++)
        {
            string folderPath = Path.Combine(basePath, $"Folder{i}");
            Directory.CreateDirectory(folderPath);
            Console.WriteLine("Created directory: " + folderPath);

            for (int j = 1; j <= filesPerFolder; j++)
            {
                string filePath = Path.Combine(folderPath, $"file{j}.txt");
                File.WriteAllText(filePath, $"This is file {j} inside folder {i}.");
                Console.WriteLine("  Created file: " + filePath);
            }
        }
        Console.WriteLine("Finished generating all directories and files.");
    }
}