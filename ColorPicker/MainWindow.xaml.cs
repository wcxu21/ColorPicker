﻿/*
MIT License

Copyright (c) Léo Corporation

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. 
*/
using ColorPicker.Classes;
using ColorPicker.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorPicker;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		InitUI();
		GC.Collect();
	}

	DoubleAnimation expandAnimation = new()
	{
		From = 0,
		To = 180,
		Duration = new Duration(TimeSpan.FromSeconds(0.2)),
	};

	DoubleAnimation collapseAnimation = new()
	{
		From = 180,
		To = 0,
		Duration = new Duration(TimeSpan.FromSeconds(0.2)),
	};

	private void InitUI()
	{
		StateChanged += (o, e) => HandleWindowStateChanged();
		Loaded += (o, e) => HandleWindowStateChanged();
		LocationChanged += (o, e) => HandleWindowStateChanged();
		SizeChanged += (o, e) =>
		{
			PageScroller.Height = (ActualHeight - (GridRow1.ActualHeight + 68) > 0) ? ActualHeight - (GridRow1.ActualHeight + 68) : 0; // Set the scroller height
			ActionsScrollViewer.Height = ActualHeight - SideBarTop.ActualHeight - GridRow1.ActualHeight - 60;
		};

		PageCard.OnCardClick += PageCard_OnCardClick;

		HelloTxt.Text = Global.HiSentence; // Show greeting message to the user

		//TODO: Add page system
		UnCheckAllButton();
		CheckButton(HomePageBtn, true);

		PageDisplayer.Navigate(Global.HomePage);
	}

	private void PageCard_OnCardClick(object? sender, PageEventArgs e)
	{
		//TODO
	}

	private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
	{
		WindowState = WindowState.Minimized; // Minimize the window
	}

	private void MaximizeRestoreBtn_Click(object sender, RoutedEventArgs e)
	{
		WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

		HandleWindowStateChanged();
	}

	private void CloseBtn_Click(object sender, RoutedEventArgs e)
	{
		//LeavePage();
		Application.Current.Shutdown(); // Close the application
	}

	private void PinBtn_Click(object sender, RoutedEventArgs e)
	{
		Topmost = !Topmost; // Toggle
		PinBtn.Content = Topmost ? "\uF604" : "\uF602"; // Set text
	}

	private void HandleWindowStateChanged()
	{
		MaximizeRestoreBtn.Content = WindowState == WindowState.Maximized
			? "\uF670" // Restore icon
			: "\uFA40"; // Maximize icon
		MaximizeRestoreBtn.FontSize = WindowState == WindowState.Maximized
			? 18
			: 14;

		DefineMaximumSize();

		WindowBorder.Margin = WindowState == WindowState.Maximized ? new(10, 10, 0, 0) : new(10); // Set
		WindowBorder.CornerRadius = WindowState == WindowState.Maximized ? new(0) : new(5); // Set

		// Update settings information
		/*Global.Settings.IsMaximized = WindowState == WindowState.Maximized;
		SettingsManager.Save();*/
	}

	private void DefineMaximumSize()
	{
		System.Windows.Forms.Screen currentScreen = System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(this).Handle); // The current screen

		float dpiX, dpiY;
		double scaling = 100; // Default scaling = 100%

		using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
		{
			dpiX = graphics.DpiX; // Get the DPI
			dpiY = graphics.DpiY; // Get the DPI

			scaling = dpiX switch
			{
				96 => 100, // Get the %
				120 => 125, // Get the %
				144 => 150, // Get the %
				168 => 175, // Get the %
				192 => 200, // Get the % 
				_ => 100
			};
		}

		double factor = scaling / 100d; // Calculate factor

		MaxHeight = currentScreen.WorkingArea.Height / factor + 5; // Set max size
		MaxWidth = currentScreen.WorkingArea.Width / factor + 5; // Set max size
	}

	private void HomePageBtn_Click(object sender, RoutedEventArgs e)
	{
		UnCheckAllButton();
		CheckButton(HomePageBtn, true);

		PageDisplayer.Navigate(Global.HomePage);
	}

	private void BookmarksPageBtn_Click(object sender, RoutedEventArgs e)
	{

	}

	private void SettingsPageBtn_Click(object sender, RoutedEventArgs e)
	{

	}

	private void PickerBtn_Click(object sender, RoutedEventArgs e)
	{
		bool expanded = PickerPanel.Visibility == Visibility.Visible;
		PickerPanel.Visibility = expanded ? Visibility.Collapsed : Visibility.Visible; // Show/hide the picker panel

		Storyboard storyboard = new(); // Create a storyboard

		storyboard.Children.Add(expanded ? collapseAnimation : expandAnimation);
		Storyboard.SetTargetName(expanded ? collapseAnimation : expandAnimation, "PickerRotator");
		Storyboard.SetTargetProperty(expanded ? collapseAnimation : expandAnimation, new(RotateTransform.AngleProperty));

		storyboard.Begin(this); // Animate the picker panel
	}

	private void SelectorPageBtn_Click(object sender, RoutedEventArgs e)
	{
		UnCheckAllButton();
		CheckButton(SelectorPageBtn);

		PageDisplayer.Navigate(Global.SelectorPage);
	}

	private void ChromaticPageBtn_Click(object sender, RoutedEventArgs e)
	{
		UnCheckAllButton();
		CheckButton(ChromaticPageBtn);

		PageDisplayer.Navigate(Global.ChromaticWheelPage);
	}

	private void CheckButton(Button button, bool isSpecial = false)
	{
		if (isSpecial)
		{
			button.Background = new SolidColorBrush(Global.GetColorFromResource("Background1"));
		}
		else
		{
			button.Background = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
			button.Foreground = new SolidColorBrush(Global.GetColorFromResource("WindowButtonsHoverForeground1"));
		}
	}

	private void UnCheckAllButton()
	{
		// Background
		HomePageBtn.Background = new SolidColorBrush(Colors.Transparent);
		BookmarksPageBtn.Background = new SolidColorBrush(Colors.Transparent);
		SettingsPageBtn.Background = new SolidColorBrush(Colors.Transparent);

		SelectorPageBtn.Background = new SolidColorBrush(Colors.Transparent);
		ChromaticPageBtn.Background = new SolidColorBrush(Colors.Transparent);
		ConverterPageBtn.Background = new SolidColorBrush(Colors.Transparent);
		TextPageBtn.Background = new SolidColorBrush(Colors.Transparent);
		PalettePageBtn.Background = new SolidColorBrush(Colors.Transparent);
		GradientPageBtn.Background = new SolidColorBrush(Colors.Transparent);

		SelectorPageBtn.Foreground = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
		ChromaticPageBtn.Foreground = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
		ConverterPageBtn.Foreground = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
		TextPageBtn.Foreground = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
		PalettePageBtn.Foreground = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
		GradientPageBtn.Foreground = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
	}

	private void ColorToolsBtn_Click(object sender, RoutedEventArgs e)
	{
		bool expanded = ColorToolsPanel.Visibility == Visibility.Visible;
		ColorToolsPanel.Visibility = expanded ? Visibility.Collapsed : Visibility.Visible; // Show/hide the picker panel

		Storyboard storyboard = new(); // Create a storyboard

		storyboard.Children.Add(expanded ? collapseAnimation : expandAnimation);
		Storyboard.SetTargetName(expanded ? collapseAnimation : expandAnimation, "ColorToolsRotator");
		Storyboard.SetTargetProperty(expanded ? collapseAnimation : expandAnimation, new(RotateTransform.AngleProperty));

		storyboard.Begin(this); // Animate the picker panel
	}

	private void ConverterPageBtn_Click(object sender, RoutedEventArgs e)
	{
		UnCheckAllButton();
		CheckButton(ConverterPageBtn);

		PageDisplayer.Navigate(Global.ConverterPage);
	}

	private void TextPageBtn_Click(object sender, RoutedEventArgs e)
	{
		UnCheckAllButton();
		CheckButton(TextPageBtn);

		PageDisplayer.Navigate(Global.TextPage);
	}

	private void CreationBtn_Click(object sender, RoutedEventArgs e)
	{
		bool expanded = CreationPanel.Visibility == Visibility.Visible;
		CreationPanel.Visibility = expanded ? Visibility.Collapsed : Visibility.Visible; // Show/hide the picker panel

		Storyboard storyboard = new(); // Create a storyboard

		storyboard.Children.Add(expanded ? collapseAnimation : expandAnimation);
		Storyboard.SetTargetName(expanded ? collapseAnimation : expandAnimation, "CreationRotator");
		Storyboard.SetTargetProperty(expanded ? collapseAnimation : expandAnimation, new(RotateTransform.AngleProperty));

		storyboard.Begin(this); // Animate the picker panel
	}

	private void PalettePageBtn_Click(object sender, RoutedEventArgs e)
	{
		UnCheckAllButton();
		CheckButton(PalettePageBtn);

		PageDisplayer.Navigate(Global.PalettePage);
	}

	private void GradientPageBtn_Click(object sender, RoutedEventArgs e)
	{
		UnCheckAllButton();
		CheckButton(GradientPageBtn);

		PageDisplayer.Navigate(Global.GradientPage);
	}
}
