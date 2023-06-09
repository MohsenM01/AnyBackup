using FluentFTP;
using Microsoft.Extensions.Options;

public interface IFileService
{
    Task DoAsync();
}

public class FileService : IFileService
{

    private readonly Settings _settings;
    private const long OneGigiaBitInBit = 1024 * 1024 * 1024;
    public FileService(IOptions<Settings> settings)
    {
        _settings = settings.Value;
    }

    public async Task DoAsync()
    {
        foreach (var rule in _settings.Rules)
        {
            await DoRuleAsyn(rule);
        }
    }

    private async Task DoRuleAsyn(Rule rule)
    {
        try
        {
            var client = new AsyncFtpClient(rule.DestinationHost)
            {
                Credentials = new System.Net.NetworkCredential(rule.DestinationUserName, rule.DestinationPassword)
            };

            client.Config.EncryptionMode = FtpEncryptionMode.Auto;
            client.Config.ValidateAnyCertificate = true;

            await client.AutoConnect();

            var destinationFiles = await client.GetListing(rule.DestinationPath);
            long fileSizes = destinationFiles.Where(a => a.Type == FtpObjectType.File).Sum(a => a.Size);

            if (fileSizes / OneGigiaBitInBit > rule.DeleteOldFilesAfterExceedFileSizesInGb)
            {
                destinationFiles = destinationFiles.OrderBy(a => a.Modified).ToArray();

                foreach (FtpListItem item in destinationFiles)
                {
                    await client.DeleteFile(item.FullName);

                    if (item.Type == FtpObjectType.File)
                        fileSizes -= item.Size;

                    if (fileSizes / (1024 * 1024 * 1024) <= rule.DeleteOldFilesAfterExceedFileSizesInGb)
                        break;
                }
            }

            await client.UploadDirectory(rule.SourcePath, rule.DestinationPath, FtpFolderSyncMode.Update);
            await client.Disconnect();
        }
        catch (Exception ex)
        {
            await File.AppendAllTextAsync(_settings.LogFilePath, ex.Message + Environment.NewLine, System.Text.Encoding.UTF8);
            await File.AppendAllTextAsync(_settings.LogFilePath, ex.InnerException?.Message + Environment.NewLine, System.Text.Encoding.UTF8);
        }

    }
}