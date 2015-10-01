using EPiServer.Core;

namespace TestSite.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}
