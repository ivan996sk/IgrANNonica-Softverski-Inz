using api.Models;

namespace api.Services
{
    public interface IFileService
    {
        FileModel Create(FileModel file);
        string GetFilePath(string id, string uploaderId);
        public FileModel getFile(string id);
        bool CheckDb();
        string GetFileId(string fullPath);
        bool CopyFile(string sourceFile, string destinacionFile);

    }
}