namespace HireFlow.Domain.Dtos.AttachmentDto;

public class AttachmentFileDto
{
    public byte[] Data { get; set; } 
    public string ContentType { get; set; } 
    public string FileName { get; set; } 
}