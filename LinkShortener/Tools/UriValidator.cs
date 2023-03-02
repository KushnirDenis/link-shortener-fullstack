namespace LinkShortener.Tools;

public static class UriValidator
{
    public static bool ValidateUri(string uri)
    {
        return Uri.TryCreate(uri, UriKind.Absolute, out Uri? validatedUri)
               && (validatedUri.Scheme == Uri.UriSchemeHttp
                   || validatedUri.Scheme == Uri.UriSchemeHttps
                   || validatedUri.Scheme == Uri.UriSchemeFtp
                   || validatedUri.Scheme == Uri.UriSchemeMailto);
    }
}