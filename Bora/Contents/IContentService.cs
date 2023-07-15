namespace Bora.Contents
{
    public interface IContentService
    {
        Task UpdateAsync(string email, ContentInput contentInput);
    }
}
