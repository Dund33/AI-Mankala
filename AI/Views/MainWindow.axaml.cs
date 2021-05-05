using System;
using System.Collections.Generic;
using System.Linq;
using AI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AI.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();

            DataContextChanged += (obj, args) =>
            {
                var comboBox = this.Find<ComboBox>("ModeSelection");
                if (comboBox == null)
                    throw new NotImplementedException();
                if (DataContext == null)
                    throw new NotImplementedException();
                comboBox.SelectionChanged += (sender, eventArgs) =>
                {
                    ((MankalaViewModel)DataContext).AivsAi = comboBox.SelectedItem as string == "AIvAI";
                };
            };
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
