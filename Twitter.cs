using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Tweetinvi.Credentials;
using Tweetinvi;
using Tweetinvi.Models;
using System.Diagnostics;
using Tweetinvi.Logic;
using Android.Net;
using System.Threading.Tasks;
using Java.Lang;
using Microsoft.Xna.Framework.Media;
using Tweetinvi.Exceptions;

namespace XamarinPong
{
    public class Twitter
    {
        private const string CONSUMER_KEY = "kLPYunievEyEDUPw4seLXYjlU";
        private const string CONSUMER_SECRET = "RP2dwdfht2lWjBO9J0iRMbxIBayBn2Qx3Z9Vd3FjPPqA17uBro";
        public static string PIN;
        private static ITwitterCredentials userCredentials;
        private static IAuthenticationContext authenticationContext;
        public static bool loggedIn => userCredentials != null;

        public static Android.Net.Uri LogIn()
        {
            var appCredentials = new TwitterCredentials(CONSUMER_KEY, CONSUMER_SECRET);
            authenticationContext = AuthFlow.InitAuthentication(appCredentials);
            return Android.Net.Uri.Parse(authenticationContext.AuthorizationURL);
        }

        public static void SetCredentials(string PIN)
        {
            userCredentials = AuthFlow.CreateCredentialsFromVerifierCode(PIN, authenticationContext);
            Auth.SetCredentials(userCredentials);
        }

        public static void Tweet(string msg)
        {
            Tweetinvi.Tweet.PublishTweet(msg);
        }

        public static bool SendPIN(string pin)
        {
            if (pin.Length == 7)
            {
                SetCredentials(pin);
                try
                {
                    Tweet("Hello there. I scored " + Settings.highScore + " points at #XamarinPong !");
                    return true;
                }
                catch (TwitterNullCredentialsException)
                {
                    return false;
                }
            }
            return false;
        }
    }
}