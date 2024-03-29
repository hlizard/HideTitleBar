﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using System.Threading.Tasks;
using System.Threading;

namespace Company.HideTitleBar
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ComVisible(true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    //[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    //[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]    //VS2012+下不工作
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string)]
    [Guid(GuidList.guidHideTitleBarPkgString)]
    public sealed class HideTitleBarPackage : AsyncPackage, IVsSolutionEvents
    {
        #region Implementation of IVsSolutionEvents

        int IVsSolutionEvents.OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            Trace.WriteLine("OnAfterOpenProject", "VSTestPackage1");
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            Trace.WriteLine("OnQueryCloseProject", "VSTestPackage1");
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            Trace.WriteLine("OnBeforeCloseProject", "VSTestPackage1");
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            Trace.WriteLine("OnAfterLoadProject", "VSTestPackage1");
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            Trace.WriteLine("OnQueryUnloadProject", "VSTestPackage1");
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            Trace.WriteLine("OnBeforeUnloadProject", "VSTestPackage1");
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            Trace.WriteLine("OnAfterOpenSolution", "VSTestPackage1");

            try
            {
                HideTitleBar();
            }
            catch
            {
            }
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            Trace.WriteLine("OnQueryCloseSolution", "VSTestPackage1");
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseSolution(object pUnkReserved)
        {
            Trace.WriteLine("OnBeforeCloseSolution", "VSTestPackage1");
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterCloseSolution(object pUnkReserved)
        {
            Trace.WriteLine("OnAfterCloseSolution", "VSTestPackage1");
            return VSConstants.S_OK;
        }

        #endregion

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public HideTitleBarPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        //protected override void Initialize()
        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            //base.Initialize();
            await base.InitializeAsync(cancellationToken, progress);

            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(this.PopupLostKeyboardFocus));
            Window mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                EventHandler layoutUpdated = null;
                layoutUpdated = delegate(object sender, EventArgs e)
                {
                    Debug.WriteLine("layoutUpdated!");
                    bool flag = false;
                    foreach (Menu current in mainWindow.FindDescendants<Menu>())
                    {
                        if (AutomationProperties.GetAutomationId(current) == "MenuBar")
                        {
                            FrameworkElement frameworkElement = current;
                            DependencyObject visualOrLogicalParent = current.GetVisualOrLogicalParent();
                            if (visualOrLogicalParent != null)
                            {
                                frameworkElement = ((visualOrLogicalParent.GetVisualOrLogicalParent() as DockPanel) ?? frameworkElement);
                            }
                            flag = true;
                            this.MenuContainer = frameworkElement;
                        }
                    }
                    if (flag)
                    {
                        mainWindow.LayoutUpdated -= layoutUpdated;
                    }
                };
                mainWindow.LayoutUpdated += layoutUpdated;
            }

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;    // 警告 VSTHRD103   GetService synchronously blocks.Await GetServiceAsync instead.

            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidHideTitleBarCmdSet, (int)PkgCmdIDList.cmdidHideTitleBar);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                mcs.AddCommand( menuItem );
            }


            //try
            //{
            //    HideTitleBar();
            //}
            //catch
            //{
            //}

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }
        #endregion

        private bool isMenuInvokeDisplay = false;
        private bool useMenuMode = false;

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            Debug.WriteLine("MenuItemCallback!");
            // Show a Message Box to prove we were here
            //IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            //Guid clsid = Guid.Empty;
            //int result;
            //Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
            //           0,
            //           ref clsid,
            //           "HideTitleBar",
            //           string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.ToString()),
            //           string.Empty,
            //           0,
            //           OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //           OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
            //           OLEMSGICON.OLEMSGICON_INFO,
            //           0,        // false
            //           out result));

            try
            {
                if(hidden)
                {
                    DisplayTitleBar();
                    isMenuInvokeDisplay = true;
                }
                else
                {
                    isMenuInvokeDisplay = false;
                    HideTitleBar();
                }

                if (!useMenuMode && !isVS2010)
                {
                    if (this._menuContainer != null)
                    {
                        this._menuContainer.IsKeyboardFocusWithinChanged -= new DependencyPropertyChangedEventHandler(this.OnMenuContainerFocusChanged);
                        this._menuContainer.MouseEnter -= new MouseEventHandler(_menuContainer_MouseEnter);
                        this._menuContainer.MouseLeave -= new MouseEventHandler(_menuContainer_MouseLeave);
                    }

                    useMenuMode = true;
                }
            }
            catch
            {
            }
        }


        private bool has_find_e = false;
        private System.Windows.FrameworkElement _e;
        private System.Windows.FrameworkElement e
        {
            get 
            {
                return has_find_e ? _e : _e = FindElement(System.Windows.Application.Current.MainWindow, "MainWindowTitleBar");
            }
        }

        private bool isVS2010 = false;

        private void HideTitleBar()
        {
            if (isMenuInvokeDisplay) return;
            Debug.WriteLine("HideTitleBar!");
            if (e != null)
            {
                e.Visibility = System.Windows.Visibility.Collapsed;
                this.hidden = true;
                return;
            }
            //else if(eWithMinY != null)    //不行，除了标题栏都变黑了
            //{
            //    eWithMinY.Visibility = System.Windows.Visibility.Collapsed;
            //    return true;
            //}
            isVS2010 = true;
            System.Windows.Application.Current.MainWindow.WindowStyle = WindowStyle.None;   //for VS2010
            this.hidden = true;
            return;
        }

        private void DisplayTitleBar()
        {
            Debug.WriteLine("DisplayTitleBar!");
            if (e != null)
            {
                e.Visibility = System.Windows.Visibility.Visible;
                this.hidden = false;
                return;
            }
            //else if(eWithMinY != null)    //不行，除了标题栏都变黑了
            //{
            //    eWithMinY.Visibility = System.Windows.Visibility.Collapsed;
            //    return true;
            //}
            System.Windows.Application.Current.MainWindow.WindowStyle = WindowStyle.SingleBorderWindow;   //for VS2010
            this.hidden = false;
            return;
        }

        private System.Windows.FrameworkElement FindElement(System.Windows.Media.Visual v, string name)
        {
            this.has_find_e = true;

            if (v == null)
                return null;

            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(v); ++i)
            {
                System.Windows.Media.Visual child =
                    System.Windows.Media.VisualTreeHelper.GetChild(v, i) as
                        System.Windows.Media.Visual;
                if (child != null)
                {
                    System.Windows.FrameworkElement e =
                        child as System.Windows.FrameworkElement;
                    //if (e != null && e.Name == name)
                    //    return e;
                    if (e != null)
                    {
                        //Debug.WriteLine(e.Name); 
                        Point position = e.PointToScreen(new Point(0d, 0d));
                        if (eWithMinY == null)
                            eWithMinY = e;
                        if (position.Y < eWithMinY.PointToScreen(new Point(0d, 0d)).Y)
                            eWithMinY = e;
                        if(e.Name == name)
                            return e;
                    }
                }
                System.Windows.FrameworkElement result = FindElement(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }

        private EnvDTE.Events events;
        private EnvDTE.DTEEvents dteEvents;

        private FrameworkElement eWithMinY;
        private bool hidden = false;

        private FrameworkElement _menuContainer;

        private FrameworkElement MenuContainer
        {
            get
            {
                return this._menuContainer;
            }
            set
            {
                Debug.WriteLine("MenuContainer_set!");
                if (this._menuContainer != null)
                {
                    this._menuContainer.IsKeyboardFocusWithinChanged -= new DependencyPropertyChangedEventHandler(this.OnMenuContainerFocusChanged);
                    this._menuContainer.MouseEnter -= new MouseEventHandler(_menuContainer_MouseEnter);
                    this._menuContainer.MouseLeave -= new MouseEventHandler(_menuContainer_MouseLeave);
                }
                this._menuContainer = value;
                if (this._menuContainer != null && !useMenuMode)
                {
                    //if (this.hidden)
                    //{
                    //    DisplayTitleBar();
                    //}
                    //else
                    //{
                    //    HideTitleBar();
                    //}
                    this._menuContainer.IsKeyboardFocusWithinChanged += new DependencyPropertyChangedEventHandler(this.OnMenuContainerFocusChanged);
                    this._menuContainer.MouseEnter += new MouseEventHandler(_menuContainer_MouseEnter);
                    this._menuContainer.MouseLeave += new MouseEventHandler(_menuContainer_MouseLeave);
                }
            }
        }

        private bool isMouseFocus = false;

        void _menuContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("_menuContainer_MouseLeave!");
            isMouseFocus = false;
        }

        void _menuContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("_menuContainer_MouseEnter!");
            if(!this.hidden && !this.IsAggregateFocusInMenuContainer(this.MenuContainer))
                HideTitleBar();
            var position = e.GetPosition(this.MenuContainer);
            if (position.Y >= 0 && position.Y <= this.MenuContainer.ActualHeight)
                isMouseFocus = true;
        }

        private void OnMenuContainerFocusChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("OnMenuContainerFocusChanged!");
            //Debug.WriteLine(string.Format("IsMouseDirectlyOver:{0} IsMouseOver:{1} IsMouseCaptured:{2} IsMouseCaptureWithin:{3}"
            //    , this.MenuContainer.IsMouseDirectlyOver, this.MenuContainer.IsMouseOver, this.MenuContainer.IsMouseCaptured, this.MenuContainer.IsMouseCaptureWithin));
            if (this.IsAggregateFocusInMenuContainer(this.MenuContainer) && !isMouseFocus)
                DisplayTitleBar();
            else
            {
                if (this.e == null)
                {
                    //var position2 = System.Windows.Application.Current.MainWindow.PointToScreen(new Point(0d, 0d));
                    //var titleBarHeight = this.MenuContainer.PointToScreen(new Point(0d, 0d)).Y - position2.Y;
                    var titleBarHeight = 23d;

                    var position = Mouse.GetPosition(this.MenuContainer);
                    if (position.Y < 0 - titleBarHeight || position.Y > 0)
                        HideTitleBar();
                }
                else
                {
                    var position = Mouse.GetPosition(this.e);
                    if (position.Y < 0 || position.Y > this.e.ActualHeight)
                        HideTitleBar();
                }
            }
        }

        private void PopupLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //Debug.WriteLine("PopupLostKeyboardFocus!");
            if (!this.hidden && this.MenuContainer != null && !this.IsAggregateFocusInMenuContainer(this.MenuContainer))
            {
                if (this.e == null)
                {
                    //var position2 = System.Windows.Application.Current.MainWindow.PointToScreen(new Point(0d, 0d));
                    //var titleBarHeight = this.MenuContainer.PointToScreen(new Point(0d, 0d)).Y - position2.Y;
                    var titleBarHeight = 23d;

                    var position = Mouse.GetPosition(this.MenuContainer);
                    if (position.Y < 0 - titleBarHeight || position.Y > 0)
                        HideTitleBar();
                }
                else
                {
                    var position = Mouse.GetPosition(this.e);
                    if (position.Y < 0 || position.Y > this.e.ActualHeight)
                        HideTitleBar();
                }
            }
        }

        private bool IsAggregateFocusInMenuContainer(FrameworkElement menuContainer)
        {
            if (menuContainer.IsKeyboardFocusWithin)
            {
                return true;
            }
            for (DependencyObject dependencyObject = (DependencyObject)Keyboard.FocusedElement; dependencyObject != null; dependencyObject = dependencyObject.GetVisualOrLogicalParent())
            {
                if (dependencyObject == menuContainer)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
