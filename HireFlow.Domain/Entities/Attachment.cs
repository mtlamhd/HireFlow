using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;

namespace HireFlow.Domain.Entities;

public class Attachment : BaseEntity, IValidatableEntity
{
    public string FileName { get; private set; }

    public string? FilePath { get; private set; }

    public string ContentType { get; private set; }

    public long? FileSize { get; private set; }

    public byte[]? Data { get; private set; }


    private Attachment() { }


    public Attachment(
        string fileName,
        string contentType,
        string? filePath = null,
        long? fileSize = null,
        byte[]? data = null)
    {
        FileName = fileName;
        FilePath = filePath;
        ContentType = contentType;
        FileSize = fileSize;
        Data = data;
        
        Validate();
    }


    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(FileName))
            throw new ValidationException(
                "File name is required.",
                1001);

        if (string.IsNullOrWhiteSpace(ContentType))
            throw new ValidationException(
                "Content type is required.",
                1002);

        if (FileSize.HasValue && FileSize <= 0)
            throw new ValidationException(
                "File size must be greater than zero.",
                1003);

        if (string.IsNullOrWhiteSpace(FilePath) && Data is null)
            throw new ValidationException(
                "Either file path or file data must be provided.",
                1004);
    }
}
