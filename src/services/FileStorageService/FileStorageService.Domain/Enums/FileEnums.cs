namespace FileStorageService.Domain.Enums;

public enum StorageProvider
{
    Local   = 0,
    Azure   = 1, // Azure Blob Storage
    Aws     = 2, // AWS S3
    Google  = 3  // Google Cloud Storage
}

public enum FileCategory
{
    LeadAttachment    = 0,
    DealContract      = 1,
    ContactPhoto      = 2,
    AiModelFile       = 3,
    ProfilePicture    = 4,
    Reports           = 5
}
