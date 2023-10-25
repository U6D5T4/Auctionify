namespace Auctionify.Application.Common.Models.Blob
{
    public class BlobInfo
    {
        public BlobInfo(Stream Content, string ContentType)
        {
            this.Content = Content;
            this.ContentType = ContentType;
        }

        public Stream Content { get; }
        public string ContentType { get; }
    }
}
