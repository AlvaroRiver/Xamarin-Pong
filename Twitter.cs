using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace XamarinPong
{
    public class Twitter
    {
        private static string loginURL = "https://twitter.com/login";
        private static IWebElement userInput, passwordInput, loginButton;

        public static void Login(string User, string Password)
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(loginURL);
            userInput = driver.FindElement(By.Name("session[username_or_email]"));
            passwordInput = driver.FindElement(By.Name("session[password]"));
            loginButton = driver.FindElement(By.ClassName("css-901oao r-1awozwy r-jwli3a r-6koalj r-18u37iz r-16y2uox r-1qd0xha r-a023e6 r-vw2c0b r-1777fci r-eljoum r-dnmrzs r-bcqeeo r-q4m81j r-qvutc0"));
            
            userInput.SendKeys(User);
            passwordInput.SendKeys(Password);
            loginButton.Click();

        }

        public static void Main(string[] args)
        {
            Twitter.Login("fwkebhfweg", "dsfdsfdfd");
        }
    }
}