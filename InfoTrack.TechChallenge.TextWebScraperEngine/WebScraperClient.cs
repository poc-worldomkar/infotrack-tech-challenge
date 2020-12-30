using InfoTrack.TechChallenge.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace InfoTrack.TechChallenge.WebScraperEngine
{
    public partial class WebScraperClient : IWebScraperClient, IDisposable
    {
        private readonly Thread BrowserStaThread;
        private readonly ILogger<WebScraperClient> Logger;
        private GetPageInternalDelegate GetPageInternal;
        private XmlDocument Document;
        private ManualResetEvent BrowserReady;
        private Dictionary<string, string> AttributesToCopy;
        private HashSet<string> InnerTextElements;
        private HashSet<string> ElementsToSkip;
        private delegate HtmlDocument GetPageInternalDelegate(string url, out bool timedOut);

        public WebScraperClient(ILogger<WebScraperClient> logger)
        {
            Logger = logger;
            AttributesToCopy = new Dictionary<string, string>
            {
                { "className", "class" },
                { "id", "id" },
                { "href", "href" },
            };
            InnerTextElements = new HashSet<string>
            {
                "a",
                "b",
                "cite",
                "span",
                "p"
            };
            ElementsToSkip = new HashSet<string> {
                "script",
                "style",
                "meta"
            };
            BrowserReady = new ManualResetEvent(false);
            BrowserStaThread = RunInBrowserThread((getPageInternal, getTimedOut) =>
            {
                GetPageInternal = (string url, out bool timedOut) =>
                {
                    var htmlDocument = getPageInternal(url);
                    timedOut = getTimedOut();
                    return htmlDocument;
                };

                BrowserReady.Set();
            }, false);
        }

        // Intentional
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XmlDocument> GetPage(
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            IWebScraperSearchEngineOptions options,
            string query,
            int pageNumber,
            int pageSize,
            int dynamicPageCursorPosition = -1)
        {
            XmlDocument xmlDocument = default;
            Document = new XmlDocument();
            // Add root element
            Document.AppendChild(Document.CreateElement("html"));
            try
            {
                BrowserReady.WaitOne();
                var url = options.GetUrl(query, pageNumber, pageSize, dynamicPageCursorPosition);
                var htmlDocument = GetPageInternal(url.ToString(), out var timedOut);
                Console.WriteLine(htmlDocument.Body.OuterHtml);
                LoadXmlTree(htmlDocument, Document);
                return Document;
            }
            catch (Exception e)
            {
                Logger?.LogDebug(e.Message);
            }

            return xmlDocument;
        }

        private void LoadXmlTree(HtmlDocument htmlDocument, XmlDocument xmlDocument)
        {
            ToXmlElement(htmlDocument.Body, xmlDocument.DocumentElement);
        }

        private void ToXmlElement(HtmlElement htmlElement, XmlElement parent)
        {
            if (htmlElement == null || !Char.IsLetterOrDigit(htmlElement.TagName, 0) || ElementsToSkip.Contains(htmlElement.TagName.ToLower()))
            {
                return;
            }

            var xmlElement = Document.CreateElement(htmlElement.TagName.ToLower());
            AttributesToCopy
                .ToList()
                .ForEach(attribute => xmlElement.SetAttribute(attribute.Value, htmlElement.GetAttribute(attribute.Key)));
            // Copy only for elements of interest
            if (InnerTextElements.Contains(htmlElement.TagName.ToLower()))
            {
                xmlElement.InnerText = htmlElement.InnerText;
            }
            parent.AppendChild(xmlElement);
            if (htmlElement.Children.Count > 0)
            {
                foreach (HtmlElement child in htmlElement.Children)
                {
                    ToXmlElement(child, xmlElement);
                }
            }
        }

        public delegate void StaBrowserAction(Func<string, HtmlDocument> getPageInternal, Func<bool> timedOut);

        public static Thread RunInBrowserThreadOnce(StaBrowserAction action) =>
            RunInBrowserThread(action, true);
        public static Thread RunInBrowserThread(StaBrowserAction action, bool disposeBrowser)
        {
            var staThread = new Thread(() =>
            {
                bool timedOut = false;
                var internalHtmlClient = new WebScraperClient.InternalHtmlClient(default);
                Func<string, HtmlDocument> getPageInternal = (url) =>
                {
                    var htmlDocument = internalHtmlClient.GetPage(url);
                    timedOut = internalHtmlClient.TimedOut;
                    return htmlDocument;
                };

                Task.Run(() =>
                {
                    action(getPageInternal, () => timedOut);
                    if (disposeBrowser)
                    {
                        internalHtmlClient.Dispose();
                    }
                });

                internalHtmlClient.PumpStaThread();
            });

            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            return staThread;
        }

        public void Dispose()
        {
            BrowserStaThread.Join();
        }
    }
}