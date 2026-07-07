using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;

namespace HireFlow.Domain.Entities;

public class Attachment : BaseEntity, IValidatableEntity
{
    public string FileName { get; private set; }

    public string FilePath { get; private set; }

    public string ContentType { get; private set; }

    public long FileSize { get; private set; }


    private Attachment() { }

    public Attachment(
        string fileName,
        string filePath,
        string contentType,
        long fileSize)
    {
        FileName = fileName;
        FilePath = filePath;
        ContentType = contentType;
        FileSize = fileSize;

        Validate();
    }


    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(FileName))
            throw new ValidationException(
                "File name is required.",
                1001);

        if (string.IsNullOrWhiteSpace(FilePath))
            throw new ValidationException(
                "File path is required.",
                1002);

        if (string.IsNullOrWhiteSpace(ContentType))
            throw new ValidationException(
                "Content type is required.",
                1003);

        if (FileSize <= 0)
            throw new ValidationException(
                "File size must be greater than zero.",
                1004);
    }
}