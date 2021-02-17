using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PlGui
{
    public partial class App : Application
    {
        public App() : base()
        {
            SetupUnhandledExceptionHandling();
        }

        private void SetupUnhandledExceptionHandling()
        {
            // Catch exceptions from all threads in the AppDomain.
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                ShowUnhandledException(args.ExceptionObject as Exception);

            // Catch exceptions from each AppDomain that uses a task scheduler for async operations.
            TaskScheduler.UnobservedTaskException += (sender, args) =>
                ShowUnhandledException(args.Exception);

            // Catch exceptions from a single specific UI dispatcher thread.
            Dispatcher.UnhandledException += (sender, args) =>
            {
                // If we are debugging, let Visual Studio handle the exception and take us to the code that threw it.
                if (!Debugger.IsAttached)
                {
                    args.Handled = true;
                    ShowUnhandledException(args.Exception);
                }
            };

            // Catch exceptions from the main UI dispatcher thread.
            // Typically we only need to catch this OR the Dispatcher.UnhandledException.
            // Handling both can result in the exception getting handled twice.
            Application.Current.DispatcherUnhandledException += (sender, args) =>
            {
                // If we are debugging, let Visual Studio handle the exception and take us to the code that threw it.
                if (!Debugger.IsAttached)
                {
                    args.Handled = true;
                    ShowUnhandledException(args.Exception);
                }
            };
        }

        void ShowUnhandledException(Exception ex)
        {
            var messageCaption = $"Unexpected Error Occurred";
            var messageContent = "The following exception occurred:";
            foreach (var property in typeof(Exception).GetProperties())
            {
                var value = property.GetValue(ex, null);
                if (value != null)
                    messageContent += $"\n{property.Name}: {value}\n";
            }

            new CustomMessageBox(messageContent,
                messageCaption,
                "Unexpected Error",
                CustomMessageBox.Buttons.IGNORE,
                CustomMessageBox.Icons.WARNING).ShowDialog();
        }
    }
}
