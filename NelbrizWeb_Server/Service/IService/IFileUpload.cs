using Microsoft.AspNetCore.Components.Forms;

namespace NelbrizWeb_Server.Service.IService
{
    public interface IFileUpload
    {
        Task<string> UploadFile(IBrowserFile file);
        Task<bool> DeleteFile(string filePath);
    }
}
