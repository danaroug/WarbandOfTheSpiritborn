using WarbandOfTheSpiritborn.Services;

namespace WarbandOfTheSpiritborn.Tests.Services
{
    public class HtmlSanitizationServiceTests
    {
        private readonly HtmlSanitizationService _sanitizer = new();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void SanitizeAbout_WhenContentIsEmpty_ReturnsEmptyString(
            string? html)
        {
            var result = _sanitizer.SanitizeAbout(html);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void SanitizeAbout_PreservesSupportedFormatting()
        {
            const string html =
                "<h1>About us</h1><p>Hello <strong>warband!</strong></p>";

            var result = _sanitizer.SanitizeAbout(html);

            // Supported About formatting should remain.
            Assert.Contains("<h1>About us</h1>", result);
            Assert.Contains("<strong>warband!</strong>", result);
        }

        [Fact]
        public void SanitizeAbout_RemovesScriptElements()
        {
            const string html =
                "<p>Safe content</p><script>alert('XSS')</script>";

            var result = _sanitizer.SanitizeAbout(html);

            // Safe content remains, but executable script content is removed.
            Assert.Contains("Safe content", result);
            Assert.DoesNotContain("<script", result.ToLowerInvariant());
            Assert.DoesNotContain("alert(", result.ToLowerInvariant());
        }

        [Fact]
        public void SanitizeAbout_RemovesEventHandlerAttributes()
        {
            const string html =
                "<p onclick=\"alert('XSS')\">Click me</p>";

            var result = _sanitizer.SanitizeAbout(html);

            // The paragraph remains, but onclick is forbidden.
            Assert.Contains("Click me", result);
            Assert.DoesNotContain("onclick", result.ToLowerInvariant());
        }

        [Fact]
        public void SanitizeAbout_RemovesDangerousLinkScheme()
        {
            const string html =
                "<a href=\"javascript:alert('XSS')\">Dangerous link</a>";

            var result = _sanitizer.SanitizeAbout(html);

            // The link text may remain, but javascript: must never survive.
            Assert.Contains("Dangerous link", result);
            Assert.DoesNotContain("javascript:", result.ToLowerInvariant());
        }

        [Fact]
        public void SanitizeAbout_PreservesSafeHttpsLink()
        {
            const string html =
                "<a href=\"https://example.com\">Safe link</a>";

            var result = _sanitizer.SanitizeAbout(html);

            Assert.Contains("https://example.com", result);
            Assert.Contains("Safe link", result);
        }

        [Fact]
        public void SanitizeBlog_PreservesQuillCodeBlock()
        {
            const string html =
                "<pre class=\"ql-syntax\">Console.WriteLine(&quot;Hello&quot;);</pre>";

            var result = _sanitizer.SanitizeBlog(html);

            // Blog posts support Quill code blocks.
            Assert.Contains("<pre", result);
            Assert.Contains("ql-syntax", result);
            Assert.Contains("Console.WriteLine", result);
        }

        [Fact]
        public void SanitizeBlog_PreservesSupportedAlignmentClass()
        {
            const string html =
                "<p class=\"ql-align-center\">Centered text</p>";

            var result = _sanitizer.SanitizeBlog(html);

            Assert.Contains("ql-align-center", result);
            Assert.Contains("Centered text", result);
        }

        [Fact]
        public void SanitizeBlog_RemovesUnsupportedCssClass()
        {
            const string html =
                "<p class=\"evil-class\">Blog content</p>";

            var result = _sanitizer.SanitizeBlog(html);

            // Only explicitly allowed Quill classes may remain.
            Assert.Contains("Blog content", result);
            Assert.DoesNotContain("evil-class", result);
        }

        [Fact]
        public void SanitizeAbout_RemovesBlogOnlyFormatting()
        {
            const string html =
                "<pre class=\"ql-syntax\">Blog-only code block</pre>";

            var result = _sanitizer.SanitizeAbout(html);

            // The stricter About policy does not allow pre or class.
            Assert.DoesNotContain("<pre", result.ToLowerInvariant());
            Assert.DoesNotContain("ql-syntax", result);
        }
    }
}
