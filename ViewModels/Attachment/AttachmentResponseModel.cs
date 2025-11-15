// File: ViewModels/Attachment/AttachmentResponseModel.cs
using System;

namespace ExaminationSystem.ViewModels.Attachment
{
    public class AttachmentResponseModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
    }
}