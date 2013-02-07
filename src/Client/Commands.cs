﻿using System.Windows.Input;


namespace Hauksoft.ResxTranslator.Commands
{
    public class FileCommands
    {
        static FileCommands()
        {
            Exit = new RoutedUICommand(
                "E_xit", "Exit", typeof(FileCommands),
                new InputGestureCollection
                {
                    new KeyGesture(Key.X, ModifierKeys.Control, "Ctrl-X")
                });
        }

        public static RoutedUICommand Exit { get; private set; }
    }


    public class LanguageCommands
    {
        static LanguageCommands()
        {
            FlipLanguage = new RoutedUICommand(null, null, typeof(LanguageCommands));
        }

        public static RoutedUICommand FlipLanguage { get; private set; }
    }
}
