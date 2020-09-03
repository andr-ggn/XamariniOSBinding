﻿using Foundation;
using UIKit;
using System;
using AppsFlyerXamarinBinding;

namespace AppsFlyerSampleApp
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.

    [Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		AppsFlyerLib appsflyer = AppsFlyerLib.Shared;
		public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method
			appsflyer.IsDebug = true;
			appsflyer.AppsFlyerDevKey = "4UGrDF4vFvPLbHq5bXtCza"; // Replace with your DevKey
			appsflyer.AppleAppID = "753258300"; // Replace with your app ID
			appsflyer.AppInviteOneLinkID = "E2bM"; // Replace with your OneLink ID
			appsflyer.AnonymizeUser = true;
			if (UIDevice.CurrentDevice.CheckSystemVersion (14, 0)) {
				appsflyer.WaitForAdvertisingIdentifierWithTimeoutInterval (10); // When Xamarin.iOS will support AppTrackingTransparency, you should implement a popup asking for IDFA permission
				// Ask for the IDFA permission here (with AppTrackingTransparency framework)
			}
			string [] networks = {"test_int", "partner_int"};
			appsflyer.SharingFilter = networks;



			// Conversion data callbacks
			ViewController controller = (ViewController)Window.RootViewController;
			AppsFlyerLibDelegate af_delegate = new AppsFlyerConversionDataDelegate (controller);
			AppsFlyerLib.Shared.Delegate = af_delegate;

            // Uninstall Measurement
            var settings = UIUserNotificationSettings.GetSettingsForTypes (
                UIUserNotificationType.Alert
                | UIUserNotificationType.Badge
                | UIUserNotificationType.Sound,
                new NSSet ());

            UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications ();

			return true;
		}

		public override void OnResignActivation (UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground (UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background exection this method is called instead of WillTerminate when the user quits.
		}

		public override void WillEnterForeground (UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		public override void OnActivated (UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
			appsflyer.Start();

		}

		public override void WillTerminate (UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		}

        [Export ("application:didFailToRegisterForRemoteNotificationsWithError:")]
        public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
        {
			Console.WriteLine ("AppsFLyer: Failed to register for remote notification with error: " + error.Description);
        }

        [Export ("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			appsflyer.RegisterUninstall (deviceToken);
		}

		[Export ("application:openURL:options:")]
		public override bool OpenUrl (UIApplication app, NSUrl url, NSDictionary options)
		{
            appsflyer.handleOpenUrl (url, options);
			return true;
		}
		//Universal Links
		public override bool ContinueUserActivity (UIApplication application, 
			NSUserActivity userActivity, 
			UIApplicationRestorationHandler completionHandler)
		{
			AppsFlyerLib.Shared.ContinueUserActivity (userActivity, completionHandler);
			return true;
		}
    }
}


