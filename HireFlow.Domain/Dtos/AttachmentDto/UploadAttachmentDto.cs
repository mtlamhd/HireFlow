namespace HireFlow.Domain.Dtos.AttachmentDto;

public class UploadAttachmentDto
{
    
    public string FileName { get; set; } 
    public string ContentType { get; set; } 
    public byte[] Data { get; set; } 
}