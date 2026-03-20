using System;
using System.IO;

namespace RPGMakerDecrypter.MVMZ
{
    public abstract class ProjectReconstructor
    {
        // Directories that should exist in the project directory
        private readonly string[] _directories = {
            "audio",
            "css",
            "data",
            "effects",
            "fonts",
            "icon",
            "img",
            "js",
            "movies"
        };

        // Files that should exist in the project directory
        private readonly string[] _files =
        {
            "index.html",
            "package.json"
        };

        protected abstract void CreateProjectFile(string outputPath);

        private string[] ParseString(string input)
        {
            string[] split = input.Split(',');
            string[] formatted = new string[split.Length];

            for (int i = 0; i < split.Length; i++)
            {
                string formattedText = split[i].Replace(@"\s+", "");
                formatted[i] = formattedText;
            }

            return formatted;
        }
        
        public virtual void Reconstruct(string deploymentPath, string outputPath, string customDirectories, string customFiles)
        {
            string[] directoriesToCopy = customDirectories != null ? ParseString(customDirectories) : _directories;
            string[] filesToCopy = customFiles != null ? ParseString(customFiles) : _files;

            if (Directory.Exists(outputPath))
            {
                Directory.Delete(outputPath, true);
                Directory.CreateDirectory(outputPath);
            }

            foreach (var directory in directoriesToCopy)
            {
                if (!Directory.Exists(Path.Combine(deploymentPath, directory))) {
                    continue;
                }

                CopyDirectory(Path.Combine(deploymentPath, directory), Path.Combine(outputPath, directory));
            }
            
            foreach (var file in filesToCopy)
            {
                File.Copy(Path.Combine(deploymentPath, file), Path.Combine(outputPath, file));
            }
            
            CreateProjectFile(outputPath);
        }

        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(sourceDir))
            {
                return;
            }
            
            Directory.CreateDirectory(destinationDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var destFilePath = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFilePath);
            }

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                var destDirectoryPath = Path.Combine(destinationDir, Path.GetFileName(directory));
                CopyDirectory(directory, destDirectoryPath);
            }
        }
    }
}