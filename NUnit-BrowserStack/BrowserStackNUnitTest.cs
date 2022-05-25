using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;


namespace BrowserStack
{
  [TestFixture]
  public class BrowserStackNUnitTest
  {
    protected IWebDriver driver;
    protected string profile;
    protected string environment;
    private Local browserStackLocal;

    public BrowserStackNUnitTest(string profile, string environment)
    {
      this.profile = profile;
      this.environment = environment;
    }
    
    [SetUp]
    public void Init()
    {
      NameValueCollection caps = ConfigurationManager.GetSection("capabilities/" + profile) as NameValueCollection;
      NameValueCollection settings = ConfigurationManager.GetSection("environments/" + environment) as NameValueCollection;

            Console.Out.WriteLine("capapabilties---" + caps + "=====settings======" + settings);

      DesiredCapabilities capability = new DesiredCapabilities();

      foreach (string key in caps.AllKeys)
      {
        capability.SetCapability(key, caps[key]);
      }

      foreach (string key in settings.AllKeys)
      {
        capability.SetCapability(key, settings[key]);
      }

      String username = Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME");
      if(username == null)
      {
        username = ConfigurationManager.AppSettings.Get("user");
                Console.Out.WriteLine("Username"+username);
      }

      String accesskey = Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY");
      if (accesskey == null)
      {
        accesskey = ConfigurationManager.AppSettings.Get("key");
                Console.Out.WriteLine("AccessKey" + accesskey);
            }

      capability.SetCapability("user", username);
      capability.SetCapability("accessKey", accesskey);

      if (capability.GetCapability("browserstack.local") != null && capability.GetCapability("browserstack.local").ToString() == "true")
      {
        browserStackLocal = new Local();
        List<KeyValuePair<string, string>> bsLocalArgs = new List<KeyValuePair<string, string>>() {
          new KeyValuePair<string, string>("key", accesskey)
        };
        browserStackLocal.start(bsLocalArgs);
      }
            Console.Out.WriteLine("Capabiliyies---000----" + capability);

            driver = new RemoteWebDriver(new Uri("http://"+ ConfigurationManager.AppSettings.Get("server") +"/wd/hub/"), capability);
    }

    [TearDown]
    public void Cleanup()
    {
      driver.Quit();
      if (browserStackLocal != null)
      {
        browserStackLocal.stop();
      }
    }
  }
}
