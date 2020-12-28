using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace InfoTrack.TechChallenge.WebScraperEngine
{
    public partial class WebScraperClient
    {
        class InternalHtmlClient : IDisposable
        {
            private readonly WebBrowser Browser;
            private readonly ILogger Logger;
            private readonly Timer PageLoadTimeout;
            private readonly AutoResetEvent PageLoadComplete;
            private readonly AutoResetEvent NewRequest;
            private readonly AutoResetEvent DocumentReady;
            private readonly ManualResetEvent NotNavigating;
            private bool Exiting;
            private HtmlDocument Document;
            private string Url;
            // Workaround magics
            private const int PageLoadTimeoutCheckInterval = 300;   // 300 mseconds; can later be moved to appsettings
            private const int PageLoadTimeoutInterval = 3000;   // 3 seconds; can later be moved to appsettings

            private WebBrowserReadyState BrowserReadyState;

            internal InternalHtmlClient(ILogger logger)
            {
                Logger = logger;
                PageLoadTimeout = new Timer();
                PageLoadTimeout.Interval = PageLoadTimeoutInterval;
                PageLoadTimeout.Tick += PageLoadTimeout_Tick;
                PageLoadComplete = new AutoResetEvent(false);
                DocumentReady = new AutoResetEvent(false);
                NotNavigating = new ManualResetEvent(true);
                Exiting = false;
                NewRequest = new AutoResetEvent(false);
                try
                {
                    Browser = new WebBrowser() { ScriptErrorsSuppressed = true };
                    Browser.Navigating += WebBrowser_Navigating;
                    Browser.DocumentCompleted += WebBrowser_DocumentCompleted;
                    BrowserReadyState = Browser.ReadyState;
                }
                catch (Exception e)
                {
                    Logger?.LogError(e.Message);
                }
            }

            public HtmlDocument GetPage(string url)
            {
                NotNavigating.WaitOne();
                Url = url;
                NotNavigating.Reset();
                NewRequest.Set();
                DocumentReady.WaitOne();
                return Document;
            }

            public void PumpStaThread()
            {
                var newRequestAvailable = false;
                while (!Exiting)
                {
                    newRequestAvailable = NewRequest.WaitOne(500);
                    if (Exiting)
                    {
                        break;
                    }
                    if (!newRequestAvailable)
                    {
                        continue;
                    }
                    try
                    {
                        Browser.Navigate(Url);
                        var succeeded = false;
                        var timedOut = false;
                        var remainingTimeOutInterval = PageLoadTimeoutInterval;
                        #region TODO: Replace with message pump (WORKAROUND for time being below)
                        do
                        {
                            succeeded = PageLoadComplete.WaitOne(PageLoadTimeoutCheckInterval);
                            // TODO: Replace with message pump
                            Application.DoEvents();
                            remainingTimeOutInterval -= PageLoadTimeoutCheckInterval;
                            timedOut = remainingTimeOutInterval <= 0;
                        } while (!succeeded && !timedOut);
                        #endregion

                        Document = Browser.Document;
                        DocumentReady.Set();
                    }
                    catch (Exception e)
                    {
                        Logger?.LogDebug(e.Message);
                    }
                    finally
                    {
                        NotNavigating.Set();
                    }
                }
            }

            private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
            {
                PageLoadComplete.Set();
            }

            private void WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
            {
                PageLoadTimeout.Start();
            }

            private void PageLoadTimeout_Tick(object sender, EventArgs e)
            {
                if (BrowserReadyState != Browser.ReadyState)
                {
                    BrowserReadyState = Browser.ReadyState;
                    if (BrowserReadyState == WebBrowserReadyState.Interactive)
                    {
                        // Cancel timeout timer
                        PageLoadTimeout.Stop();
                        // Signal "abort page load"
                        Browser.Stop();
                    }
                }
            }

            public void Dispose()
            {
                Exiting = true;
            }
        }
    }
}