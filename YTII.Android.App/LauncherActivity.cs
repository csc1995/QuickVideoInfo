using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace YTII.Android.App
{
    [Activity(Name = FullActivityName, Label = Constants.AppTitle, MainLauncher = true, Icon = "@drawable/icon")]
    public class LauncherActivity : Activity
    {
        internal const string FullActivityName = Constants.PackageName + ".LauncherActivity";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var prefs = Application.Context.GetSharedPreferences(Constants.PackageName, FileCreationMode.Private);
            var isLauncherIconEnabled = prefs.GetBoolean("IsLaunchIconEnabled", true);

            if (isLauncherIconEnabled)
            {
                StartActivity(typeof(AboutActivity));
            }
        }
        bool IsPaused = false;
        protected override void OnPause()
        {
            base.OnPause();
            IsPaused = true;
        }
        protected override void OnResume()
        {
            base.OnResume();

            if (IsPaused)
                FinishAndRemoveTask();
        }
    }
}