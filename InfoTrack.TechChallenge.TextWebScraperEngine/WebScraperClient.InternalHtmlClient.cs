using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace InfoTrack.TechChallenge.WebScraperEngine
{
    public partial class WebScraperClient
    {
        public class InternalHtmlClient : IDisposable
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

            // Polling intervals -- TODO: Replace in future with using message pump
            private const int PageLoadTimeoutCheckInterval = 50;   // 50 mseconds; can later be moved to appsettings
            private const int NewRequestCheckInterval = 100;   // 100 mseconds; can later be moved to appsettings
            private const int PageLoadTimeoutInterval = 3000;   // 3 seconds; can later be moved to appsettings
            private const int PageLoadMessagePumpTimeoutInterval = 700;   // 700 mseconds; can later be moved to appsettings

            public bool TimedOut { get; private set; }

            public InternalHtmlClient(ILogger logger)
            {
                Logger = logger;
                PageLoadTimeout = new Timer();
                PageLoadTimeout.Interval = PageLoadTimeoutInterval;
                PageLoadTimeout.Tick += PageLoadTimeout_Tick;
                PageLoadComplete = new AutoResetEvent(false);
                DocumentReady = new AutoResetEvent(false);
                NotNavigating = new ManualResetEvent(true);
                Exiting = false;
                TimedOut = false;
                NewRequest = new AutoResetEvent(false);
                try
                {
                    Browser = new WebBrowser() { ScriptErrorsSuppressed = true, Size = new System.Drawing.Size { Width = 1920, Height = 1080 } };
                    Browser.Navigating += WebBrowser_Navigating;
                    Browser.DocumentCompleted += WebBrowser_DocumentCompleted;
                }
                catch (Exception e)
                {
                    Logger?.LogError(e.Message);
                    throw;
                }
            }

            public HtmlDocument GetPage(string url)
            {
                NotNavigating.WaitOne();
                Url = url;
                NotNavigating.Reset();
                NewRequest.Set();
                DocumentReady.WaitOne();
                PageLoadTimeout.Stop();
                // TODO: Replace with message pump
                Application.DoEvents();
                NotNavigating.Set();
                return Document;
            }

            public void PumpStaThread()
            {
                bool newRequestAvailable;
                while (!Exiting)
                {
                    newRequestAvailable = NewRequest.WaitOne(NewRequestCheckInterval);
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
                        TimedOut = false;
                        // PageLoadTimeout.Start();
                        var remainingTimeOutInterval = PageLoadTimeoutInterval + PageLoadMessagePumpTimeoutInterval;
                        #region TODO: Replace with message pump (WORKAROUND for time being below)
                        do
                        {
                            succeeded = PageLoadComplete.WaitOne(PageLoadTimeoutCheckInterval);
                            // TODO: Replace with message pump
                            Application.DoEvents();
                            remainingTimeOutInterval -= PageLoadTimeoutCheckInterval;
                            TimedOut = remainingTimeOutInterval <= 0;
                        } while (!succeeded && !TimedOut);
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
                        PageLoadTimeout.Stop();
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
                // Cancel timeout timer
                PageLoadTimeout.Stop();
                // Signal "abort page load" as workaround for pages with script errors
                // Actual page gets loaded in few hundred ms
                Browser.Stop();
                if (Browser.ReadyState == WebBrowserReadyState.Loading)
                {
                    // DocumentCompleted will not fire
                    PageLoadComplete.Set();
                }
            }

            public void Dispose()
            {
                Application.ExitThread();
                Exiting = true;
            }
        }
    }
}