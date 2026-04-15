record DownloadItem(string Repo, string File)
{
    public string Url => $"https://huggingface.co/{Repo}/resolve/main/{File}";
    public string ShaUrl => $"{Url}.sha256";
}
