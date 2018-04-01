using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Picosa.App
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            PropertyChanged?.Invoke(this, args);
        }
    }
}
