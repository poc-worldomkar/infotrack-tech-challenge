using InfoTrack.TechChallenge.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace InfoTrack.TechChallenge.WebScraperEngine
{
    public partial class WebScraperClient : IWebScraperClient, IDisposable
    {
        private readonly Thread BrowserSTAThread;
        private readonly ILogger<WebScraperClient> Logger;
        private XmlDocument Document;
        private ManualResetEvent BrowserReady;
        private Func<string, HtmlDocument> GetPageInternal;

        public WebScraperClient(ILogger<WebScraperClient> logger)
        {
            Logger = logger;
            BrowserReady = new ManualResetEvent(false);
            BrowserSTAThread = new Thread(() =>
            {
                InternalHtmlClient internalHtmlClient = new InternalHtmlClient(logger);
                GetPageInternal = (url) =>
                {
                    var htmlDocument = internalHtmlClient.GetPage(url);
                    return htmlDocument;
                };
                BrowserReady.Set();
                internalHtmlClient.PumpStaThread();
            });
            BrowserSTAThread.SetApartmentState(ApartmentState.STA);
            BrowserSTAThread.Start();
        }

        public async Task<XmlDocument> GetPage(IWebScraperSearchEngineOptions options, string query, int pageNumber, int pageSize)
        {
            XmlDocument xmlDocument = default;
            Document = new XmlDocument();
            // Add root element
            Document.AppendChild(Document.CreateElement("html"));
            try
            {
                BrowserReady.WaitOne();
                var url = options.GetUrl(query, pageNumber, pageSize);
                var htmlDocument = GetPageInternal(url.ToString());
                LoadXmlTree(options, htmlDocument);
                return Document;
            }
            catch (Exception e)
            {
                Logger?.LogDebug(e.Message);
            }

            return xmlDocument;
        }

        private void LoadXmlTree(IWebScraperSearchEngineOptions options, HtmlDocument htmlDocument)
        {
            ToXmlElement(htmlDocument.Body, Document.DocumentElement);
        }

        private void ToXmlElement(HtmlElement htmlElement, XmlElement parent)
        {
            if (htmlElement == null || !Char.IsLetterOrDigit(htmlElement.TagName, 0))
            {
                return;
            }

            var xmlElement = Document.CreateElement(htmlElement.TagName);
            xmlElement.SetAttribute("id", htmlElement.GetAttribute("id"));
            xmlElement.SetAttribute("class", htmlElement.GetAttribute("className"));
            xmlElement.InnerText = htmlElement.InnerText;
            parent.AppendChild(xmlElement);
            if (htmlElement.Children.Count > 0)
            {
                foreach (HtmlElement child in htmlElement.Children)
                {
                    ToXmlElement(child, xmlElement);
                }
            }
        }

        public void Dispose()
        {
            BrowserSTAThread.Join();
        }
    }
}