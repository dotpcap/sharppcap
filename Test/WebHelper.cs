using System.Net;

namespace Test
{

    /// <summary>
    ///  cloudflair has a website at this address, should be accessible from any
    /// computer with a connection to the Internet
    /// </summary>
    internal static class WebHelper
    {
        public static readonly IPAddress Address = IPAddress.Parse("1.1.1.1");

        public static WebExceptionStatus WebFetch()
        {
            try
            {
                var req = WebRequest.Create("http://" + Address.ToString());
                req.Timeout = 3000;
                req.GetResponse();
                return WebExceptionStatus.Success;
            }
            catch (WebException we)
            {
                return we.Status;
            }
        }
    }
}
