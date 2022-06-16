using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Helpers
{
    public static class FileManager
    {
        public static string Save(string root, string folder, IFormFile file)
        {
            string newFileImage = Guid.NewGuid().ToString() + (file.FileName.Length > 64 ? file.FileName.Substring(file.FileName.Length - 64,64):file.FileName);
            string path = Path.Combine(root, folder, newFileImage);
            using(FileStream stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return newFileImage;
        }

        public static void Delete(string root, string folder, string image)
        {
            string path = Path.Combine(root, folder, image);
            if (File.Exists(path))
                File.Delete(path);
        }    
    }
}
