using System.Collections.Generic;
using System.Linq;

namespace Picosa.App.Infrastructure.Dialogs
{
    public class DialogViewModel : ViewModelBase
    {
        private bool? _dialogResult;
        private string _caption = App.Name;
        private string _defaultButtonCaption = "_OK";
        private string _cancelButtonCaption = "_Cancel";

        private List<DialogButtonViewModelBase> _dialogButtons;

        public string Caption
        {
            get => _caption;
            set
            {
                if (_caption == value)
                    return;

                _caption = value;

                OnPropertyChanged();
            }
        }

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

        public string DefaultButtonCaption
        {
            get => _defaultButtonCaption;
            set
            {
                if (_defaultButtonCaption == value)
                    return;

                _defaultButtonCaption = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(DialogButtons));
            }
        }

        public string CancelButtonCaption
        {
            get => _cancelButtonCaption;
            set
            {
                if (_cancelButtonCaption == value)
                    return;

                _cancelButtonCaption = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(DialogButtons));
            }
        }

        public virtual bool CanClickDefaultButton => true;

        public virtual bool CanClickCancelButton => true;

        public virtual IEnumerable<DialogButtonViewModelBase> DialogButtons
        {
            get
            {
                yield return new DialogButtonViewModel(DefaultButtonCaption, () => DialogResult = true,
                    () => CanClickDefaultButton, DialogButtonOptions.IsDefault);

                yield return new DialogButtonViewModel(CancelButtonCaption, () => DialogResult = false,
                    () => CanClickCancelButton, DialogButtonOptions.IsCancel);
            }
        }

        public void ClickDefaultButton()
        {
            DialogButtons.FirstOrDefault(b => b.IsDefault)?.Execute(null);
        }

        public void ClickCancelButton()
        {
            DialogButtons.FirstOrDefault(b => b.IsCancel)?.Execute(null);
        }

        public IEnumerable<DialogButtonViewModelBase> DangerousButtons
        {
            get
            {
                if (_dialogButtons == null)
                    _dialogButtons = new List<DialogButtonViewModelBase>(DialogButtons);

                return _dialogButtons.Where(b => b.IsDangerous);
            }
        }

        public IEnumerable<DialogButtonViewModelBase> SafeButtons
        {
            get
            {
                if (_dialogButtons == null)
                    _dialogButtons = new List<DialogButtonViewModelBase>(DialogButtons);

                return _dialogButtons.Where(b => !b.IsDangerous);
            }
        }
    }
}
