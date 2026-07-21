namespace WarbandOfTheSpiritborn.Services
{
    public interface IHtmlSanitizationService
    {
        string SanitizeAbout(string? html);
        string SanitizeBlog(string? html);
     }
}