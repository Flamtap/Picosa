using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Picosa.App.Infrastructure;

namespace Picosa.App.Features.Common
{
    public class UnhandledExceptionViewModel : ViewModelBase
    {
        private bool? _dialogResult;

        public UnhandledExceptionViewModel(Exception exception) => Exception = exception;

        #region Properties

        public Exception Exception { get; }

        public bool? DialogResult
        {
            get => _dialogResult;
            set
            {
                if (_dialogResult == value)
                    return;

                _dialogResult = value;

                OnPropertyChanged();
            }
        }

        public string ExceptionLog
        {
            get
            {
                var separator = new string('#', 20);

                var sb = new StringBuilder();

                sb.AppendLine("Picosa Error Log");
                sb.AppendLine(separator);
                sb.AppendLine($"Date: {DateTime.UtcNow:R}");
                sb.AppendLine($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
                sb.AppendLine(separator);
                sb.AppendLine(Exception.ToString());
                sb.AppendLine(separator);
                sb.AppendLine("Inner Exception(s):");
                sb.AppendLine(InnerExceptions);
                sb.AppendLine(separator);
                sb.AppendLine("Stack Trace:");
                sb.AppendLine(StackTrace);

                return sb.ToString();
            }
        }

        public string InnerExceptions
        {
            get
            {
                var sb = new StringBuilder();

                for (var ex = Exception; ex != null; ex = ex.InnerException)
                    sb.AppendLine($"{ex.Message} in {ex.Source}");

                return sb.ToString();
            }
        }

        public string StackTrace
        {
            get
            {
                var sb = new StringBuilder();

                for (var ex = Exception; ex != null; ex = ex.InnerException)
                    sb.AppendLine(ex.StackTrace);

                return sb.ToString();
            }
        }

        #endregion

        #region Commands

        public ICommand IgnoreCommand => new RelayCommand(() => DialogResult = false);

        public ICommand SaveLogCommand => new RelayCommand(SaveLog);

        private void SaveLog(object owner)
        {
            var window = owner is DependencyObject depObj ? Window.GetWindow(depObj) : null;

            var dialog = new SaveFileDialog
            {
                FileName = "PicosaException",
                DefaultExt = ".txt",
                Filter = "Text Files (.txt)|*.txt|All Files|*.*"
            };

            if (dialog.ShowDialog(window) != true)
                return;

            try
            {
                using (var streamWriter = new StreamWriter(dialog.OpenFile()))
                {
                    streamWriter.Write(ExceptionLog);
                }

                DialogResult = true;
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Failed to save log.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        #endregion
    }
}
