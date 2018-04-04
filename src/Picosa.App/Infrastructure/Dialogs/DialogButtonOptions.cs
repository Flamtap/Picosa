using System;

namespace Picosa.App.Infrastructure.Dialogs
{
    [Flags]
    public enum DialogButtonOptions
    {
        None = 0x0000,

        IsDefault = 0x0001,

        IsCancel = 0x0002,
        
        IsDangerous = 0x0004
    }
}