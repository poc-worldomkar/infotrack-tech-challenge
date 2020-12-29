using InfoTrack.TechChallenge.WebScraperEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Xunit;

namespace InfoTrack.TechChallenge.Tests
{
    public class InternalHtmlClient
    {
        [Fact]
        public void InternalHtmlClientDoesNotTimeOutLivePages_ReliabilityAndRepeatabilityCheck()
        {
            bool timedOut = false;
            WebScraperClient.RunInBrowserThreadOnce((Func<string, HtmlDocument> getPageInternal, Func<bool> getTimedOut) =>
                {
                    var responsiveLiveSites = new List<string>
                    {
                        "https://www.google.com",
                        "https://www.google.com.au/search?q=infotrack",
                        "https://www.google.com/search?q=infotrack&start=10",
                        "https://www.google.com.au",
                        "https://www.bing.com",
                        "https://www.google.com.au/search?q=infotrack",
                    };
                    var timedOuts = responsiveLiveSites.Select(responsiveLiveSite =>
                    {
                        var page = getPageInternal(responsiveLiveSite);
                        return getTimedOut();
                    }).ToList();
                    timedOut = timedOuts.Any(t => t);
                })
                .Join();

            Assert.False(timedOut);
        }

        [Fact]
        public void InternalHtmlClientDoesNotTimeOutLiveBingPages()
        {
            bool timedOut = false;
            WebScraperClient.RunInBrowserThreadOnce((Func<string, HtmlDocument> getPageInternal, Func<bool> getTimedOut) =>
                {
                    var responsiveLiveSites = new List<string>
                    {
                        "https://www.bing.com/search?q=online+title+search&form=QBLH&sp=-1&pq=online+title+sear&sc=5-17&qs=n&sk=&cvid=1B6B1CBB3B8948BABB691CB9E522CA7D",
                        "https://www.bing.com/search?q=online+title+search&sp=-1&pq=online+title+sear&sc=5-17&qs=n&sk=&cvid=1B6B1CBB3B8948BABB691CB9E522CA7D&first=5&FORM=PERE",
                        "https://www.bing.com/search?q=online+title+search&sp=-1&pq=online+title+sear&sc=5-17&qs=n&sk=&cvid=1B6B1CBB3B8948BABB691CB9E522CA7D&first=15&FORM=PERE1",
                        "https://www.bing.com/search?q=online+title+search&sp=-1&pq=online+title+sear&sc=5-17&qs=n&sk=&cvid=1B6B1CBB3B8948BABB691CB9E522CA7D&first=25&FORM=PERE2",
                        "https://www.bing.com/search?q=online+title+search&sp=-1&pq=online+title+sear&sc=5-17&qs=n&sk=&cvid=1B6B1CBB3B8948BABB691CB9E522CA7D&first=35&FORM=PERE3",
                        "https://www.bing.com/search?q=online+title+search&sp=-1&pq=online+title+sear&sc=5-17&qs=n&sk=&cvid=1B6B1CBB3B8948BABB691CB9E522CA7D&first=45&FORM=PERE4",
                        "https://www.bing.com/search?q=online+title+search&sp=-1&pq=online+title+sear&sc=5-17&qs=n&sk=&cvid=1B6B1CBB3B8948BABB691CB9E522CA7D&first=55&FORM=PERE4"
                    };
                    timedOut = responsiveLiveSites.Any(responsiveLiveSite =>
                    {
                        var page = getPageInternal(responsiveLiveSite);
                        return getTimedOut();
                    });
                })
                .Join();

            Assert.False(timedOut);
        }

        [Fact]
        public void InternalHtmlClientDoesNotTimeOutOnStaticPages()
        {
            bool timedOut = false;
            WebScraperClient.RunInBrowserThreadOnce((Func<string, HtmlDocument> getPageInternal, Func<bool> getTimedOut) =>
                {
                    var page = getPageInternal("http://infotrack-tests.infotrack.com.au/Google/Page01.html");
                    page = getPageInternal("http://infotrack-tests.infotrack.com.au/Google/Page02.html");
                    page = getPageInternal("http://infotrack-tests.infotrack.com.au/Google/Page03.html");
                    page = getPageInternal("http://infotrack-tests.infotrack.com.au/Google/Page04.html");
                    page = getPageInternal("http://infotrack-tests.infotrack.com.au/Google/Page05.html");
                    page = getPageInternal("http://infotrack-tests.infotrack.com.au/Google/Page06.html");
                    page = getPageInternal("http://infotrack-tests.infotrack.com.au/Google/Page07.html");
                    page = getPageInternal("http://infotrack-tests.infotrack.com.au/Google/Page08.html");
                    page = getPageInternal("http://infotrack-tests.infotrack.com.au/Google/Page09.html");
                    page = getPageInternal("http://infotrack-tests.infotrack.com.au/Google/Page10.html");
                    timedOut = getTimedOut();
                })
                .Join();

            Assert.False(timedOut);
        }

        [Fact]
        public void InternalHtmlClientDoesNotTimeOutOnStaticBingPages()
        {
            bool timedOut = false;
            WebScraperClient.RunInBrowserThreadOnce((Func<string, HtmlDocument> getPageInternal, Func<bool> getTimedOut) =>
                {
                    var page = getPageInternal("https://infotrack-tests.infotrack.com.au/Bing/Page01.html");
                    timedOut = getTimedOut();
                })
                .Join();

            Assert.False(timedOut);
        }
    }
}
