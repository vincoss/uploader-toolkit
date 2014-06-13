using System;
using System.Linq;
using System.Collections.Generic;


namespace UploaderWebSite.Services
{
    public static class AllowedExtensionList
    {
        private readonly static List<string> Extensions = new List<string>();

        static AllowedExtensionList()
        {
            LoadExtensions();
        }

        public static void Add(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentNullException("extension");
            }
            if (!Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                Extensions.Add(extension);
            }
        }

        public static bool Exists(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentException("extension");
            }
            return Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        private static void LoadExtensions()
        {
            // TODO: Load extensions form config file / text file or other source.
            // Possible to add ExtensionSourceProvider

            Extensions.Add("jpg");
            Extensions.Add("png");
            Extensions.Add("gif");
            Extensions.Add("txt");
            Extensions.Add("docx");
            Extensions.Add("msi");
        }
    }
}