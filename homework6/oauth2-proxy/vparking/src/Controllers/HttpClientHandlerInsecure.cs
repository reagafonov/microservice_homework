class HttpClientHandlerInsecure : HttpClientHandler
{
    public HttpClientHandlerInsecure()
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
    }
}