using System.Windows.Input;

namespace Picosa.App.Features.Editor
{
    /// <summary>
    /// Interaction logic for GlobalEditorView.xaml
    /// </summary>
    public partial class GlobalEditorView
    {
        public GlobalEditorView()
        {
            InitializeComponent();
            
            //Focus this so that InputBindings are picked up
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}
