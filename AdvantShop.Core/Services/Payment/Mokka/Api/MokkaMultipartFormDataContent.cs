using System.IO;
using System.Net;
using System.Net.Http;

namespace AdvantShop.Core.Services.Payment.Mokka.Api
{
    public class MokkaMultipartFormDataContent : MultipartFormDataContent
    {
        public MokkaMultipartFormDataContent() : base() { }
        public MokkaMultipartFormDataContent(string boundary) : base(boundary) { }

        public void SerializeToStream(Stream stream, TransportContext context)
        {
            base.SerializeToStreamAsync(stream, context).Wait();
        } 
    }
}