using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace WebApp.UITests
{
    // [Parallelizable(ParallelScope.Self)]
    // [TestFixture]
    // public class FetchDataPageTest : BlazorTest
    // {
    //     [Test]
    //     public async Task HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingtoTheIntroPage()
    //     {
    //         for (int i = 1; i < 11; i++)
    //         {
    //             await Page.GotoAsync(RootUri.AbsoluteUri);
    //             await Expect(Page).ToHaveTitleAsync("Northern Mountains");
    //         }
    //     }
    // }
    [TestFixture]
    public class Tests : PageTest
    {
        public GenericWebTestServerFactory<Program> Server { get; set; }
        private string Url;

        [OneTimeSetUp]
        public void Setup()
        {
            Server = new GenericWebTestServerFactory<Program>();
            Url = Server.FullUri; //save this
        }

        [Test]
        public async Task DoesSearchWork()
        {
            await Page.GotoAsync(Url);

            await Expect(Page).ToHaveTitleAsync("Catalog");
        }
    }
}
