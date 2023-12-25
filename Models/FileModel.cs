#pragma warning disable
public class FileModel
{
    public string Description { get; set; }

    public byte[] File { get; set; }

    public string Filename { get; set; }

    public object InputStream { get; set; }

    public bool Open { get; set; }

    public bool Readable { get; set; }

    public UriProperties Uri { get; set; }

    public UrlProperties Url { get; set; }

    public bool Absolute { get; set; }

    public string AbsoluteFile { get; set; }

    public string AbsolutePath { get; set; }

    public string CanonicalFile { get; set; }

    public string CanonicalPath { get; set; }

    public string Directory { get; set; }

    public long FreeSpace { get; set; }

    public bool Hidden { get; set; }

    public string Name { get; set; }

    public string Parent { get; set; }

    public string Path { get; set; }

    public long TotalSpace { get; set; }

    public long UsableSpace { get; set; }

}

public class UriProperties
{
    public string Absolute { get; set; }

    public string Authority { get; set; }

    public string Fragment { get; set; }

    public string Host { get; set; }

    public bool Opaque { get; set; }

    public string Path { get; set; }

    public int? Port { get; set; }

    public string Query { get; set; }

    public string RawAuthority { get; set; }

    public string RawFragment { get; set; }

    public string RawPath { get; set; }

    public string RawQuery { get; set; }

    public string RawSchemeSpecificPart { get; set; }

    public string RawUserInfo { get; set; }

    public string Scheme { get; set; }

    public string SchemeSpecificPart { get; set; }

    public string UserInfo { get; set; }

}

public class UrlProperties
{
    public string Authority { get; set; }

    public object Content { get; set; }

    public int? DefaultPort { get; set; }

    public string File { get; set; }

    public string Host { get; set; }

    public string Path { get; set; }

    public string Port { get; set; }

    public string Protocol { get; set; }

    public string Query { get; set; }

    public string Ref { get; set; }

    public string UserInfo { get; set; }
}
